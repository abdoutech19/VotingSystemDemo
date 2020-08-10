using Akka.Actor;
using Akka.Event;
using Microsoft.EntityFrameworkCore;
using NSec.Cryptography;
using Org.BouncyCastle.Asn1.Ocsp;
using Server.DataAccess;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class StateManager : UntypedActor
    {
        protected ILoggingAdapter Logging { get; } = Context.GetLogger();
        protected State MyState { get; }
        protected string SessionPrivateKey { get; }
        protected List<Candidate> Candidates { get; }
        private VotingContext _dbContext = new VotingContext();
        private readonly List<string> _consensusNodesAddresses;
        private Dictionary<IActorRef, string> _senderToRequesId = new Dictionary<IActorRef, string>();
        public StateManager(State state, List<Candidate> candidates, string sessionPrivateKey)
        {
            MyState = state;
            Candidates = candidates;
            SessionPrivateKey = sessionPrivateKey;

            TryGetAddresses(MyState.ZipCode, out _consensusNodesAddresses);
        }

        protected override void PreStart()
        {
            Logging.Info($"Actor responsible for state {MyState.StateName} has started...");
        }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case VotingDataRequest request when request.StateZipCode == MyState.ZipCode:

                    var senderActor = Context.ActorOf(VotingDataSender.Props(SessionPrivateKey, request.SessionSignature, Candidates, _consensusNodesAddresses));
                    Context.Watch(senderActor);
                    senderActor.Forward(request);
                    _senderToRequesId.Add(senderActor, request.RequestId);
                    break;
                case VotingDataRequest request:

                    Context.Parent.Tell(new NotMyState(request.RequestId,request.StateZipCode));
                    break;
                case FinishedQuery finished:

                    Context.Unwatch(finished.ActorRef);
                    _senderToRequesId.Remove(finished.ActorRef);
                    break;
                case Terminated terminated:

                    if(_senderToRequesId.TryGetValue(terminated.ActorRef, out string requestId))
                    {
                        Sender.Tell(new InternalErrorOccurred(requestId));
                        _senderToRequesId.Remove(terminated.ActorRef);
                    }
                    break;
                default:
                    break;
            }
        }
        public static Props Props(State myState, List<Candidate> candidates, string sessionPrivateKey) =>
            Akka.Actor.Props.Create(() => new StateManager(myState, candidates, sessionPrivateKey));

        //Tries to Get consensus nodes addresses
        private bool TryGetAddresses(string stateZipCode, out List<string> addresses)
        {
            try
            {
                addresses = _dbContext.ConsensusNodes
                    .Where( _ => _.ConsensusNodeSate.ZipCode == stateZipCode)
                    .Select( _ => _.Address)
                    .ToList();
                return true;
            }
            catch (Exception e)
            {
                Logging.Error(e, $"{MyState.StateName}-Actor could not start. Failed Connection To Database");
                throw;
            }
        }
    }
     public sealed class NotMyState 
    {
        public string RequestId { get; }
        public string StateZipCode { get; }

        public NotMyState(string requestId, string stateZipCode)
        {
            RequestId = requestId;
            StateZipCode = stateZipCode;
        }
    }
}
