using CCA.Services.RepositoryNook.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Service
{
    public interface IRepositoryNookService
    {
        string kill();
        void Create(RepositoryNookModel nookObject);
    }
}
