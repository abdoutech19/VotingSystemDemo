using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public sealed class RegistrationRequest
    {
        public string RequestId { get; }
        public string PublicKey { get; }
        public string Username { get; }
        public string Password { get; }
        public string StateZipCode { get; set; }

        public RegistrationRequest(string requestId, string publicKey, string username, string password, string stateZipCode)
        {
            RequestId = requestId;
            PublicKey = publicKey;
            Username = username;
            Password = password;
            StateZipCode = stateZipCode;
        }
    }
}
