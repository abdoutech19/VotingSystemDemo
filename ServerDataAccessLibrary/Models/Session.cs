using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Server.Models
{
    public class Session
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string SessionName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }
        [Required]
        [MaxLength(50)]
        public string TimeStamp { get; set; }

        [Required]
        [MaxLength(100)]
        public string PrivateKey { get; set; }

        public bool VotingStarted { get; set; } = false;

    }
}
