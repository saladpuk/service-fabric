using System;
using System.Collections.Generic;
using System.Text;

namespace ReliableMessaging.Shared
{
    public class ServerState
    {
        public bool IsLeftServerSending { get; set; }
        public bool IsServerActive { get; set; }
    }
}
