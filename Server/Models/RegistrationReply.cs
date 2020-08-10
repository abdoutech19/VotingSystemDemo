using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    class RegistrationReply
    {
        public string RequestId { get; }
        public int UserId { get; }

        public RegistrationReply(string requestId, int userId)
        {
            RequestId = requestId;
            UserId = userId;
        }
    }
}
