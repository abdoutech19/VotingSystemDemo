using Akka.Actor;
using Akka.Event;
using NSec.Cryptography;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class AccountVerificationManager : UntypedActor
    {
        protected ILoggingAdapter Logging { get; } = Context.GetLogger();
        public Session CurrentSession { get; }
        private Dictionary<int,IActorRef> _uidToActor = new Dictionary<int,IActorRef>();
        private Dictionary<IActorRef, int> _actorToUid = new Dictionary<IActorRef, int>();

        public AccountVerificationManager(Session currentSession)
        {
            CurrentSession = currentSession;
        }
        protected override void PreStart()
        {
            Logging.Info("Account Verification Service Started...");
        }
        protected override void OnReceive(object message)
        {   
            switch (message)
            {
                case AccountVerificationRequest request:

                    if (_uidToActor.ContainsKey(request.UserId))
                    {
                        var verificationActor = _uidToActor[request.UserId];
                        verificationActor.Forward(request);
                    }
                    else
                    {
                        Sender.Tell(NotAllowedToVerify.Instance);
                    }
                    break;
                case AllowToApprove allowTo:

                    if (CorrectSessionCredentials(allowTo))
                    {
                        if (!_uidToActor.ContainsKey(allowTo.UserId))
                        {
                            var verificationActor = Context.ActorOf(AccountVerifier.Props());
                            Context.Watch(verificationActor);
                            _uidToActor.Add(allowTo.UserId, verificationActor);
                            _actorToUid.Add(verificationActor, allowTo.UserId);
                        }
                    }
                    else
                    {
                        Sender.Tell(InvalidCredentials.Instance);
                    }
                    break;
                case FinishedQuery finished:

                    Context.Unwatch(finished.ActorRef);
                    var uid = _actorToUid[finished.ActorRef];
                    _uidToActor.Remove(uid);
                    _actorToUid.Remove(finished.ActorRef);
                    break;
                case Terminated terminated:

                    var userId = _actorToUid[terminated.ActorRef];
                    _uidToActor.Remove(userId);
                    _actorToUid.Remove(terminated.ActorRef);
                    Sender.Tell(InternalErrorOccurred.Instance);
                    break;
                default:
                    break;
            }
        }
        public static Props Props(Session currentSession) =>
            Akka.Actor.Props.Create(() => new AccountVerificationManager(currentSession));
        private bool CorrectSessionCredentials(AllowToApprove request)
        {
            byte[] passwordHash = HashAlgorithm.Sha256.Hash(Encoding.UTF8.GetBytes(request.Password));
            string password = Convert.ToBase64String(passwordHash);
            string sessionName = request.SessionName;
            return CurrentSession.SessionName.Equals(sessionName) && CurrentSession.Password.Equals(password);
        }
    }
    public sealed class NotAllowedToVerify
    {
        public static NotAllowedToVerify Instance { get;} = new NotAllowedToVerify();

        private NotAllowedToVerify() { }
    }
    public sealed class AllowToApprove
    {
        public int UserId { get; }
        public string SessionName { get; }
        public string Password { get; }

        public AllowToApprove(int userId, string sessionName, string password)
        {
            UserId = userId;
            SessionName = sessionName;
            Password = password;
        }
    }
}
