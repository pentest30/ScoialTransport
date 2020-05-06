using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace tacchograaph_reader.Core.Commands.DddFiles
{
    public class AddDDDFileCommand : IRequest
    {
        public IList<IFormFile> DddFiles  { get; set; }
        public List<string> Paths { get; set; }
        public IHostingEnvironment HostingEnvironment { get; set; }

    }
}
