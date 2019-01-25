using CCA.Services.RepositoryNook.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Tasks
{
    internal class TaskManager : BackgroundService
    {
        private readonly IWorker _worker;
        private double _intervalSeconds;
        private ILogger _logger;
        private IJsonConfiguration _config;

        public TaskManager(IJsonConfiguration config, IWorker worker, ILogger logger)
        {
            _worker = worker;                             // simple task injection model
            _logger = logger;
            _config = config;
            _intervalSeconds = _config.TaskManagerIntervalSeconds;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            { 
                cancellationToken.Register(() => _logger.LogDebug($"TaskManager background task service is stopping."));

                _logger.LogInformation("TaskManager dispatch loop started.");
                while (true)                                                                                // example of forever loop (polling)
                {
                    await Task.Delay(TimeSpan.FromSeconds( _intervalSeconds )).ContinueWith(tsk => { } );   // timer   ,  .ContinueWith() swallows the exception
                    //    _logger.Debug($"awake: TaskManager dispatch loop" );

                    //await _worker.DoTheTask();       // task manager worker routine, run asynchronously                   
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "TaskManager.ExecuteAsync error.");
            }
        }
    }
}
