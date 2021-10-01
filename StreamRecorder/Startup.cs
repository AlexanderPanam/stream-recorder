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
            services.AddDefaultAWSOptions(_configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            foreach (var streamConfiguration in streamsConfiguration.StreamsConfigurations)
            {
                services.AddSingleton<IRecorder, BaseRecorder>(provider =>
                    new BaseRecorder(provider.GetRequiredService<ILogger<BaseRecorder>>(),
                        streamConfiguration,
                        provider.GetRequiredService<IOptions<VolumeConfiguration>>().Value));
            }
            services.AddHostedService<RecorderHostedService>();
        }

        public void Configure(IApplicationBuilder app)
        {
        }
    }
}