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
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Product>  Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Scale> Scales { get; set; }
        public DbSet<Tournament> Tournaments { get; set;}
        public DbSet<TourPlayer> Tours { get; set; }
        public DbSet<SoloMatch> SoloMatches { get; set; }
        public DbSet<GameRule> GameRules { get; set; }
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
            
            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountID);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Account)
                .HasForeignKey(a => a.RoleID);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Player)
                .WithOne(p => p.Account)
                .HasForeignKey<Player>(p => p.AccountID);

           
            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryId);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryID);

         
            modelBuilder.Entity<Club>()
                .HasKey(c => c.ClubId);

          

          
            modelBuilder.Entity<MatchOfTournament>()
                .HasKey(m => m.TourMatchID);

            modelBuilder.Entity<MatchOfTournament>()
                .HasOne(m => m.tournament)
                .WithMany(t => t.MatchOfTournamentList)
                .HasForeignKey(m => m.TourID);

            modelBuilder.Entity<MatchOfTournament>()
                .HasOne(m => m.player)
                .WithOne(p => p.MatchOfTournament)
                .HasForeignKey<MatchOfTournament>(m => m.PlayerID);

          
            modelBuilder.Entity<News>()
                .HasKey(n => n.NewsID);

            modelBuilder.Entity<News>()
                .HasOne(n => n.Account)
                .WithMany(a => a.NewsList)
                .HasForeignKey(n => n.AccountID);

          
            modelBuilder.Entity<Order>()
                .HasKey(o => o.OrderID);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Account)
                .WithMany(a => a.OrderList)
                .HasForeignKey(o => o.AccountID);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Details)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderID);

            
            modelBuilder.Entity<OrderDetails>()
                .HasKey(od => new { od.OrderID, od.ProductID });

            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Order)
                .WithMany(o => o.Details)
                .HasForeignKey(od => od.OrderID);

         

           
            modelBuilder.Entity<Player>()
                .HasKey(p => p.PlayerID);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.Account)
                .WithOne(a => a.Player)
                .HasForeignKey<Player>(p => p.AccountID);

           
            modelBuilder.Entity<Player>()
                .HasMany(p => p.soloMatches)
                .WithOne(sm => sm.Players)
                .HasForeignKey(sm => sm.Player1);

           

           
            modelBuilder.Entity<Product>()
                .HasKey(p => p.ProductID);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.OrderDetails)
                .WithOne(od => od.Products)
                .HasForeignKey<OrderDetails>(od => od.ProductID);


            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleID);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Account)
                .WithOne(a => a.Role)
                .HasForeignKey(a => a.RoleID);

        
            modelBuilder.Entity<Scale>()
                .HasKey(s => s.ScaleID);

            modelBuilder.Entity<Scale>()
                .HasMany(s => s.Tournament)
                .WithOne(t => t.scale)
                .HasForeignKey(t => t.ScaleID);

            
            modelBuilder.Entity<SoloMatch>()
                .HasKey(sm => sm.SoloMatchID);

            modelBuilder.Entity<SoloMatch>()
                .HasOne(sm => sm.Players)
                .WithMany(p => p.soloMatches)
                .HasForeignKey(sm => sm.Player1);

          

            modelBuilder.Entity<SoloMatch>()
                .HasOne(sm => sm.Type)
                .WithMany(t => t.Match)
                .HasForeignKey(sm => sm.TypeID);

            
            modelBuilder.Entity<Tournament>()
                .HasKey(t => t.TournamentID);

            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.scale)
                .WithMany(s => s.Tournament)
                .HasForeignKey(t => t.ScaleID);

           

            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.type)
                .WithMany(type => type.Tournament)
                .HasForeignKey(t => t.TypeID);

          
            modelBuilder.Entity<TourPlayer>()
                .HasKey(tp => tp.Id);

            modelBuilder.Entity<TourPlayer>()
                .HasOne(tp => tp.Tournament)
                .WithMany(t => t.tourPlayer)
                .HasForeignKey(tp => tp.TourID);

            modelBuilder.Entity<TourPlayer>()
                .HasOne(tp => tp.Player)
                .WithMany(p => p.TourPlayer)
                .HasForeignKey(tp => tp.PlayerID);

            
            modelBuilder.Entity<Type>()
                .HasKey(t => t.TypeID);

            modelBuilder.Entity<Type>()
                .HasMany(t => t.Match)
                .WithOne(sm => sm.Type)
                .HasForeignKey(sm => sm.TypeID);

            modelBuilder.Entity<Type>()
                .HasMany(t => t.Tournament)
                .WithOne(t => t.type)
                .HasForeignKey(t => t.TypeID);

           

            base.OnModelCreating(modelBuilder);
        }



    }
}
