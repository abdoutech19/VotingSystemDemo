using Microsoft.EntityFrameworkCore;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataAccess
{
    public class VotingContext : DbContext
    {
        public DbSet<Voter> Voters { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<ConsensusNode> ConsensusNodes { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Session> Sessions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseMySQL("server=localhost;database=EFVotingDemo;user=root;password=root");
    }
}
