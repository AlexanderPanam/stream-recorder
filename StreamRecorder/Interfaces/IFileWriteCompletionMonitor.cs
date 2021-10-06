using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StreamRecorder.Interfaces
{
    public interface IFileWriteCompletionMonitor
    {
        Task<IEnumerable<string>> GetCompletedFilesAsync(CancellationToken cancellationToken);
    }
}