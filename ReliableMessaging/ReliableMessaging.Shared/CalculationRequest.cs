using System;
using System.Collections.Generic;
using System.Text;

namespace ReliableMessaging.Shared
{
    public class CalculationRequest
    {
        public double BallPositionY { get; set; }
        public double PlayerPositionY { get; set; }
        public double PlayerHeight { get; set; }
    }
}
