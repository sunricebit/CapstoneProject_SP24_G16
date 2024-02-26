﻿// <auto-generated />
using System;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BusinessObject.Migrations
{
    [DbContext(typeof(PoolComContext))]
    partial class PoolComContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.26")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BusinessObject.Models.Access", b =>
                {
                    b.Property<int>("AccessID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AccessID"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AccessID");

                    b.ToTable("Accesses");
                });

            modelBuilder.Entity("BusinessObject.Models.Account", b =>
                {
                    b.Property<int>("AccountID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AccountID"), 1L, 1);

                    b.Property<int?>("ClubId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleID")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("verifyCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("veriyCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AccountID");

                    b.HasIndex("ClubId");

                    b.HasIndex("RoleID");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("BusinessObject.Models.Club", b =>
                {
                    b.Property<int>("ClubId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ClubId"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClubName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Facebook")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ClubId");

                    b.ToTable("Clubs");
                });

            modelBuilder.Entity("BusinessObject.Models.ClubPost", b =>
                {
                    b.Property<int>("PostID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PostID"), 1L, 1);

                    b.Property<int>("ClubID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("PostID");

                    b.HasIndex("ClubID");

                    b.ToTable("ClubPosts");
                });

            modelBuilder.Entity("BusinessObject.Models.GameType", b =>
                {
                    b.Property<int>("GameTypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GameTypeID"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GameTypeID");

                    b.ToTable("GameTypes");
                });

            modelBuilder.Entity("BusinessObject.Models.GameRule", b =>
                {
                    b.Property<int>("GameRuleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GameRuleID"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RuleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GameRuleID");

                    b.ToTable("GameRules");
                });

            modelBuilder.Entity("BusinessObject.Models.MatchOfTournament", b =>
                {
                    b.Property<int>("MatchNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MatchNumber"), 1L, 1);

                    b.Property<bool>("IsFinish")
                        .HasColumnType("bit");

                    b.Property<int>("Score1")
                        .HasColumnType("int");

                    b.Property<int>("Score2")
                        .HasColumnType("int");

                    b.Property<int>("TourID")
                        .HasColumnType("int");

                    b.Property<int>("player1")
                        .HasColumnType("int");

                    b.Property<int>("player2")
                        .HasColumnType("int");

                    b.HasKey("MatchNumber");

                    b.HasIndex("TourID");

                    b.ToTable("MatchOfTournaments");
                });

            modelBuilder.Entity("BusinessObject.Models.News", b =>
                {
                    b.Property<int>("NewsID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NewsID"), 1L, 1);

                    b.Property<int>("AccID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("link")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NewsID");

                    b.HasIndex("AccID");

                    b.ToTable("News");
                });

            modelBuilder.Entity("BusinessObject.Models.Player", b =>
                {
                    b.Property<int>("PlayerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PlayerID"), 1L, 1);

                    b.Property<int>("AccountID")
                        .HasColumnType("int");

                    b.Property<int>("AccountID1")
                        .HasColumnType("int");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MatchOfTournamentMatchNumber")
                        .HasColumnType("int");

                    b.Property<string>("PlayerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PlayerID");

                    b.HasIndex("AccountID1")
                        .IsUnique();

                    b.HasIndex("MatchOfTournamentMatchNumber");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("BusinessObject.Models.Role", b =>
                {
                    b.Property<int>("RoleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleID"), 1L, 1);

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleID");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("BusinessObject.Models.SoloMatch", b =>
                {
                    b.Property<int>("SoloMatchID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SoloMatchID"), 1L, 1);

                    b.Property<int>("ClubID")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GameTypeID")
                        .HasColumnType("int");

                    b.Property<int>("Player1")
                        .HasColumnType("int");

                    b.Property<int>("Player2")
                        .HasColumnType("int");

                    b.Property<int>("Score1")
                        .HasColumnType("int");

                    b.Property<int>("Score2")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("SoloMatchID");

                    b.HasIndex("ClubID");

                    b.HasIndex("GameTypeID");

                    b.ToTable("SoloMatches");
                });

            modelBuilder.Entity("BusinessObject.Models.Tournament", b =>
                {
                    b.Property<int>("TourID")
                        .HasColumnType("int");

                    b.Property<int>("AccessID")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DrawType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EntryFee")
                        .HasColumnType("int");

                    b.Property<int>("GameTypeID")
                        .HasColumnType("int");

                    b.Property<int>("MaxPlayerNumber")
                        .HasColumnType("int");

                    b.Property<string>("PaymentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlayerNumber")
                        .HasColumnType("int");

                    b.Property<string>("RaceToString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RegistrationDeadline")
                        .HasColumnType("datetime2");

                    b.Property<string>("Rule")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TotalPrice")
                        .HasColumnType("int");

                    b.Property<string>("TourName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TournamentTypeID")
                        .HasColumnType("int");

                    b.HasKey("TourID");

                    b.HasIndex("AccessID");

                    b.HasIndex("GameTypeID");

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("BusinessObject.Models.TournamentType", b =>
                {
                    b.Property<int>("TournamentTypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TournamentTypeID"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TournamentTypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TournamentTypeID");

                    b.ToTable("TournamentTypes");
                });

            modelBuilder.Entity("BusinessObject.Models.TourPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("PlayerID")
                        .HasColumnType("int");

                    b.Property<int>("TournamentID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlayerID")
                        .IsUnique();

                    b.HasIndex("TournamentID");

                    b.ToTable("TourPlayers");
                });

            modelBuilder.Entity("BusinessObject.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("AccountID")
                        .HasColumnType("int");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DOB")
                        .HasColumnType("datetime2");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId");

                    b.HasIndex("AccountID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BusinessObject.Models.Account", b =>
                {
                    b.HasOne("BusinessObject.Models.Club", "Club")
                        .WithMany()
                        .HasForeignKey("ClubId");

                    b.HasOne("BusinessObject.Models.Role", "Role")
                        .WithMany("Account")
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Club");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BusinessObject.Models.ClubPost", b =>
                {
                    b.HasOne("BusinessObject.Models.Club", "Club")
                        .WithMany("ClubPost")
                        .HasForeignKey("ClubID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Club");
                });

            modelBuilder.Entity("BusinessObject.Models.MatchOfTournament", b =>
                {
                    b.HasOne("BusinessObject.Models.Tournament", "tournament")
                        .WithMany("MatchOfTournamentList")
                        .HasForeignKey("TourID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("tournament");
                });

            modelBuilder.Entity("BusinessObject.Models.News", b =>
                {
                    b.HasOne("BusinessObject.Models.Account", "Account")
                        .WithMany("NewsList")
                        .HasForeignKey("AccID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("BusinessObject.Models.Player", b =>
                {
                    b.HasOne("BusinessObject.Models.Account", "Account")
                        .WithOne("Player")
                        .HasForeignKey("BusinessObject.Models.Player", "AccountID1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.MatchOfTournament", "MatchOfTournament")
                        .WithMany("playerList")
                        .HasForeignKey("MatchOfTournamentMatchNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("MatchOfTournament");
                });

            modelBuilder.Entity("BusinessObject.Models.SoloMatch", b =>
                {
                    b.HasOne("BusinessObject.Models.Club", "Club")
                        .WithMany("SoloMatches")
                        .HasForeignKey("ClubID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.GameType", "Type")
                        .WithMany("Match")
                        .HasForeignKey("GameTypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Club");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("BusinessObject.Models.Tournament", b =>
                {
                    b.HasOne("BusinessObject.Models.Access", "access")
                        .WithMany("tournaments")
                        .HasForeignKey("AccessID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.GameType", "type")
                        .WithMany("Tournament")
                        .HasForeignKey("GameTypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.TournamentType", "tournamentType")
                        .WithMany("tournaments")
                        .HasForeignKey("TourID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("access");

                    b.Navigation("tournamentType");

                    b.Navigation("type");
                });

            modelBuilder.Entity("BusinessObject.Models.TourPlayer", b =>
                {
                    b.HasOne("BusinessObject.Models.Player", "Player")
                        .WithOne("TourPlayer")
                        .HasForeignKey("BusinessObject.Models.TourPlayer", "PlayerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Tournament", "Tournament")
                        .WithMany("TourPlayer")
                        .HasForeignKey("TournamentID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("Tournament");
                });

            modelBuilder.Entity("BusinessObject.Models.User", b =>
                {
                    b.HasOne("BusinessObject.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Player", "Player")
                        .WithOne("User")
                        .HasForeignKey("BusinessObject.Models.User", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("BusinessObject.Models.Access", b =>
                {
                    b.Navigation("tournaments");
                });

            modelBuilder.Entity("BusinessObject.Models.Account", b =>
                {
                    b.Navigation("NewsList");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("BusinessObject.Models.Club", b =>
                {
                    b.Navigation("ClubPost");

                    b.Navigation("SoloMatches");
                });

            modelBuilder.Entity("BusinessObject.Models.GameType", b =>
                {
                    b.Navigation("Match");

                    b.Navigation("Tournament");
                });

            modelBuilder.Entity("BusinessObject.Models.MatchOfTournament", b =>
                {
                    b.Navigation("playerList");
                });

            modelBuilder.Entity("BusinessObject.Models.Player", b =>
                {
                    b.Navigation("TourPlayer")
                        .IsRequired();

                    b.Navigation("User")
                        .IsRequired();
                });

            modelBuilder.Entity("BusinessObject.Models.Role", b =>
                {
                    b.Navigation("Account");
                });

            modelBuilder.Entity("BusinessObject.Models.Tournament", b =>
                {
                    b.Navigation("MatchOfTournamentList");

                    b.Navigation("TourPlayer");
                });

            modelBuilder.Entity("BusinessObject.Models.TournamentType", b =>
                {
                    b.Navigation("tournaments");
                });
#pragma warning restore 612, 618
        }
    }
}
