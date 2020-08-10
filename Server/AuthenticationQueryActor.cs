using Akka.Actor;
using Akka.Event;
using Microsoft.EntityFrameworkCore;
using NSec.Cryptography;
using Server.DataAccess;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    // Actor responsible for authenticating clients by verifiying their existance in database.
    class AuthenticationQueryActor : UntypedActor
    {
        protected ILoggingAdapter Logging { get; } = Context.GetLogger();
        protected string SessionPrivatekey { get; }
        private VotingContext _dbContext = new VotingContext();

        public AuthenticationQueryActor(string sessionPrivatekey)
        {
            SessionPrivatekey = sessionPrivatekey;
        }

        protected override void PostStop()
        {
            Context.Parent.Tell(new FinishedQuery(Self));
            Logging.Info($"Authentication Query Actor has stopped.");
        }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case AuthenticationRequest request:
                    var reply = VerifyUserInDatabase(request);
                    if (reply != null)
                    {
                        Sender.Tell(reply);
                        Context.Stop(Self);
                    }
                    else
                    {
                        Sender.Tell(new InternalErrorOccurred(request.RequestId));
                    }
                    break;
            }
        }
        public static Props Props(string privateKey) => 
            Akka.Actor.Props.Create(() => new AuthenticationQueryActor(privateKey));
        
        private IAuthenticationRespond VerifyUserInDatabase(AuthenticationRequest request)
        {
            var userPasswordHash = HashAlgorithm.Sha256.Hash(Encoding.UTF8.GetBytes(request.Password));
            string userPassword = Convert.ToBase64String(userPasswordHash);
            try
            {
                Voter voter = _dbContext.Voters
                .Where(v => v.Verified == true)
                .Where(v => v.Id == request.UserId && v.PublicKey == request.PublicKey)
                .Include(v => v.VotingSession)
                .Include(_ => _.VoterState)
                .FirstOrDefault();

                bool successfullyFound =  voter != null && (voter.Id.Equals(request.UserId) && voter.PublicKey.Equals(request.PublicKey) && voter.Username.Equals(request.Username) && voter.Password.Equals(userPassword)) ? true : false;

                if (successfullyFound)
                {
                    var keyBlob = Convert.FromBase64String(SessionPrivatekey);
                    //Committing the user's public key
                    string sessionSignature = SignUserData(keyBlob, voter.PublicKey);

                    return new AuthenticationReply(request.RequestId, sessionSignature, voter.VoterState.ZipCode, voter.VoterState.StateName);
                }
                else if (voter != null && voter.Id.Equals(request.UserId) && voter.PublicKey.Equals(request.PublicKey))
                {
                    return new InvalidCredentials(request.RequestId);
                }
                else
                {
                    return new UserNotFound(request.RequestId);
                }
            }
            catch (Exception e)
            {
                Logging.Error(e, "Error Connecting To Database");
                return null;
            }
            
        }
        private string SignUserData(byte[] keyBlob, string data)
        {
            using var key = Key.Import(SignatureAlgorithm.Ed25519, keyBlob, KeyBlobFormat.NSecPrivateKey);
            var dataHash = HashAlgorithm.Sha256.Hash(Encoding.UTF8.GetBytes(data));
            var signature = SignatureAlgorithm.Ed25519.Sign(key, dataHash);
            return Convert.ToBase64String(signature);
        }
     
    }
    public sealed class FinishedQuery
    {
        public IActorRef ActorRef { get; }

        public FinishedQuery(IActorRef actorRef)
        {
            ActorRef = actorRef;
        }
    }

}
