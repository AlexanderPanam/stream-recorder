using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace StreamRecorder.HostedServices
{
    public class FileUploaderHostedService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new System.NotImplementedException();
        }
    }
}