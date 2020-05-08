using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DataFileReader;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using tacchograaph_reader.Core.Commands.DddFiles;
using tacchograaph_reader.Core.Entities;
using TachographReader.Application.helpers;
using TachoReader.Data.Data;

namespace TachographReader.Application.Handlers
{
    public class DriverCardFileHandler : AsyncRequestHandler<AddDDDFileCommand>
    {
        protected override Task Handle(AddDDDFileCommand notification, CancellationToken cancellationToken)
        {
            return HandleDddFilesAsync(notification.DddFiles, notification.Paths, notification.HostingEnvironment);
        }
        private async Task HandleDddFilesAsync(IList<IFormFile> dddFiles, List<string> filePaths, IHostingEnvironment _env)
        {
            string webRootPath = _env.WebRootPath;
            foreach (var formFile in dddFiles)
            {
                if (formFile.Length <= 0) continue;
                // save c1b file and gets  full path 
                var path = await SaveDriverCardFileAsync(filePaths, webRootPath, formFile).ConfigureAwait(false);
                try
                {
                    //parses the binary file and save it as xml file using tacho reader project
                    var context = new ApplicationDbContext(ConfigHelper.DbContextOptionsBuilder.Options);

                    var xmlFile = ParseDriverFileCad(webRootPath, path);
                    // deserialize  the xml file to driverData object
                    var driverData = DeserializeXmlFIle(webRootPath, xmlFile);
                    var driver = await AddDriverAsync(driverData, context).ConfigureAwait(false);
                     var identifier = await AddIdentifierAsync(driverData, driver, context).ConfigureAwait(false);
                    var lastActUtc = await AddDriverCarActivitiesAsync(driverData, identifier, context).ConfigureAwait(false);
                    await SaveLegalFileOfDriverAsync(driver, path, lastActUtc, context).ConfigureAwait(false);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }

        private async Task<DateTime>  AddDriverCarActivitiesAsync(DriverData.DriverData driverData, Identifier identifier, ApplicationDbContext Context)
        {
            var currentDate = default(DateTime);
            foreach (var record in driverData.DriverActivityData.CardDriverActivity.CardActivityDailyRecord)
            { 
                currentDate = DateTime.Parse((string)record.DateTime);
                var dailyActivity = await Context.CardActivityDailyRecords.FirstOrDefaultAsync(x =>
                    x.Date == currentDate && x.CardNumber == identifier.CardNumber).ConfigureAwait(false);
                if (dailyActivity == null)
                {
                    dailyActivity = new CardActivityDailyRecord
                    {
                        CardNumber = identifier.CardNumber,
                        Date = currentDate.Date,
                        TotalDistance = double.Parse(record.Distance)
                    };
                    await Context.CardActivityDailyRecords.AddAsync(dailyActivity).ConfigureAwait(false);

                }

                var lastAct = record.ActivityChangeInfo.LastOrDefault();
                if (lastAct != null)
                {
                    TimeSpan time = TimeSpan.Parse(lastAct.Time);
                    var activityType = (DriverActivityType)Enum.Parse(typeof(DriverActivityType), lastAct.Activity);
                    byte slot = (byte)(lastAct.Slot == "0" ? 0 : 1);

                    var activity = await Context.CardDriverActivities
                        .FirstOrDefaultAsync(x =>
                            x.TimeSpan == time
                            && x.DriverActivityType == activityType
                            && x.ActivityDailyRecordId == identifier.Id
                            && x.SlotOne == slot)
                        .ConfigureAwait(false);
                    if (activity != null)
                        return activity.ActivityUtc;
                }

                foreach (var activityChangeInfo in record.ActivityChangeInfo)
                {

                    TimeSpan time = TimeSpan.Parse(activityChangeInfo.Time);
                    var activityType = (DriverActivityType)Enum.Parse(typeof(DriverActivityType), activityChangeInfo.Activity);
                    byte slot = (byte)(activityChangeInfo.Slot == "0" ? 0 : 1);
                    var activity = new CardDriverActivity
                    {
                        ActivityDailyRecordId = dailyActivity.Id,
                        TimeSpan = time,
                        SlotOne = slot,
                        CardPresent =Boolean.Parse( activityChangeInfo.Inserted .ToLower()),
                        DriverActivityType = activityType,
                        Offset = activityChangeInfo.FileOffset,
                        ActivityUtc = currentDate.Add(time)
                    };
                    await Context.CardDriverActivities.AddAsync(activity).ConfigureAwait(false);
                }

                await Context.SaveChangesAsync().ConfigureAwait(false);
            }

            return currentDate;

        }

        private async Task<Driver> AddDriverAsync(DriverData.DriverData driverData, ApplicationDbContext Context)
        {
            var fullName = driverData.Identification.DriverCardHolderIdentification.CardHolderFirstNames + " " +
                           driverData.Identification.DriverCardHolderIdentification.CardHolderSurname;
            var driver = await Context.Drivers
                .FirstOrDefaultAsync(x => x.Name == fullName)
                .ConfigureAwait(false);
            if (driver == null)
            {
                var customerId = Guid.Parse("13bce473-5e31-4f3f-87a0-8863b6a814f5");
                var customer = await Context.Customers.FirstOrDefaultAsync(x => x.Id == customerId)
                    .ConfigureAwait(false);
                driver = new Driver
                {
                    Name = driverData.Identification.DriverCardHolderIdentification.CardHolderFirstNames + " " + driverData.Identification.DriverCardHolderIdentification.CardHolderSurname,
                    BirthDate = DateTime.Parse((string)driverData.Identification.DriverCardHolderIdentification.CardHolderBirthDate
                        .Datef),
                    CustomerId = customer.Id
                };
                await Context.Drivers.AddAsync(driver).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);
            }

            return driver;
        }

        private async Task SaveLegalFileOfDriverAsync(Driver driver, string path, DateTime generatedFileUtc, ApplicationDbContext context)
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            string fileName = path.Split("\\").LastOrDefault();
            var generatedUtc = generatedFileUtc;
             var legalFile = await context.LegalFiles.FirstOrDefaultAsync(f => f.GenerationDateUtc == generatedUtc)
                .ConfigureAwait(false);
            if (legalFile == null)
            {
                context.LegalFiles.Add(new LegalFile
                {
                    FileName = fileName,
                    GenerationDateUtc = generatedUtc,
                    DriverId = driver.Id,
                    FileContent =fileBytes,
                    CustomerId = driver.CustomerId
                });
            }
            await context.SaveChangesAsync().ConfigureAwait(false);
            File.Delete(path);
        }
        private async Task<Identifier> AddIdentifierAsync(DriverData.DriverData driverData, Driver driver, ApplicationDbContext Context)
        {

            var cardNumber = driverData.Identification.CardIdentification.CardNumber.Text;
            var expiryDate = DateTime.Parse((string)driverData.Identification.CardIdentification.CardExpiryDate.DateTime);
            var identifier = await Context.Identifiers.FirstOrDefaultAsync(x =>
                x.CardNumber == cardNumber
                && x.DriverId == driver.Id
                && x.CustomerId == driver.CustomerId
                && x.CardExpiryDate >= expiryDate).ConfigureAwait(false);
            if (identifier != null) return identifier;
            identifier = new Identifier
            {
                CardNumber = cardNumber,
                DriverId = driver.Id,
                CustomerId = driver.CustomerId,
                CardIssueDate = DateTime.Parse((string)driverData.Identification.CardIdentification.CardIssueDate.DateTime),
                CardValidityBegin = DateTime.Parse((string)driverData.Identification.CardIdentification.CardValidityBegin.DateTime),
                CardExpiryDate = DateTime.Parse((string)driverData.Identification.CardIdentification.CardExpiryDate.DateTime),

            };
            await Context.Identifiers.AddAsync(identifier).ConfigureAwait(false);
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return identifier;
        }
        private static DriverData.DriverData DeserializeXmlFIle(string webRootPath, string xmlFile)
        {
            StreamReader sr = new StreamReader(string.Concat(webRootPath, xmlFile));
            XmlSerializer xml = new XmlSerializer(typeof(DriverData.DriverData));
            var res = xml.Deserialize(sr);
            sr.Close();
            return (DriverData.DriverData)res;
        }

        private static string ParseDriverFileCad(string webRootPath, string path)
        {
            var xmlFile = $@"\uploads\result-xml-{DateTime.Now:yyMMddHHmm}.xml";
            DataFile vudf = DriverCardDataFile.Create();
            var xtw = new XmlTextWriter(string.Concat(webRootPath, $@"\uploads\result-xml-{DateTime.Now:yyMMddHHmm}.xml"), Encoding.UTF8);
            vudf.Process(path, xtw);
            xtw.Close();
            xtw.Dispose();
            return xmlFile;
        }

        private async Task<string> SaveDriverCardFileAsync(List<string> filePaths, string webRootPath, IFormFile formFile)
        {
            var path = string.Concat(webRootPath, @"\uploads\" + formFile.FileName);
            filePaths.Add(path);
            await using (var stream = new FileStream(path, FileMode.Create))
            {
                await formFile.CopyToAsync(stream, CancellationToken.None).ConfigureAwait(false);
            }

            return path;
        }

      
    }
}
