using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Services
{
    public class AdminService : IAdminService
    {
        private IApplicationLifetime _applicationLifetime;

        public AdminService(IApplicationLifetime applicationLifetime)     // ctor
        {
            _applicationLifetime = applicationLifetime;

        }
        public string kill()
        {
            _applicationLifetime.StopApplication();
            return "RepositoryNook service stopped.";
        }

        public string version()
        {
            return typeof(Startup).Assembly.GetName().Version.ToString();
        }
    }
}
