using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    class InvalidCredentials : IAuthenticationRespond
    {
        public string RequstId { get; }
        public static InvalidCredentials Instance { get; } = new InvalidCredentials();
        private InvalidCredentials() { }
        public InvalidCredentials(string requstId)
        {
            RequstId = requstId;
        }
    }
}
