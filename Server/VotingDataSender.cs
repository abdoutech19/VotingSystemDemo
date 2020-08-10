using Akka.Actor;
using Akka.Event;
using NSec.Cryptography;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class VotingDataSender : ReceiveActor
    {
        protected ILoggingAdapter Logging { get; }
        protected string SessionPrivateKey { get; }
        protected string UserSessionSignature { get; }
        protected List<Candidate> Candidates { get; }
        protected  List<string> ConsensusNodesAddresses { get; }
        private const int _timeout = 5;
        
        public VotingDataSender(string sessionPrivateKey, string userSessionSignature, List<Candidate> candidates, List<string> consensusNodesAddresses)
        {
            SessionPrivateKey = sessionPrivateKey;
            UserSessionSignature = userSessionSignature;
            Candidates = candidates;
            ConsensusNodesAddresses = consensusNodesAddresses;

            Receive<VotingDataRequest>(request => 
            {
                if (VerifySessionSignature(request))
                {
                    Sender.Tell(new VotingDataReply(request.RequestId, Candidates.ToArray(), ConsensusNodesAddresses.ToArray()));
                    Context.SetReceiveTimeout(TimeSpan.FromSeconds(_timeout));
                    Become(WaitForAckOrRetryOnce(request.RequestId));   
                }
                else
                {
                    Sender.Tell(new InvalidSessionSignature(request.RequestId));
                }
            
            });
        }
        protected override void PostStop()
        {
            Context.Parent.Tell(new FinishedQuery(Self));
        }
        public static Props Props(string sessionPrivateKey, string userSesssionSignature, List<Candidate> candidates, List<string> cnAddresses) =>
            Akka.Actor.Props.Create(() => new VotingDataSender(sessionPrivateKey, userSesssionSignature, candidates, cnAddresses));
        private bool VerifySessionSignature(VotingDataRequest request)
        {
            var privateKeyBlob = Convert.FromBase64String(SessionPrivateKey);
            using var privateKey = Key.Import(SignatureAlgorithm.Ed25519, privateKeyBlob, KeyBlobFormat.NSecPrivateKey);
            var signature = Convert.FromBase64String(request.SessionSignature);
            var publicKeyHash = HashAlgorithm.Sha256.Hash(Encoding.UTF8.GetBytes(request.PublicKey));

            return SignatureAlgorithm.Ed25519.Verify(privateKey.PublicKey, publicKeyHash, signature);
        }
        private UntypedReceive WaitForAckOrRetryOnce(string requestId)
        {
            return message => 
            {
                switch (message)
                {
                    case ReceiveTimeout _:
                        Sender.Tell(new VotingDataReply(requestId, Candidates.ToArray(), ConsensusNodesAddresses.ToArray()));
                        Context.Stop(Self);
                        break;
                    case VotingDataAck ack when ack.RequestId.Equals(requestId):
                        Context.Stop(Self);
                        break;
                }
            };
        }
    }
}
