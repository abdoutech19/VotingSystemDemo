using Akka.Actor;
using Akka.Event;
using NSec.Cryptography;
using Server.DataAccess;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    class AccountVerifier : UntypedActor
    {
        public ILoggingAdapter Logging { get; } = Context.GetLogger();
        private VotingContext _dbContext = new VotingContext();
        protected override void PostStop()
        {
            Context.Parent.Tell(new FinishedQuery(Self));
        }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case AccountVerificationRequest request:
                    try
                    {
                        if (UserExists(request, out Voter voter, out bool wrongCredentials))
                        {
                            if (TryApproveUser(voter))
                            {
                                Sender.Tell(SuccessfullyVerified.Instance);
                            }
                            else
                            {
                                Sender.Tell(new InternalErrorOccurred(request.RequestId));
                            }
                        }
                        else
                        {
                            if (wrongCredentials)
                            {
                                Sender.Tell(new InvalidCredentials(request.RequestId));
                            }
                            else
                            {
                                Sender.Tell(new UserNotFound(request.RequestId));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logging.Error(e, "Could not find user. Error Conntecting to Database.");
                        Sender.Tell(new InternalErrorOccurred(request.RequestId));
                    }
                    Context.Stop(Self);
                    break;
            }
        }
        public static Props Props() =>
            Akka.Actor.Props.Create(() => new AccountVerifier());
        private bool TryApproveUser(Voter voter)
        {
            try
            {
                voter.Verified = true;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logging.Error(e, "Could not approve user. Error Connecting to Database");
                return false;
            }
        }
        private bool UserExists(AccountVerificationRequest request, out Voter voter, out bool wrongCredentials)
        {
            wrongCredentials = false;
            byte[] passwordHash = HashAlgorithm.Sha256.Hash(Encoding.UTF8.GetBytes(request.Password));
            string password = Convert.ToBase64String(passwordHash);
            
            try
            {
                voter = _dbContext.Voters
                    .Where(v => v.Id.Equals(request.UserId) && v.PublicKey.Equals(request.Publickey))
                    .FirstOrDefault();
                if (voter.Username.Equals(request.Username) && voter.Password.Equals(password))
                {
                    return true;
                }
                else
                {
                    wrongCredentials = true;
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }     
    }
    public sealed class SuccessfullyVerified
    {
        public static SuccessfullyVerified Instance { get; } = new SuccessfullyVerified();
        private SuccessfullyVerified() { }
    }
}
