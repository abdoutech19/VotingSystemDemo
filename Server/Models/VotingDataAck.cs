using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public sealed class VotingDataAck
    {
        public string RequestId { get; }

        public VotingDataAck(string requestId)
        {
            RequestId = requestId;
        }
    }
}
