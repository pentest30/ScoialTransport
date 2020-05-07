using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tacchograaph_reader.Core.Entities;
using TachographReader.Application.Dtos;
using TachographReader.Application.Dtos.Driver;
using TachographReader.Application.Services;
using TachoReader.Data.Data;

namespace TachographReader.Application.Queries
{
    public class DriverQueries : BaseService, IDriverQueries
    {
        public DriverQueries(ApplicationDbContext context) : base(context) { }
       
        public async Task<List<Driver>> GetDriversAsync(Guid customerId)
        {
            var drivers = await (from d in Context.Drivers
                where d.CustomerId.Equals(customerId)
                select d).ToListAsync().ConfigureAwait(false);
            return drivers;
        }

       public async Task<DatatablesQueryModel<LegalFilesQueryViewModel>> GetLegalFilesAsync(Guid customerId, string term, int start, int length)
        {
             var total = await Context.LegalFiles.CountAsync(x => x.CustomerId == customerId).ConfigureAwait(false);
            int recordsFilterd = total;
            var query = (from f in Context.LegalFiles
                    join driver in Context.Drivers on f.DriverId equals driver.Id
                    where f.CustomerId == customerId
                    select f
                );
            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(x => x.Driver.Name.Contains(term));
                recordsFilterd = query.Count();
            }

            var data = await query.Skip(start)
                .Take(length).Select(x => new LegalFilesQueryViewModel
                {
                    GenerationDateUtc = x.GenerationDateUtc.ToShortDateString(),
                    FileName = x.FileName,
                    Id = x.Id.ToString(),
                    Driver = x.Driver.Name
                })
                .ToListAsync()
                .ConfigureAwait(false);

            var result = new DatatablesQueryModel<LegalFilesQueryViewModel>
            {
                Data = data.OrderBy(x => x.Driver),
                RecordsTotal = total,
                RecordsFilterd = recordsFilterd

            };
            return result;
        }


        public Task<int> GetLegalFilesCountAsync(Guid customerId)
        {
            return Context.LegalFiles.CountAsync(x => x.CustomerId == customerId);
        }

        public async Task<DatatablesQueryModel<DriverQueryViewModel>> GetLisOfDriversAsync(Guid customerId, string term,
            int start, int length)
        {
            var total = await Context.Drivers.CountAsync(x => x.CustomerId == customerId).ConfigureAwait(false);
            int recordsFilterd = total;
            var query = (from d in Context.Drivers
              
                let cardNumber  = (from idf in Context.Identifiers
                   orderby idf.CardExpiryDate descending
                   where idf.DriverId == d.Id
                       select idf.CardNumber).FirstOrDefault()
               
               let expiryDate = (from idf in Context.Identifiers
                   orderby idf.CardExpiryDate descending
                   where idf.DriverId == d.Id
                   select idf.CardExpiryDate).FirstOrDefault()
                         
               let lastDownloadDate = (from dailyAct in Context.CardActivityDailyRecords.DefaultIfEmpty()
                    orderby dailyAct.Date descending
                    where dailyAct.CardNumber == cardNumber
                    select dailyAct.Date).FirstOrDefault()
                        
                where d.CustomerId.Equals(customerId)
                select  new {d.Name, d.BirthDate, d.DriverNumber, d.Id, d.Tel, cardNumber, expiryDate,lastDownloadDate});
            if (!string.IsNullOrEmpty(term))
            {
                query = query
                    .Where(x => x.Name.Contains(term)
                                || x.DriverNumber.Contains(term)
                                || x.cardNumber.Contains(term));
                recordsFilterd = query.Count();
            }

            var data = await query.Skip(start)
                .Take(length).Select(x => new DriverQueryViewModel
                {
                    FullName = x.Name,
                    DrivingLicenseNumber = x.DriverNumber,
                    CardNumber = x.cardNumber,
                    ExpiryDate = x.expiryDate!= default? x.expiryDate.ToShortDateString(): string.Empty,
                    Phone = x.Tel,
                    LastDownloadDate = x.lastDownloadDate!=default? x.lastDownloadDate.ToShortDateString():string.Empty,
                    Id = x.Id,
                })
                .ToListAsync()
                .ConfigureAwait(false);

            var result = new DatatablesQueryModel<DriverQueryViewModel>
            {
                Data = data.OrderBy(x => x.FullName),
                RecordsTotal = total,
                RecordsFilterd = recordsFilterd

            };
            return result;
        }

        public async Task<IEnumerable<DriverDto>> GetListOfDriversForApiAsync(Guid customerId)
        {
            var query =  (from d in Context.Drivers

                join identifier in Context.Identifiers on d.Id equals identifier.DriverId into identifiers
                from idf in identifiers.DefaultIfEmpty()
                
                let lastDownloadDate = (from dailyAct in Context.CardActivityDailyRecords.DefaultIfEmpty()
                    orderby dailyAct.Date descending
                    where dailyAct.CardNumber == idf.CardNumber
                    select dailyAct.Date).FirstOrDefault()

                where d.CustomerId.Equals(customerId)
                select new { d.Name, d.BirthDate, d.DriverNumber, d.Id, d.Tel, lastDownloadDate });
            IEnumerable<DriverDto> result = await query
                .Select(q => new DriverDto
                {
                    FullName = q.Name,
                    Phone = q.Tel,
                    LastDownloadDate = q.lastDownloadDate,
                    BirthDate = q.BirthDate,
                    DrivingLicenseNumber = q.DriverNumber,
                    Id = q.Id
                })
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var q in result)
            {
                q.Identifiers = Context.Identifiers.Where(x => x.DriverId == q.Id).Select(id => new IdentifierDto()
                {
                    CardExpiryDate = id.CardExpiryDate,
                    CardIssueDate = id.CardIssueDate,
                    CardNumber = id.CardNumber
                });
            }
            return result;
        }

        public async Task<Driver> GetDriverByIdAsync(Guid driverId)
        {
            return await Context.Drivers.FindAsync(driverId).ConfigureAwait(false);
        }
    }
}
