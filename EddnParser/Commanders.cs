using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EddnParser
{
    public sealed partial class Commanders
    {
        public Commanders(string id)
        {
            Id = id.Trim();
            Messages= new ObservableCollection<CommanderMessages>();
        }

        public string Id { get; set; } = "";
        public ObservableCollection<CommanderMessages> Messages { get; set; }

        public TimeSpan LongestStopInSystem { get; set; }
        public TimeSpan TotalTimeInSystem { get; set; }
        public TimeSpan SessionTime { get; set; }
        public int JumpCount { get; set; }
    }
}
