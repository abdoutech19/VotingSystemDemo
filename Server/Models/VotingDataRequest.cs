using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public sealed class VotingDataRequest
    {
        public string RequestId { get; }
        public string PublicKey { get; }
        public string SessionSignature { get; }
        public string StateZipCode { get; }

        public VotingDataRequest(string requestId, string publicKey, string sessionSignature, string stateZipCode)
        {
            RequestId = requestId;
            PublicKey = publicKey;
            SessionSignature = sessionSignature;
            StateZipCode = stateZipCode;
        }
    }
}
