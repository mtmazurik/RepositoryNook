using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Service
{
    public class RepositoryNookService : IRepositoryNookService
    {
        private IApplicationLifetime _applicationLifetime;
        public RepositoryNookService(IApplicationLifetime applicationLifetime)     // ctor
        {
            _applicationLifetime = applicationLifetime;
        } 
        public string kill()
        {
            _applicationLifetime.StopApplication();
            return "RepositoryNook service stopped.";
        }
    }
}
