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
    class SessionMasterManager : ReceiveActor
    {
        protected ILoggingAdapter Logging { get; } = Context.GetLogger();
        public string SessionName { get; }
        public string Password { get;}
        private VotingContext _dbContext = new VotingContext();
        private readonly  Session _currentVotingSession;
        private IActorRef _authenticationManager = null;
        private IActorRef _registrationManager = null;
        private IActorRef _votingDataSupervisor = null;
        private IActorRef _accountVerificationManager = null;
        public SessionMasterManager(string sessionName, string password)
        {
            SessionName = sessionName;
            Password = password;
            _currentVotingSession = CreateSession(SessionName, Password);

            Receive<StartVoting>(message =>
            {
                // create this actor when all candidates and consensus nodes are registred and voting has officially started.
                _votingDataSupervisor = Context.ActorOf(VotingDataSupervisor.Props(_currentVotingSession.PrivateKey),"VotingDataService");
                Context.Watch(_votingDataSupervisor);
            });
            Receive<AuthenticationRequest>(message => 
            {
                _authenticationManager.Forward(message);
            });
            Receive<RegistrationRequest>(message => 
            {
                _registrationManager.Forward(message);
            });
            Receive<VotingDataRequest>(message => 
            {
                if (_votingDataSupervisor != null)
                {
                    _votingDataSupervisor.Forward(message);
                }
                else
                {
                    Sender.Tell(DataNotAvailable.Instance);
                }
            });
            Receive<AccountVerificationRequest>(message =>
            {
                _accountVerificationManager.Forward(message);
            });
        }
        public static Props Props(string sessionName, string password) =>
            Akka.Actor.Props.Create(() => new SessionMasterManager(sessionName, password));
        protected override void PreStart()
        {
            _authenticationManager = Context.ActorOf(AuthenticationManager.Props(_currentVotingSession),"AuthenticationService");
            _registrationManager = Context.ActorOf(RegistrationManager.Props(_currentVotingSession.Id),"RegistrationService");
            _accountVerificationManager = Context.ActorOf(AccountVerificationManager.Props(_currentVotingSession),"AccountVerificationService");
            Context.Watch(_authenticationManager);
            Context.Watch(_registrationManager);
            Context.Watch(_accountVerificationManager);
        }
        protected override void PostStop()
        {
            Logging.Info("Session Stopped.");
        }

        private Session CreateSession(string sessionName, string password)
        {
            using Key key = Key.Create(SignatureAlgorithm.Ed25519, 
                new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextArchiving });
            var keyBlob = key.Export(KeyBlobFormat.NSecPrivateKey);
            string privateKey = Convert.ToBase64String(keyBlob);
            string timeStamp = DateTime.Now.ToLocalTime().ToString();
            byte[] passwordHash = HashAlgorithm.Sha256.Hash(Encoding.UTF8.GetBytes(password));
            
            try
            {
                var votingSession = _dbContext.Sessions
                    .Add(new Session { PrivateKey = privateKey, TimeStamp = timeStamp, SessionName = sessionName, Password = Convert.ToBase64String(passwordHash) })
                    .Entity;
                _dbContext.SaveChanges();
                return votingSession;
            }
            catch (Exception e)
            {
                Logging.Error(e, "Could not create session. Error Connecting To Database");
                throw e;
            }
        }
    }
    public sealed class StartVoting
    {
        public static StartVoting Instance { get; } = new StartVoting();
        private StartVoting() { }
    }
    public sealed class DataNotAvailable
    {
        public static DataNotAvailable Instance { get; } = new DataNotAvailable();
        private DataNotAvailable() { }
    }
}
