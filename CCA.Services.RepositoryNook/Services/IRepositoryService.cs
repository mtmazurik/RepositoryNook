using CCA.Services.RepositoryNook.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Services
{
    public interface IRepositoryService
    {
        Task<Repository> Create(string repository, string collection, Repository repoObject);
        Task<Repository> Read(string _id, string repository, string collection);
        List<Repository> QueryByKey(string repository, string collection, string keyName, string keyValue);
        Task Update(string _id, string repository, string collection, Repository repoObject);
        Task Delete(string _id, string repository, string collection);
    }
}