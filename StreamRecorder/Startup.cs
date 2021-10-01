using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StreamRecorder.Configurations;
using StreamRecorder.Interfaces;

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
            foreach (var streamConfiguration in streamsConfiguration.StreamsConfigurations)
            {
                services.AddScoped<IRecorder, BaseRecorder>(provider =>
                    new BaseRecorder(provider.GetRequiredService<ILogger<BaseRecorder>>(), streamConfiguration));
            }
            services.AddHostedService<RecorderHostedService>();
        }

        public void Configure(IApplicationBuilder app)
        {
        }
    }
}