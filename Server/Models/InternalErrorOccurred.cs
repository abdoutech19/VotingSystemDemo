using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public sealed class InternalErrorOccurred : IAuthenticationRespond
    {
        public string RequestId { get; }
        public static InternalErrorOccurred Instance { get; } = new InternalErrorOccurred();
        private InternalErrorOccurred() { }
        public InternalErrorOccurred(string requestId)
        {
            RequestId = requestId;
        }
    }
}
