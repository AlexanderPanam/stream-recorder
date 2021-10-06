using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StreamRecorder.Configurations;
using StreamRecorder.Interfaces;

namespace StreamRecorder.Services
{
    public class BaseRecorder : IRecorder
    {
        private readonly ILogger<BaseRecorder> _logger;
        private readonly StreamConfiguration _streamConfiguration;
        private readonly VolumeConfiguration _volumeConfiguration;
        private readonly IFilesStorage _filesStorage;
        private bool _stopped = false;

        public BaseRecorder(ILogger<BaseRecorder> logger, StreamConfiguration streamConfiguration, VolumeConfiguration volumeConfiguration, IFilesStorage filesStorage)
        {
            _logger = logger;
            _streamConfiguration = streamConfiguration;
            _volumeConfiguration = volumeConfiguration;
            _filesStorage = filesStorage;
        }

        public Task RecordAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var url = _streamConfiguration.Url;
                _logger.LogInformation($"Start streamlink process for parsing from {url}");
                while (!cancellationToken.IsCancellationRequested && !_stopped)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = @"streamlink",
                        Arguments = $"--hls-duration 02:00 -o {_volumeConfiguration.SavingPath}/{fileName}.mpeg {url} best",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };
                    _logger.LogDebug($"Start iteration of recording for {url}");
                    var process = Process.Start(processStartInfo);
                    process?.WaitForExit();
                    var output = process.StandardOutput.ReadToEnd();
                    _logger.LogInformation($"Exit code {url}: {process.ExitCode}");
                    if (process.ExitCode == 1)
                    {
                        _logger.LogWarning($"Stop iterating {url} due stream inactivity");
                        _stopped = true;
                    }
                    else
                    {
                        _filesStorage.Add(process.Id, fileName);
                    }
                    _logger.LogDebug(output);
                }

                _logger.LogInformation($"Shutdown recorder for {url}");
            }, cancellationToken);
        }
    }
}