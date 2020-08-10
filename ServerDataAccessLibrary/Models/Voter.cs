using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Server.Models
{
    public class Voter
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public string PublicKey { get; set; }

        [Required]
        public State VoterState { get; set; }

        [Required]
        public bool Verified { get; set; } = false;

        [Required]
        public Session VotingSession { get; set; }
    }
}
