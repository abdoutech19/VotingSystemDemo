using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public sealed class UserNotFound : IAuthenticationRespond
    {
        public string RequestId{ get; }

        public UserNotFound(string requestId)
        {
            this.RequestId = requestId;
        }
    }
}
