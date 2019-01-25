using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCA.Services.RepositoryNook.Config;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace CCA.Services.RepositoryNook.Tasks
{
    public class Worker : IWorker
    {
        private ILogger _logger;
        private IJsonConfiguration _config;

        public Worker(IJsonConfiguration config, ILogger logger)                 //ctor
        {
            _logger = logger;
            _config = config;
        }

        public async Task DoTheTask()
        {
            try
            {
                await Task.Run(() => MeaningfulWork());
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "DoTheTask() task error, while attempting MeaningfulWork() async method call.");
            }
        }

        private void MeaningfulWork()
        {

            try
            {
               //peanutButters = _repo.ReadAllPeanutButters();   // code that executes, as a periodic scheduled worker task
            }
            catch(Exception exc)
            {
                _logger.LogError(exc, "Error reading peanut butter table.");
                throw exc;
            }
        }
    }
}
