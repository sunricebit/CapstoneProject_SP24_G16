﻿// <auto-generated />
using System;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BusinessObject.Migrations
{
    [DbContext(typeof(PoolComContext))]
    [Migration("20240129151648_DB2")]
    partial class DB2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.26")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BusinessObject.Models.Account", b =>
                {
                    b.Property<int>("AccountID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AccountID"), 1L, 1);

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ClubId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleID")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("AccountID");

                    b.HasIndex("ClubId");

                    b.HasIndex("RoleID");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("BusinessObject.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"), 1L, 1);

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoryId");

                    b.ToTable("Category");
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
                        .IsRequired()
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

                    b.ToTable("Club");
                });

            modelBuilder.Entity("BusinessObject.Models.ClubPost", b =>
                {
                    b.Property<int>("PostID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PostID"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ClubID")
                        .HasColumnType("int");

                    b.Property<string>("ClubName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Facebook")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PostID");

                    b.HasIndex("ClubID");

                    b.ToTable("ClubPost");
                });

            modelBuilder.Entity("BusinessObject.Models.MatchOfTournament", b =>
                {
                    b.Property<int>("TourMatchID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TourMatchID"), 1L, 1);

                    b.Property<int>("MatchNumber")
                        .HasColumnType("int");

                    b.Property<int>("PlayerID")
                        .HasColumnType("int");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<int>("TourID")
                        .HasColumnType("int");

                    b.HasKey("TourMatchID");

                    b.HasIndex("PlayerID")
                        .IsUnique();

                    b.HasIndex("TourID");

                    b.ToTable("MatchOfTournament");
                });

            modelBuilder.Entity("BusinessObject.Models.News", b =>
                {
                    b.Property<int>("NewsID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NewsID"), 1L, 1);

                    b.Property<int>("AccountID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("NewsID");

                    b.HasIndex("AccountID");

                    b.ToTable("News");
                });

            modelBuilder.Entity("BusinessObject.Models.Order", b =>
                {
                    b.Property<int>("OrderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderID"), 1L, 1);

                    b.Property<int>("AccountID")
                        .HasColumnType("int");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("RequiredDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ShippedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("OrderID");

                    b.HasIndex("AccountID");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("BusinessObject.Models.OrderDetails", b =>
                {
                    b.Property<int>("OrderID")
                        .HasColumnType("int");

                    b.Property<int>("ProductID")
                        .HasColumnType("int");

                    b.Property<decimal>("Discount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("OrderID", "ProductID");

                    b.HasIndex("ProductID")
                        .IsUnique();

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("BusinessObject.Models.Player", b =>
                {
                    b.Property<int>("PlayerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PlayerID"), 1L, 1);

                    b.Property<int>("AccountID")
                        .HasColumnType("int");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PlayerID");

                    b.HasIndex("AccountID")
                        .IsUnique();

                    b.ToTable("Players");
                });

            modelBuilder.Entity("BusinessObject.Models.Product", b =>
                {
                    b.Property<int>("ProductID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductID"), 1L, 1);

                    b.Property<int>("CategoryID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("UnitOfStock")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ProductID");

                    b.HasIndex("CategoryID");

                    b.ToTable("Products");
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

            modelBuilder.Entity("BusinessObject.Models.Scale", b =>
                {
                    b.Property<int>("ScaleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ScaleID"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ScaleID");

                    b.ToTable("Scales");
                });

            modelBuilder.Entity("BusinessObject.Models.SoloMatch", b =>
                {
                    b.Property<int>("SoloMatchID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SoloMatchID"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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

                    b.Property<int>("TypeID")
                        .HasColumnType("int");

                    b.HasKey("SoloMatchID");

                    b.HasIndex("Player1");

                    b.HasIndex("TypeID");

                    b.ToTable("SoloMatches");
                });

            modelBuilder.Entity("BusinessObject.Models.Tournament", b =>
                {
                    b.Property<int>("TournamentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TournamentID"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GameRuleID")
                        .HasColumnType("int");

                    b.Property<int>("MaxPlayer")
                        .HasColumnType("int");

                    b.Property<int>("MinPlayer")
                        .HasColumnType("int");

                    b.Property<int>("ScaleID")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TourName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TypeID")
                        .HasColumnType("int");

                    b.HasKey("TournamentID");

                    b.HasIndex("ScaleID");

                    b.HasIndex("TypeID");

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("BusinessObject.Models.TourPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("PlayerID")
                        .HasColumnType("int");

                    b.Property<int>("TourID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlayerID");

                    b.HasIndex("TourID");

                    b.ToTable("Tours");
                });

            modelBuilder.Entity("BusinessObject.Models.Type", b =>
                {
                    b.Property<int>("TypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TypeID"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TypeID");

                    b.ToTable("Type");
                });

            modelBuilder.Entity("BusinessObject.Models.Account", b =>
                {
                    b.HasOne("BusinessObject.Models.Club", "Club")
                        .WithMany()
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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
                    b.HasOne("BusinessObject.Models.Player", "player")
                        .WithOne("MatchOfTournament")
                        .HasForeignKey("BusinessObject.Models.MatchOfTournament", "PlayerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Tournament", "tournament")
                        .WithMany("MatchOfTournamentList")
                        .HasForeignKey("TourID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("player");

                    b.Navigation("tournament");
                });

            modelBuilder.Entity("BusinessObject.Models.News", b =>
                {
                    b.HasOne("BusinessObject.Models.Account", "Account")
                        .WithMany("NewsList")
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("BusinessObject.Models.Order", b =>
                {
                    b.HasOne("BusinessObject.Models.Account", "Account")
                        .WithMany("OrderList")
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("BusinessObject.Models.OrderDetails", b =>
                {
                    b.HasOne("BusinessObject.Models.Order", "Order")
                        .WithMany("Details")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Product", "Products")
                        .WithOne("OrderDetails")
                        .HasForeignKey("BusinessObject.Models.OrderDetails", "ProductID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("BusinessObject.Models.Player", b =>
                {
                    b.HasOne("BusinessObject.Models.Account", "Account")
                        .WithOne("Player")
                        .HasForeignKey("BusinessObject.Models.Player", "AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("BusinessObject.Models.Product", b =>
                {
                    b.HasOne("BusinessObject.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("BusinessObject.Models.SoloMatch", b =>
                {
                    b.HasOne("BusinessObject.Models.Player", "Players")
                        .WithMany("soloMatches")
                        .HasForeignKey("Player1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Type", "Type")
                        .WithMany("Match")
                        .HasForeignKey("TypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Players");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("BusinessObject.Models.Tournament", b =>
                {
                    b.HasOne("BusinessObject.Models.Scale", "scale")
                        .WithMany("Tournament")
                        .HasForeignKey("ScaleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Type", "type")
                        .WithMany("Tournament")
                        .HasForeignKey("TypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("scale");

                    b.Navigation("type");
                });

            modelBuilder.Entity("BusinessObject.Models.TourPlayer", b =>
                {
                    b.HasOne("BusinessObject.Models.Player", "Player")
                        .WithMany("TourPlayer")
                        .HasForeignKey("PlayerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Tournament", "Tournament")
                        .WithMany("tourPlayer")
                        .HasForeignKey("TourID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("Tournament");
                });

            modelBuilder.Entity("BusinessObject.Models.Account", b =>
                {
                    b.Navigation("NewsList");

                    b.Navigation("OrderList");

                    b.Navigation("Player")
                        .IsRequired();
                });

            modelBuilder.Entity("BusinessObject.Models.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("BusinessObject.Models.Club", b =>
                {
                    b.Navigation("ClubPost");
                });

            modelBuilder.Entity("BusinessObject.Models.Order", b =>
                {
                    b.Navigation("Details");
                });

            modelBuilder.Entity("BusinessObject.Models.Player", b =>
                {
                    b.Navigation("MatchOfTournament")
                        .IsRequired();

                    b.Navigation("TourPlayer");

                    b.Navigation("soloMatches");
                });

            modelBuilder.Entity("BusinessObject.Models.Product", b =>
                {
                    b.Navigation("OrderDetails")
                        .IsRequired();
                });

            modelBuilder.Entity("BusinessObject.Models.Role", b =>
                {
                    b.Navigation("Account");
                });

            modelBuilder.Entity("BusinessObject.Models.Scale", b =>
                {
                    b.Navigation("Tournament");
                });

            modelBuilder.Entity("BusinessObject.Models.Tournament", b =>
                {
                    b.Navigation("MatchOfTournamentList");

                    b.Navigation("tourPlayer");
                });

            modelBuilder.Entity("BusinessObject.Models.Type", b =>
                {
                    b.Navigation("Match");

                    b.Navigation("Tournament");
                });
#pragma warning restore 612, 618
        }
    }
}