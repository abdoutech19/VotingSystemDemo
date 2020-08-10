using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public interface IAuthenticationRespond {}
    public sealed class AuthenticationReply : IAuthenticationRespond
    {
        public string RequestId { get; }
        public string SessionSignature { get; }
        public string SateZipCode { get; }
        public string StateName { get; set; }
        public AuthenticationReply(string requestId, string sessionSignature, string sateZipCode, string stateName)
        {
            RequestId = requestId;
            SessionSignature = sessionSignature;
            SateZipCode = sateZipCode;
            StateName = stateName;
        }
    }
}
