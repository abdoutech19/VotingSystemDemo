using Akka.Actor;
using Akka.Event;
using Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Models
{
    class AuthenticationManager : UntypedActor
    {
        protected ILoggingAdapter Logging { get; } = Context.GetLogger();
        protected Session CurrentSession { get; }
        private Dictionary<IActorRef, string> _queryActorToRequestId = new Dictionary<IActorRef, string>();
        
        public AuthenticationManager(Session currentSession)
        {
            CurrentSession = currentSession;
        }
        protected override void PreStart()
        {
            Logging.Info("Authentication Service Satrted...");
        }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case AuthenticationRequest request:
                    var authenticationQuery = Context.ActorOf(AuthenticationQueryActor.Props(CurrentSession.PrivateKey));
                    Context.Watch(authenticationQuery);
                    _queryActorToRequestId.Add(authenticationQuery, request.RequestId);
                    authenticationQuery.Forward(request);
                    break;
                case FinishedQuery finished:
                    Context.Unwatch(finished.ActorRef);
                    _queryActorToRequestId.Remove(finished.ActorRef);
                    break;
                case Terminated terminated:
                    var actor = terminated.ActorRef;
                    _queryActorToRequestId.Remove(actor);
                    Sender.Tell(new InternalErrorOccurred(_queryActorToRequestId.GetValueOrDefault(actor)));
                    break;
                default:
                    Logging.Warning("Unkown Message Format");
                    break;
            }
        }

        public static Props Props(Session currentSession) =>
            Akka.Actor.Props.Create(() => new AuthenticationManager(currentSession));

    }
}
