﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace BusinessObject.Models
{
    public partial class poolcomvnContext : DbContext
    {
        public poolcomvnContext()
        {
        }

        public poolcomvnContext(DbContextOptions<poolcomvnContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Access> Accesses { get; set; } = null!;
        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Club> Clubs { get; set; } = null!;
        public virtual DbSet<ClubPost> ClubPosts { get; set; } = null!;
        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<GameType> GameTypes { get; set; } = null!;
        public virtual DbSet<MatchOfTournament> MatchOfTournaments { get; set; } = null!;
        public virtual DbSet<News> News { get; set; } = null!;
        public virtual DbSet<Player> Players { get; set; } = null!;
        public virtual DbSet<PlayerInMatch> PlayerInMatches { get; set; } = null!;
        public virtual DbSet<PlayerInSoloMatch> PlayerInSoloMatches { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<SoloMatch> SoloMatches { get; set; } = null!;
        public virtual DbSet<Table> Tables { get; set; } = null!;
        public virtual DbSet<Tournament> Tournaments { get; set; } = null!;
        public virtual DbSet<TournamentType> TournamentTypes { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

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
            modelBuilder.Entity<Access>(entity =>
            {
                entity.Property(e => e.AccessId).HasColumnName("AccessID");
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.ClubId, "IX_Accounts_ClubId");

                entity.HasIndex(e => e.RoleId, "IX_Accounts_RoleID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.VerifyCode).HasColumnName("verifyCode");

                entity.HasOne(d => d.Club)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.ClubId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<Club>(entity =>
            {
                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Clubs)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Clubs_Accounts");
            });

            modelBuilder.Entity<ClubPost>(entity =>
            {
                entity.HasKey(e => e.PostId);

                entity.HasIndex(e => e.ClubId, "IX_ClubPosts_ClubID");

                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.Property(e => e.ClubId).HasColumnName("ClubID");

                entity.Property(e => e.Flyer).HasMaxLength(255);

                entity.Property(e => e.Link).HasMaxLength(255);

                entity.HasOne(d => d.Club)
                    .WithMany(p => p.ClubPosts)
                    .HasForeignKey(d => d.ClubId);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");

                entity.Property(e => e.CountryId)
                    .ValueGeneratedNever()
                    .HasColumnName("CountryID");

                entity.Property(e => e.CountryImage).HasMaxLength(255);

                entity.Property(e => e.CountryName).HasMaxLength(255);
            });

            modelBuilder.Entity<GameType>(entity =>
            {
                entity.Property(e => e.GameTypeId).HasColumnName("GameTypeID");
            });

            modelBuilder.Entity<MatchOfTournament>(entity =>
            {
                entity.HasKey(e => e.MatchId);

                entity.Property(e => e.MatchId).HasColumnName("MatchID");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.MatchCode).HasMaxLength(50);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.TourId).HasColumnName("TourID");

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.MatchOfTournaments)
                    .HasForeignKey(d => d.TourId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MatchOfTournaments_Tournaments");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.HasIndex(e => e.AccId, "IX_News_AccID");

                entity.Property(e => e.NewsId).HasColumnName("NewsID");

                entity.Property(e => e.AccId).HasColumnName("AccID");

                entity.Property(e => e.Link).HasMaxLength(255);

                entity.HasOne(d => d.Acc)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.AccId);
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.Property(e => e.PlayerId).HasColumnName("PlayerID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CountryId).HasColumnName("CountryID");

                entity.Property(e => e.TourId).HasColumnName("TourID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Players)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_Players_Country");

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.Players)
                    .HasForeignKey(d => d.TourId)
                    .HasConstraintName("FK_Players_Tournaments");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Players)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Players_Users");
            });

            modelBuilder.Entity<PlayerInMatch>(entity =>
            {
                entity.HasKey(e => e.PlayerMatchId);

                entity.ToTable("PlayerInMatch");

                entity.Property(e => e.PlayerMatchId).HasColumnName("PlayerMatchID");

                entity.Property(e => e.GameWin).HasMaxLength(50);

                entity.Property(e => e.MatchId).HasColumnName("MatchID");

                entity.Property(e => e.PlayerId).HasColumnName("PlayerID");

                entity.HasOne(d => d.Match)
                    .WithMany(p => p.PlayerInMatches)
                    .HasForeignKey(d => d.MatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlayerInMatchs_MatchOfTournaments");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerInMatches)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlayerInMatch_Players");
            });

            modelBuilder.Entity<PlayerInSoloMatch>(entity =>
            {
                entity.HasKey(e => e.PlayerSoloMatchId);

                entity.ToTable("PlayerInSoloMatch");

                entity.Property(e => e.PlayerSoloMatchId).HasColumnName("PlayerSoloMatchID");

                entity.Property(e => e.GameWin).HasMaxLength(50);

                entity.Property(e => e.PlayerId).HasColumnName("PlayerID");

                entity.Property(e => e.SoloMatchId).HasColumnName("SoloMatchID");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerInSoloMatches)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlayerInSoloMatchs_Players");

                entity.HasOne(d => d.SoloMatch)
                    .WithMany(p => p.PlayerInSoloMatches)
                    .HasForeignKey(d => d.SoloMatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlayerInSoloMatchs_SoloMatches");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnName("RoleID");
            });

            modelBuilder.Entity<SoloMatch>(entity =>
            {
                entity.HasIndex(e => e.ClubId, "IX_SoloMatches_ClubID");

                entity.HasIndex(e => e.GameTypeId, "IX_SoloMatches_GameTypeID");

                entity.Property(e => e.SoloMatchId).HasColumnName("SoloMatchID");

                entity.Property(e => e.ClubId).HasColumnName("ClubID");

                entity.Property(e => e.GameTypeId).HasColumnName("GameTypeID");

                entity.HasOne(d => d.Club)
                    .WithMany(p => p.SoloMatches)
                    .HasForeignKey(d => d.ClubId);

                entity.HasOne(d => d.GameType)
                    .WithMany(p => p.SoloMatches)
                    .HasForeignKey(d => d.GameTypeId);
            });

            modelBuilder.Entity<Table>(entity =>
            {
                entity.Property(e => e.ClubId).HasColumnName("ClubID");

                entity.HasOne(d => d.Club)
                    .WithMany(p => p.Tables)
                    .HasForeignKey(d => d.ClubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tables_Clubs");
            });

            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.HasKey(e => e.TourId);

                entity.HasIndex(e => e.AccessId, "IX_Tournaments_AccessID");

                entity.HasIndex(e => e.GameTypeId, "IX_Tournaments_GameTypeID");

                entity.Property(e => e.TourId)
                    .ValueGeneratedNever()
                    .HasColumnName("TourID");

                entity.Property(e => e.AccessId).HasColumnName("AccessID");

                entity.Property(e => e.ClubId).HasColumnName("ClubID");

                entity.Property(e => e.GameTypeId).HasColumnName("GameTypeID");

                entity.Property(e => e.Rules).HasMaxLength(255);

                entity.Property(e => e.TournamentTypeId).HasColumnName("TournamentTypeID");

                entity.HasOne(d => d.Access)
                    .WithMany(p => p.Tournaments)
                    .HasForeignKey(d => d.AccessId);

                entity.HasOne(d => d.Club)
                    .WithMany(p => p.Tournaments)
                    .HasForeignKey(d => d.ClubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tournaments_Clubs");

                entity.HasOne(d => d.GameType)
                    .WithMany(p => p.Tournaments)
                    .HasForeignKey(d => d.GameTypeId);

                entity.HasOne(d => d.Tour)
                    .WithOne(p => p.TournamentTour)
                    .HasForeignKey<Tournament>(d => d.TourId);

                entity.HasOne(d => d.TournamentType)
                    .WithMany(p => p.TournamentTournamentTypes)
                    .HasForeignKey(d => d.TournamentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tournaments_TournamentTypes");
            });

            modelBuilder.Entity<TournamentType>(entity =>
            {
                entity.Property(e => e.TournamentTypeId).HasColumnName("TournamentTypeID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.AccountId, "IX_Users_AccountID");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.AccountId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}