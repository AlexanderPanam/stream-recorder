using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StreamRecorder.Configurations;
using StreamRecorder.HostedServices;
using StreamRecorder.Interfaces;
using StreamRecorder.Services;
using StreamRecorder.Storages;

namespace StreamRecorder
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var streamsConfiguration = _configuration.GetSection("streams").Get<StreamsConfiguration>();
            services.Configure<VolumeConfiguration>(_configuration.GetSection("volume"));
            services.AddSingleton<IAmazonS3>(provider =>
            {
                var awsOptions = _configuration.GetAWSOptions();
                var config = new AmazonS3Config
                {
                    AuthenticationRegion = awsOptions.DefaultClientConfig.AuthenticationRegion,
                    ServiceURL = awsOptions.DefaultClientConfig.ServiceURL,
                    ForcePathStyle = true,
                };
                return new AmazonS3Client(awsOptions.Credentials, config);
            });
            foreach (var streamConfiguration in streamsConfiguration.StreamsConfigurations)
            {
                services.AddSingleton<IRecorder, BaseRecorder>(provider =>
                    new BaseRecorder(provider.GetRequiredService<ILogger<BaseRecorder>>(),
                        streamConfiguration,
                        provider.GetRequiredService<IOptions<VolumeConfiguration>>().Value,
                        provider.GetRequiredService<IFilesStorage>()));
            }

            services.AddSingleton<IFilesStorage, FilesStorage>()
                .AddSingleton<IFileWriteCompletionMonitor, FileWriteCompletionMonitor>()
                .AddSingleton<IRemoteFileUploaderService, S3RemoteFileUploaderService>();
            services.AddHostedService<FileUploaderHostedService>();
            services.AddHostedService<RecorderHostedService>();
        }

        public void Configure(IApplicationBuilder app)
        {
        }
    }
}