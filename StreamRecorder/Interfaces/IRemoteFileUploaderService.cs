using System.Threading.Tasks;

namespace StreamRecorder.Interfaces
{
    public interface IRemoteFileUploaderService
    {
        Task UploadFileToRemote(string filename);
    }
}