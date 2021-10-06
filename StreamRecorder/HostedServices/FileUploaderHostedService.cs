using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StreamRecorder.Configurations;
using StreamRecorder.Interfaces;

namespace StreamRecorder.HostedServices
{
    public class FileUploaderHostedService : BackgroundService
    {
        private readonly VolumeConfiguration _volumeConfiguration;
        private readonly IFileWriteCompletionMonitor _completionMonitor;
        private readonly IRemoteFileUploaderService _remoteFileUploaderService;
        private readonly ILogger<FileUploaderHostedService> _logger;

        public FileUploaderHostedService(IOptions<VolumeConfiguration> volumeConfiguration,
            IFileWriteCompletionMonitor completionMonitor,
            IRemoteFileUploaderService remoteFileUploaderService, ILogger<FileUploaderHostedService> logger)
        {
            _completionMonitor = completionMonitor;
            _remoteFileUploaderService = remoteFileUploaderService;
            _logger = logger;
            _volumeConfiguration = volumeConfiguration.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var completedFiles = await _completionMonitor.GetCompletedFilesAsync(stoppingToken);
                    var taskList = new List<Task>();
                    foreach (var completedFile in completedFiles)
                    {
                        taskList.Add(_remoteFileUploaderService.UploadFileToRemoteAsync(completedFile, stoppingToken));
                    }

                    await Task.WhenAll(taskList.ToArray());
                    await Task.Delay(_volumeConfiguration.FetchingInterval, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error:");
                }
            }
        }
    }
}