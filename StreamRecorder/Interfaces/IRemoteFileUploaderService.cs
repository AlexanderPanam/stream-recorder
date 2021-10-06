using System.Threading;
using System.Threading.Tasks;

namespace StreamRecorder.Interfaces
{
    public interface IRemoteFileUploaderService
    {
        Task UploadFileToRemoteAsync(string filename, CancellationToken cancellationToken);
    }
}