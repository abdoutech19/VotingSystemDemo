using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Server.Models
{
    public class ConsensusNode
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        public State ConsensusNodeSate { get; set; }

        [Required]
        [MaxLength(100)]
        public string PublicKey { get; set; }

        [Required]
        public Session VotingSession { get; set; }
    }
}
