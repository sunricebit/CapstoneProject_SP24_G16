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
        public PoolComContext(DbContextOptions<PoolComContext> options) : base(options) { }
        public DbSet<Account> Account { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Product>  Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Scale> Scales { get; set; }
        public DbSet<Tournament> Tournaments { get; set;}
        public DbSet<TourPlayer> Tours { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("PoolCom"));
        }


    }
}
