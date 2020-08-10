using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public sealed class InvalidSessionSignature
    {
        public string RequestId { get; }

        public InvalidSessionSignature(string requestId)
        {
            RequestId = requestId;
        }
    }
}
