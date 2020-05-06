using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tacchograaph_reader.Core.Entities;
using TachographReader.Application.Dtos;
using TachographReader.Application.Services;
using TachoReader.Data.Data;

namespace TachographReader.Application.Queries
{
    public class DriverQueries : BaseService, IDriverQueries
    {
        public DriverQueries(ApplicationDbContext context) : base(context)
        {
        }

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
    }
}
