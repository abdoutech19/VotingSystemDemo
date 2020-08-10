using Akka.Actor;
using Akka.Event;
using Server.DataAccess;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    //Actor responsible for creating and managing registration query child actors 
    class RegistrationManager : UntypedActor
    {
        protected ILoggingAdapter Logging { get; } = Context.GetLogger();
        protected int SessionId { get; }
        private Dictionary<IActorRef, string> _queryActorToRequestId = new Dictionary<IActorRef, string>();
        
        public RegistrationManager(int sessionId)
        {
            SessionId = sessionId;
        }
        protected override void PreStart()
        {
            Logging.Info("Registration Service Started...");
        }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RegistrationRequest request:

                    var addUserQuery = Context.ActorOf(RegistrationQueryActor.Props(SessionId));
                    Context.Watch(addUserQuery);
                    _queryActorToRequestId.Add(addUserQuery, request.RequestId);
                    addUserQuery.Forward(request);
                    break;
                case FinishedQuery finished:

                    Context.Unwatch(finished.ActorRef);
                    _queryActorToRequestId.Remove(finished.ActorRef);
                    break;
                case Terminated terminated:

                    var actor = terminated.ActorRef;
                    Sender.Tell(new InternalErrorOccurred(_queryActorToRequestId.GetValueOrDefault(actor)));
                    _queryActorToRequestId.Remove(actor);
                    break;
                default:

                    Logging.Warning("Unkown Message Format");
                    break;
            }
        }

        public static Props Props(int sessionId) =>
           Akka.Actor.Props.Create(() => new RegistrationManager(sessionId));
    }
}
