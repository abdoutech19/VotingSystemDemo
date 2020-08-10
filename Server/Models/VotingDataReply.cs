using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public sealed class VotingDataReply
    {
        public string RequestId { get; }
        public Candidate[] Candidates { get; }
        public string[] ConsensusNodesAddresses { get; }

        public VotingDataReply(string requestId, Candidate[] candidates, string[] consensusNodesAddresses)
        {
            RequestId = requestId;
            Candidates = candidates;
            ConsensusNodesAddresses = consensusNodesAddresses;
        }
    }
}
