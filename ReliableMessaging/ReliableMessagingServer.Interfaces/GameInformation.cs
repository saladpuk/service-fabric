using System;
using System.Collections.Generic;
using System.Text;

namespace ReliableMessagingServer.Interfaces
{
    public class GameInformation
    {
        public string Tag { get; set; }
        public double BallPositionY { get; set; }
        public double PlayerPositionY { get; set; }
        public double PlayerHeight { get; set; }
    }
}
