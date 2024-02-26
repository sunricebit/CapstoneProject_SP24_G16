using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class PoolComContext:DbContext
    {
        public PoolComContext()
        {

        }
        public PoolComContext(DbContextOptions<PoolComContext> options) : base(options)
        {
        }
        public DbSet<Access> Accesses { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<ClubPost> ClubPosts { get; set; }
        public DbSet<GameType> GameTypes { get; set; }
        public DbSet<MatchOfTournament> MatchOfTournaments { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SoloMatch> SoloMatches { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentType> TournamentTypes { get; set; }
        public DbSet<TourPlayer> TourPlayers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("PoolCom"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Access>()
                .HasKey(a => a.AccessID);
            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountID);
            modelBuilder.Entity<Club>()
                .HasKey(c => c.ClubId);
            modelBuilder.Entity<ClubPost>()
                .HasKey(cp => cp.PostID);
            modelBuilder.Entity<GameType>()
                .HasKey(g => g.GameTypeID);
            modelBuilder.Entity<MatchOfTournament>()
                .HasKey(m => m.MatchNumber);
            modelBuilder.Entity<News>()
                .HasKey(m => m.NewsID);
            modelBuilder.Entity<Player>()
                .HasKey(m => m.PlayerID);
            modelBuilder.Entity<Role>()
                .HasKey(m => m.RoleID);
            modelBuilder.Entity<SoloMatch>()
                .HasKey(s => s.SoloMatchID);
            modelBuilder.Entity<Tournament>()
                .HasKey(t => t.TourID);
            modelBuilder.Entity<TournamentType>()
                .HasKey(t => t.TournamentTypeID);
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);
            modelBuilder.Entity<Tournament>()
            .HasOne(t => t.access)
            .WithMany(a => a.tournaments)
            .HasForeignKey(t => t.AccessID);
            modelBuilder.Entity<News>()
           .HasOne(n => n.Account)
           .WithMany(a => a.NewsList)
           .HasForeignKey(n => n.AccID);
            modelBuilder.Entity<ClubPost>()
            .HasOne(cp => cp.Club)
            .WithMany(c => c.ClubPost)
            .HasForeignKey(cp => cp.ClubID);
            modelBuilder.Entity<SoloMatch>()
            .HasOne(sm => sm.Type)
            .WithMany(gt => gt.Match)
            .HasForeignKey(sm => sm.GameTypeID);
            modelBuilder.Entity<Player>()
            .HasOne(p => p.User)
            .WithOne(u => u.Player)
            .HasForeignKey<Player>(p => p.AccountID)
            .IsRequired();
            modelBuilder.Entity<Role>()
            .HasMany(r => r.Account)
            .WithOne(a => a.Role)
            .HasForeignKey(a => a.RoleID);
            modelBuilder.Entity<SoloMatch>()
            .HasOne(sm => sm.Club)
            .WithMany(c => c.SoloMatches)
           .HasForeignKey(sm => sm.ClubID);
            modelBuilder.Entity<Tournament>()
            .HasMany(t => t.TourPlayer)
             .WithOne(tp => tp.Tournament)
        .HasForeignKey(tp => tp.TournamentID)
        .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
             .HasOne(u => u.Player)
             .WithOne(p => p.User)
             .HasForeignKey<User>(p => p.UserId)
             .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.type)
                .WithMany(t => t.Tournament)
                .HasForeignKey(t => t.GameTypeID);
            modelBuilder.Entity<Tournament>()
                .HasMany(t => t.MatchOfTournamentList)
                .WithOne(t => t.tournament)
                .HasForeignKey(t => t.TourID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.tournamentType)
                .WithMany(t => t.tournaments)
                .HasForeignKey(t => t.TourID);



            base.OnModelCreating(modelBuilder);
        }



    }
}
