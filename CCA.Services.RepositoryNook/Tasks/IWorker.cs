using System.Threading;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Tasks
{
    public interface IWorker
    {
        Task DoTheTask();
    }
}