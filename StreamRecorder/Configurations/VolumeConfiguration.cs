using System;

namespace StreamRecorder.Configurations
{
    public class VolumeConfiguration
    {
        public string SavingPath { get; set; }
        public TimeSpan FetchingInterval { get; set; }
    }
}