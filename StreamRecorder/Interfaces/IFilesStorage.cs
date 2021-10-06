using System.Collections.Generic;

namespace StreamRecorder.Interfaces
{
    public interface IFilesStorage
    {
        void Add(int pid, string filename);
        IDictionary<int, string> GetAll();
        bool Delete(int pid);
        bool Delete(IEnumerable<int> pids);
    }
}