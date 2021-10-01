using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StreamRecorder.Interfaces;

namespace StreamRecorder.HostedServices
{
    public class RecorderHostedService : BackgroundService
    {
        private readonly IEnumerable<IRecorder> _recorders;
        private readonly ILogger<RecorderHostedService> _logger;

        public RecorderHostedService(IEnumerable<IRecorder> recorders, ILogger<RecorderHostedService> logger)
        {
            _recorders = recorders;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Recorder Hosted service is starting");
            var taskList = new List<Task>();
            foreach (var recorder in _recorders)
            {
                taskList.Add(recorder.RecordAsync(stoppingToken));
            }
            return Task.WhenAll(taskList);
        }
    }
}