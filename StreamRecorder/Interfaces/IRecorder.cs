using System.Threading;
using System.Threading.Tasks;

namespace StreamRecorder.Interfaces
{
    public interface IRecorder
    {
        Task RecordAsync(CancellationToken cancellationToken);
    }
}