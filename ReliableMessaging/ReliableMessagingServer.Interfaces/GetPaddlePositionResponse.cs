using System;
using System.Collections.Generic;
using System.Text;

namespace ReliableMessagingServer.Interfaces
{
    public class GetPaddlePositionResponse
    {
        public string Tag { get; set; }
        public double MovementPoistionY { get; set; }
    }
}
