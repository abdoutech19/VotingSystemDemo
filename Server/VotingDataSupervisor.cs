using Akka.Actor;
using Akka.Event;
using Server.DataAccess;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class VotingDataSupervisor : UntypedActor
    {
        protected ILoggingAdapter Logging { get; } = Context.GetLogger();
        protected string SessionPrivateKey { get; }
        private VotingContext _dbContext = new VotingContext();
        private readonly List<Candidate> _candidates;
        private readonly List<State> _states;
        private Dictionary<State, IActorRef> _stateToActor = new Dictionary<State, IActorRef>();
        private Dictionary<IActorRef, State> _actorToState = new Dictionary<IActorRef, State>();

        public VotingDataSupervisor(string sessionPrivateKey)
        {
            SessionPrivateKey = sessionPrivateKey;
            _candidates = TryGetCandidates();
            _states = TryGetStates();
        }
        protected override void PreStart()
        {
            foreach (var state in _states)
            {
                var sendingManager = Context.ActorOf(StateManager.Props(state, _candidates, SessionPrivateKey));
                Context.Watch(sendingManager);
                _stateToActor.Add(state, sendingManager);
                _actorToState.Add(sendingManager, state);
            }
            Logging.Info("Voting data available...");
        }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case VotingDataRequest request:

                    State state = _stateToActor.Keys.Where(_ => _.ZipCode.Equals(request.StateZipCode)).FirstOrDefault();
                    var stateManager = _stateToActor[state];
                    stateManager.Forward(request);
                    break;
                case Terminated t:

                    var carshedState = _actorToState[t.ActorRef];
                    Logging.Warning($"Actor of State {carshedState.StateName} has crashed");
                    _stateToActor.Remove(carshedState);
                    _actorToState.Remove(t.ActorRef);
                    //Handle the crash of a state actor                    
                    break;
            }
        }
        public static Props Props(string sessionPrivateKey) =>
            Akka.Actor.Props.Create(() => new VotingDataSupervisor(sessionPrivateKey));
        private List<Candidate> TryGetCandidates()
        {
            try
            {
                var candidates = _dbContext.Candidates.ToList();
                if (candidates != null)
                {
                    return candidates;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            catch (Exception e)
            {
                Logging.Error(e, "Error while trying to fetch candidates list");
                throw;
            }
        }
        private List<State> TryGetStates()
        {
            try
            {
                var states = _dbContext.States.ToList();
                if (states != null)
                {
                    return states;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            catch (Exception e)
            {
                Logging.Error(e, "Error while trying to fetch states list");
                throw;
            }
        }
    }
}
