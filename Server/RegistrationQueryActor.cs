using Akka.Actor;
using Akka.Event;
using NSec.Cryptography;
using Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Models
{
    //Actor responsible for adding one user to the database.
    class RegistrationQueryActor : UntypedActor
    {
        protected ILoggingAdapter Logging { get; } = Context.GetLogger();
        protected int SessionId { get; }
        private VotingContext _dbContext = new VotingContext();
        public RegistrationQueryActor(int sessionId)
        {
            SessionId = sessionId;
        }
        protected override void PostStop()
        {
            Context.Parent.Tell(new FinishedQuery(Self));
            Logging.Info("Registration Query Has Finished");
        }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RegistrationRequest request:

                    int? voterId = AddUserToDatabase(request, out bool invalidState);
                    if (!voterId.Equals(null))
                    {
                        Sender.Tell(new RegistrationReply(request.RequestId, (int) voterId));
                    }
                    else if (invalidState)
                    {
                        Sender.Tell(new InvalidState(request.RequestId));
                    }
                    else
                    {
                        Sender.Tell(new InternalErrorOccurred(request.RequestId));
                    }
                    Context.Stop(Self);
                    break;
            }
        }
        public static Props Props(int sessionId) =>
            Akka.Actor.Props.Create(() => new RegistrationQueryActor(sessionId));
        private int? AddUserToDatabase(RegistrationRequest request, out bool invalidState)
        {
            var userPasswordHash = HashAlgorithm.Sha256.Hash(Encoding.UTF8.GetBytes(request.Password));
            invalidState = false;
            try
            {
                if(_dbContext.States.Any(s => s.ZipCode.Equals(request.StateZipCode)))
                {
                    Voter voter = new Voter
                    {
                        Username = request.Username,
                        Password = Convert.ToBase64String(userPasswordHash),
                        PublicKey = request.PublicKey,
                        VotingSession = _dbContext.Sessions.Where(s => s.Id == SessionId).FirstOrDefault(),
                        VoterState = _dbContext.States.Where(s => s.ZipCode.Equals(request.StateZipCode)).FirstOrDefault()
                    };
                    _dbContext.Voters.Add(voter);
                    _dbContext.SaveChanges();
                    return voter.Id;
                }
                else
                {
                    invalidState = true;
                    return null;
                }
            }
            catch (Exception e)
            {
                Logging.Error(e, "Error Connecting To Database");
                return null;
            }
        }
       
    }
    public sealed class InvalidState
    {
        public string RequestId { get; }

        public InvalidState(string requestId)
        {
            RequestId = requestId;
        }
    }
}
