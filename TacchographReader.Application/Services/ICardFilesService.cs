using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace TachographReader.Application.Services
{
    public interface ICardFilesService
    {
        Task HandleDddFilesAsync(IList<IFormFile> dddFiles, List<string> filePaths , IHostingEnvironment _env);
    }
}