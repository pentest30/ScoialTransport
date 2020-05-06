using System;

namespace TachographReader.Application.Dtos
{
    public class LegalFilesQueryViewModel
    {
        public string Id { get; set; }
        public string GenerationDateUtc { get; set; }
        public String FileName { get; set; }
        public Guid CustomerId { get; set; }
       
        public string Driver { get; set; }
    }
}
