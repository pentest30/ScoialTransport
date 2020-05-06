using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tacchograaph_reader.Core.Entities;
using TachographReader.Application.Dtos;

namespace TachographReader.Application.Queries
{
    public interface IDriverQueries
    {
        Task<List<Driver>> GetDriversAsync(Guid customerId);
        Task<DatatablesQueryModel<LegalFilesQueryViewModel>> GetLegalFilesAsync(Guid customerId ,string term , int start, int length);
        Task<int> GetLegalFilesCountAsync(Guid customerId);
    }
}