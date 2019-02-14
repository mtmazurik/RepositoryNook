using CCA.Services.RepositoryNook.Models;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Services
{
    public interface IRepositoryService
    {
        Task<Repository> Create(string repository, string collection, Repository repoObject);
        Task<Repository> Read(string repository, string collection, Repository repoObject);
        Task Update(string repository, string collection, Repository repoObject);
        Task Delete(string repository, string collection, Repository repoObject);
    }
}