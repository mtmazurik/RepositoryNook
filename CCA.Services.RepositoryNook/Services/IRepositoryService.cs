using CCA.Services.RepositoryNook.Models;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Services
{
    public interface IRepositoryService
    {
        Task<Repository> Create(Repository repoObject);
        Task Delete(Repository repoObject);
    }
}