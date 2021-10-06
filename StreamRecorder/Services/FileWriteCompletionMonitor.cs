using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StreamRecorder.Interfaces;

namespace StreamRecorder.Services
{
    public class FileWriteCompletionMonitor : IFileWriteCompletionMonitor
    {
        private readonly IFilesStorage _filesStorage;

        public FileWriteCompletionMonitor(IFilesStorage filesStorage)
        {
            _filesStorage = filesStorage;
        }

        public async Task<IEnumerable<string>> GetCompletedFilesAsync(CancellationToken cancellationToken)
        {
            var processToFileMappings = _filesStorage.GetAll();
            var allProcesses = Process.GetProcesses().ToList();
            var finishedProcesses = processToFileMappings.Keys.Except(allProcesses.Select(x => x.Id)).ToList();
            var completedFiles = finishedProcesses.Select(x => processToFileMappings[x]).ToList();
            _filesStorage.Delete(finishedProcesses);

            return completedFiles;
        }
    }
}