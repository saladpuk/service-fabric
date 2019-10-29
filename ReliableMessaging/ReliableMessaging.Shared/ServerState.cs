using System;
using System.Collections.Generic;
using System.Text;

namespace ReliableMessaging.Shared
{
    public class ServerState
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }
}
