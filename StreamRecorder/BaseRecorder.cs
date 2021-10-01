using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StreamRecorder.Configurations;
using StreamRecorder.Interfaces;

namespace StreamRecorder
{
    public class BaseRecorder : IRecorder
    {
        private readonly ILogger<BaseRecorder> _logger;
        private readonly StreamConfiguration _streamConfiguration;
        private readonly VolumeConfiguration _volumeConfiguration;

        public BaseRecorder(ILogger<BaseRecorder> logger, StreamConfiguration streamConfiguration, VolumeConfiguration volumeConfiguration)
        {
            _logger = logger;
            _streamConfiguration = streamConfiguration;
            _volumeConfiguration = volumeConfiguration;
        }

        public Task RecordAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var url = _streamConfiguration.Url;
                _logger.LogInformation($"Start streamlink process for parsing from {url}");
                while (!cancellationToken.IsCancellationRequested)
                {
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = @"streamlink",
                        Arguments = $"--hls-duration 02:00 -o {_volumeConfiguration.SavingPath}/{Guid.NewGuid()}.mpeg {url} best",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };
                    _logger.LogDebug($"Start iteration of recording for {url}");
                    var process = Process.Start(processStartInfo);
                    process?.WaitForExit();
                    var output = process?.StandardOutput.ReadToEnd();
                    _logger.LogInformation($"Exit code {url}: {process.ExitCode}");
                    _logger.LogDebug(output);
                }

                _logger.LogInformation($"Shutdown recorder for {url}");
            }, cancellationToken);
        }
    }
}