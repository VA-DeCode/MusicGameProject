using System.Collections.Generic;

namespace MusicGameProject.Core
{
    public class DoorData
    {
        public string TargetLevel { get; set; }
        public char TargetDoor { get; set; }
        public string Direction { get; set; } // "Left", "Right", "Top", "Bottom"
    }

    public class LevelData
    {
        public string[] Map { get; set; }
        public Dictionary<char, DoorData> Doors { get; set; }
    }
}