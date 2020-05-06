using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TachoReader.Data.Data;

namespace TachographReader.Application.helpers
{
    public static class ConfigHelper 
    {
		private static DbContextOptionsBuilder<ApplicationDbContext> _builder;

		public static DbContextOptionsBuilder<ApplicationDbContext> DbContextOptionsBuilder
		{
            get
            {
                if (_builder != null)
                    return _builder;
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                _builder = new DbContextOptionsBuilder<ApplicationDbContext>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                _builder.UseSqlServer(connectionString);
                return _builder;
            }
            set { _builder = value; }
		}

	}
}
