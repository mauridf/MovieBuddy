﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieBuddy.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MovieBuddy.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MovieBuddy.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MovieBuddy.Models.UserPreference", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("GenreId")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "GenreId");

                    b.ToTable("UserPreferences");
                });

            modelBuilder.Entity("MovieBuddy.Models.UserRating", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("MovieId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsTvShow")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("RatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("Rating")
                        .HasColumnType("double precision");

                    b.HasKey("UserId", "MovieId");

                    b.ToTable("UserRatings");
                });

            modelBuilder.Entity("MovieBuddy.Models.UserPreference", b =>
                {
                    b.HasOne("MovieBuddy.Models.User", "User")
                        .WithMany("Preferences")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieBuddy.Models.UserRating", b =>
                {
                    b.HasOne("MovieBuddy.Models.User", "User")
                        .WithMany("Ratings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieBuddy.Models.User", b =>
                {
                    b.Navigation("Preferences");

                    b.Navigation("Ratings");
                });
#pragma warning restore 612, 618
        }
    }
}
