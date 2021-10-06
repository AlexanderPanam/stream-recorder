using System.Collections.Concurrent;
using System.Collections.Generic;
using StreamRecorder.Interfaces;

namespace StreamRecorder.Storages
{
    public class FilesStorage : IFilesStorage
    {
        private readonly IDictionary<int, string> _processesToFilesMappings = new ConcurrentDictionary<int, string>();

        public void Add(int pid, string filename)
        {
            _processesToFilesMappings.Add(pid, filename);
        }

        public IDictionary<int, string> GetAll()
        {
            return _processesToFilesMappings;
        }

        public bool Delete(int pid)
        {
            return _processesToFilesMappings.Remove(pid);
        }

        public bool Delete(IEnumerable<int> pids)
        {
            var result = true;
            foreach (var pid in pids)
            {
                result = result && _processesToFilesMappings.Remove(pid);
            }

            return result;
        }
    }
}