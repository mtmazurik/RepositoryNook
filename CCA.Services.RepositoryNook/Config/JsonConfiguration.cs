using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CCA.Services.RepositoryNook.Exceptions;


namespace CCA.Services.RepositoryNook.Config
{
    public class JsonConfiguration : IJsonConfiguration
    {
        private IConfiguration _configuration;
        public JsonConfiguration()              // ctor
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            _configuration = configBuilder.Build();
        }


        public string AtlasMongoConnection
        {
            get
            {
                string connectionString = _configuration["AtlasMongoConnection"];
                if (connectionString is null) throw new ConfigFileReadError("Check appsettings.json; AtlasMongoConnection not found.");
                return connectionString;
            }
        }

        public double TaskManagerIntervalSeconds
        {
            get
            {
                string intervalString = _configuration["TaskManagerIntervalSeconds"];
                if (intervalString is null) throw new ConfigFileReadError("Check appsettings.json; TaskManagerIntervalSeconds not found.");
                return Convert.ToDouble(intervalString);
            }
        }

        public bool EnforceTokenLife
        {
            get
            {
                var enforceTokenLife = true;
                if (!bool.TryParse(_configuration["EnforceTokenLife"], out enforceTokenLife))                
                    throw new ConfigFileReadError("Check appsettings.json; EnforceTokenLife not found.");

                return enforceTokenLife;
            }
        }
    }
}
