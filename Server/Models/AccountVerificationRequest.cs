using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public sealed class AccountVerificationRequest
    {
        public string RequestId { get; }
        public int UserId { get; }
        public string Publickey { get; }
        public string Username { get; }
        public string Password { get; }

        public AccountVerificationRequest(string requestId, int userId, string publickey, string username, string password)
        {
            RequestId = requestId;
            UserId = userId;
            Publickey = publickey;
            Username = username;
            Password = password;
        }
    }
}
