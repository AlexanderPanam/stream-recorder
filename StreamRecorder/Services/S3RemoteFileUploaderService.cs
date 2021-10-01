using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.Extensions.Options;
using StreamRecorder.Configurations;
using StreamRecorder.Interfaces;

namespace StreamRecorder.Services
{
    public class S3RemoteFileUploaderService : IRemoteFileUploaderService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly VolumeConfiguration _volumeConfiguration;

        public S3RemoteFileUploaderService(IAmazonS3 s3Client, IOptions<VolumeConfiguration> volumeConfiguration)
        {
            _s3Client = s3Client;
            _volumeConfiguration = volumeConfiguration.Value;
        }

        public Task UploadFileToRemote(string filename)
        {
            return _s3Client.UploadObjectFromFilePathAsync("recordings", filename,
                $"{_volumeConfiguration.SavingPath}/{filename}", null);
        }
    }
}