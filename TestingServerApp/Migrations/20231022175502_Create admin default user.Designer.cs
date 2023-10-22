﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestingServerApp.DbEntities;

#nullable disable

namespace TestingServerApp.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20231022175502_Create admin default user")]
    partial class Createadmindefaultuser
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.12");

            modelBuilder.Entity("TestingServerApp.Answer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Image")
                        .HasMaxLength(1024)
                        .HasColumnType("BLOB");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("INTEGER");

                    b.Property<int>("QuestionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("TestingServerApp.DetailedResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AnswerId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAnsweredByUser")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ShortResultId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AnswerId");

                    b.HasIndex("ShortResultId");

                    b.ToTable("DetailedResults");
                });

            modelBuilder.Entity("TestingServerApp.IssuedTest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AttemptsAmount")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ExpiredDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("IssueDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("TestId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserGroupId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.HasIndex("UserGroupId");

                    b.ToTable("IssuedTests");
                });

            modelBuilder.Entity("TestingServerApp.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Image")
                        .HasMaxLength(1024)
                        .HasColumnType("BLOB");

                    b.Property<bool>("MultipleAnswersAllowed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("PartialAnswersAllowed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("QuestionWeight")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TestId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("TestingServerApp.ShortResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("TestId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TestMaxScores")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("UserScores")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.HasIndex("UserId");

                    b.ToTable("ShortResults");
                });

            modelBuilder.Entity("TestingServerApp.Test", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Image")
                        .HasMaxLength(1024)
                        .HasColumnType("BLOB");

                    b.Property<int>("MaxTestScores")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinutsForTest")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("QuestionsAmountForTest")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TestCategoryId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TestCategoryId");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("TestingServerApp.TestCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestCategories");
                });

            modelBuilder.Entity("TestingServerApp.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("BLOB");

                    b.Property<int>("UserGroupId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserGroupId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TestingServerApp.UserGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("UserGroups");
                });

            modelBuilder.Entity("TestingServerApp.Answer", b =>
                {
                    b.HasOne("TestingServerApp.Question", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("TestingServerApp.DetailedResult", b =>
                {
                    b.HasOne("TestingServerApp.Answer", "Answer")
                        .WithMany()
                        .HasForeignKey("AnswerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TestingServerApp.ShortResult", "ShortResult")
                        .WithMany("DetailedResults")
                        .HasForeignKey("ShortResultId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Answer");

                    b.Navigation("ShortResult");
                });

            modelBuilder.Entity("TestingServerApp.IssuedTest", b =>
                {
                    b.HasOne("TestingServerApp.Test", "Test")
                        .WithMany("IssuedTests")
                        .HasForeignKey("TestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TestingServerApp.UserGroup", "UserGroup")
                        .WithMany("IssuedTests")
                        .HasForeignKey("UserGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Test");

                    b.Navigation("UserGroup");
                });

            modelBuilder.Entity("TestingServerApp.Question", b =>
                {
                    b.HasOne("TestingServerApp.Test", "Test")
                        .WithMany("Questions")
                        .HasForeignKey("TestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Test");
                });

            modelBuilder.Entity("TestingServerApp.ShortResult", b =>
                {
                    b.HasOne("TestingServerApp.Test", "Test")
                        .WithMany("ShortResult")
                        .HasForeignKey("TestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TestingServerApp.User", "User")
                        .WithMany("ShortResults")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Test");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TestingServerApp.Test", b =>
                {
                    b.HasOne("TestingServerApp.TestCategory", "TestCategory")
                        .WithMany("Tests")
                        .HasForeignKey("TestCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TestCategory");
                });

            modelBuilder.Entity("TestingServerApp.User", b =>
                {
                    b.HasOne("TestingServerApp.UserGroup", "UserGroup")
                        .WithMany("Users")
                        .HasForeignKey("UserGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserGroup");
                });

            modelBuilder.Entity("TestingServerApp.Question", b =>
                {
                    b.Navigation("Answers");
                });

            modelBuilder.Entity("TestingServerApp.ShortResult", b =>
                {
                    b.Navigation("DetailedResults");
                });

            modelBuilder.Entity("TestingServerApp.Test", b =>
                {
                    b.Navigation("IssuedTests");

                    b.Navigation("Questions");

                    b.Navigation("ShortResult");
                });

            modelBuilder.Entity("TestingServerApp.TestCategory", b =>
                {
                    b.Navigation("Tests");
                });

            modelBuilder.Entity("TestingServerApp.User", b =>
                {
                    b.Navigation("ShortResults");
                });

            modelBuilder.Entity("TestingServerApp.UserGroup", b =>
                {
                    b.Navigation("IssuedTests");

                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
