using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DataFileReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using tacchograaph_reader.Core.Entities;
using TachoReader.Data.Data;

namespace TachographReader.Application.Services
{
    public class CardFilesService : BaseService, ICardFilesService
    {
        
        public CardFilesService(ApplicationDbContext context) : base(context) { }
        
        public async Task HandleDddFilesAsync(IList<IFormFile> dddFiles, List<string> filePaths , IHostingEnvironment _env)
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
                    var xmlFile = ParseDriverFileCad(webRootPath, path);
                    // deserialize  the xml file to driverData object
                    var driverData = DeserializeXmlFIle(webRootPath, xmlFile);
                    var driver = await AddDriverAsync(driverData).ConfigureAwait(false);
                    await SaveLegalFileOfDriverAsync(driver, path).ConfigureAwait(false);
                    var identifier = await AddIdentifierAsync(driverData, driver).ConfigureAwait(false);
                    await AddDriverCarActivitiesAsync(driverData, identifier).ConfigureAwait(false);
                 }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }

        private async Task AddDriverCarActivitiesAsync(DriverData.DriverData driverData, Identifier identifier)
        {
            foreach (var record in driverData.DriverActivityData.CardDriverActivity.CardActivityDailyRecord)
            {
                var currentDate = DateTime.Parse((string) record.DateTime);
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
                    if(activity!= null )
                        return;
                }

                foreach (var activityChangeInfo in record.ActivityChangeInfo)
                {
                    
                    TimeSpan time = TimeSpan.Parse(activityChangeInfo.Time);
                    var activityType =(DriverActivityType) Enum.Parse(typeof(DriverActivityType), activityChangeInfo.Activity);
                    byte slot = (byte) (activityChangeInfo.Slot == "0"? 0 : 1);
                    var  activity = new CardDriverActivity
                    {
                        ActivityDailyRecordId = dailyActivity.Id,
                        TimeSpan = time,
                        SlotOne = slot,
                        CardPresent = activityChangeInfo.Inserted !="False",
                        DriverActivityType = activityType,
                        Offset = activityChangeInfo.FileOffset,
                        ActivityUtc = currentDate.Add(time)
                    };
                    await Context.CardDriverActivities.AddAsync(activity).ConfigureAwait(false);
                }

                await Context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private async Task<Driver> AddDriverAsync(DriverData.DriverData driverData)
        {
            var fullName = driverData.Identification.DriverCardHolderIdentification.CardHolderFirstNames + " " +
                           driverData.Identification.DriverCardHolderIdentification.CardHolderSurname;
            var driver = await Context.Drivers
                .FirstOrDefaultAsync(x =>  x.Name == fullName)
                .ConfigureAwait(false);
            if (driver == null)
            {
                var customerId = Guid.Parse("13bce473-5e31-4f3f-87a0-8863b6a814f5");
                var customer = await Context.Customers.FirstOrDefaultAsync(x => x.Id == customerId)
                    .ConfigureAwait(false);
                driver = new Driver
                {
                    Name = driverData.Identification.DriverCardHolderIdentification.CardHolderFirstNames + " "+ driverData.Identification.DriverCardHolderIdentification.CardHolderSurname,
                    BirthDate = DateTime.Parse((string) driverData.Identification.DriverCardHolderIdentification.CardHolderBirthDate
                        .Datef),
                    CustomerId = customer.Id
                };
                await Context.Drivers.AddAsync(driver).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);
            }

            return driver;
        }

        private async Task SaveLegalFileOfDriverAsync(Driver driver, string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            string fileName = path.Split("\\").LastOrDefault();
            var generatedUtc = DateTime.ParseExact(String.Concat(fileName.Skip(17).Take(10)),
                "yyMMddHHmm", new CultureInfo("fr-FR"));
            var legalFile = await Context.LegalFiles.FirstOrDefaultAsync(f => f.GenerationDateUtc == generatedUtc)
                .ConfigureAwait(false);
            if (legalFile == null)
            {
                Context.LegalFiles.Add(new LegalFile
                {
                    FileName = fileName,
                    GenerationDateUtc = generatedUtc,
                    DriverId = driver.Id,
                    FileContent = fileBytes,
                    CustomerId = driver.CustomerId
                });
            }
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }
        private async Task<Identifier> AddIdentifierAsync(DriverData.DriverData driverData, Driver driver)
        {
            
            var cardNumber = driverData.Identification.CardIdentification.CardNumber.ToString();
            var expiryDate = DateTime.Parse((string) driverData.Identification.CardIdentification.CardExpiryDate.DateTime);
            var identifier = await Context.Identifiers.FirstOrDefaultAsync(x =>
                x.CardNumber == cardNumber 
                && x.DriverId == driver.Id 
                && x.CustomerId == driver.CustomerId
                && x.CardExpiryDate>= expiryDate).ConfigureAwait(false);
            if (identifier != null) return identifier;
            identifier = new Identifier
            {
                CardNumber = cardNumber,
                DriverId = driver.Id,
                CustomerId = driver.CustomerId,
                CardIssueDate = DateTime.Parse((string) driverData.Identification.CardIdentification.CardIssueDate.DateTime),
                CardValidityBegin = DateTime.Parse((string) driverData.Identification.CardIdentification.CardValidityBegin.DateTime),
                CardExpiryDate = DateTime.Parse((string) driverData.Identification.CardIdentification.CardExpiryDate.DateTime),

            };
            await Context.Identifiers.AddAsync(identifier).ConfigureAwait(false);
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return identifier;
        }
        private static DriverData.DriverData DeserializeXmlFIle(string webRootPath, string xmlFile)
        {
            StreamReader sr = new StreamReader(string.Concat(webRootPath,xmlFile));
            XmlSerializer xml = new XmlSerializer(typeof(DriverData.DriverData));

            var res = xml.Deserialize(sr);
            sr.Close();
            return (DriverData.DriverData) res;
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

        private  async Task<string> SaveDriverCardFileAsync(List<string> filePaths, string webRootPath, IFormFile formFile)
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
