using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public sealed class AuthenticationRequest : IAuthenticationRespond
    {
        public string RequestId { get; }
        public int UserId { get; }
        public string PublicKey { get; }
        public string Username { get; }
        public string Password { get; }

        public AuthenticationRequest(string requestId, int userId, string publicKey, string username, string password)
        {
            RequestId = requestId;
            UserId = userId;
            PublicKey = publicKey;
            Username = username;
            Password = password;
        }
    }
}
