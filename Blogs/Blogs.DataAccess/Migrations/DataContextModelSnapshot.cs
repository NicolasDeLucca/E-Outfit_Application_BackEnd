﻿// <auto-generated />
using System;
using Blogs.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Blogs.DataAccess.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<int>("Template")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("Visibility")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ArticleId")
                        .HasColumnType("int");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Mentions.Mention", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ArticleId")
                        .HasColumnType("int");

                    b.Property<string>("MentionType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("UserId");

                    b.ToTable("Mentions");

                    b.HasDiscriminator<string>("MentionType").HasValue("Mention");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Notification<Blogs.Domain.BusinessEntities.Article>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ArticleId")
                        .HasColumnType("int");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("ArticleNotifications");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Notification<Blogs.Domain.BusinessEntities.Mentions.Mention>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("MentionId")
                        .HasColumnType("int");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MentionId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("MentionNotifications");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Offense", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Word")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Offenses");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Session", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("AuthToken")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("nvarchar(12)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Mentions.Comment", b =>
                {
                    b.HasBaseType("Blogs.Domain.BusinessEntities.Mentions.Mention");

                    b.HasDiscriminator().HasValue("Comment");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Mentions.Response", b =>
                {
                    b.HasBaseType("Blogs.Domain.BusinessEntities.Mentions.Mention");

                    b.Property<int>("CommentId")
                        .HasColumnType("int");

                    b.HasIndex("CommentId")
                        .IsUnique()
                        .HasFilter("[CommentId] IS NOT NULL");

                    b.HasDiscriminator().HasValue("Response");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Article", b =>
                {
                    b.HasOne("Blogs.Domain.BusinessEntities.User", "Author")
                        .WithMany("PostedArticles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Image", b =>
                {
                    b.HasOne("Blogs.Domain.BusinessEntities.Article", "Article")
                        .WithMany("Images")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Mentions.Mention", b =>
                {
                    b.HasOne("Blogs.Domain.BusinessEntities.Article", "Article")
                        .WithMany("Mentions")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Blogs.Domain.BusinessEntities.User", "Author")
                        .WithMany("PostedMentions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Article");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Notification<Blogs.Domain.BusinessEntities.Article>", b =>
                {
                    b.HasOne("Blogs.Domain.BusinessEntities.Article", "AssociatedContent")
                        .WithOne()
                        .HasForeignKey("Blogs.Domain.BusinessEntities.Notification<Blogs.Domain.BusinessEntities.Article>", "ArticleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Blogs.Domain.BusinessEntities.User", "Receiver")
                        .WithMany("ArticleNotifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssociatedContent");

                    b.Navigation("Receiver");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Notification<Blogs.Domain.BusinessEntities.Mentions.Mention>", b =>
                {
                    b.HasOne("Blogs.Domain.BusinessEntities.Mentions.Mention", "AssociatedContent")
                        .WithOne()
                        .HasForeignKey("Blogs.Domain.BusinessEntities.Notification<Blogs.Domain.BusinessEntities.Mentions.Mention>", "MentionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Blogs.Domain.BusinessEntities.User", "Receiver")
                        .WithMany("MentionNotifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssociatedContent");

                    b.Navigation("Receiver");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Session", b =>
                {
                    b.HasOne("Blogs.Domain.BusinessEntities.User", "AuthenticatedUser")
                        .WithOne()
                        .HasForeignKey("Blogs.Domain.BusinessEntities.Session", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AuthenticatedUser");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Mentions.Response", b =>
                {
                    b.HasOne("Blogs.Domain.BusinessEntities.Mentions.Comment", "AssociatedComment")
                        .WithOne()
                        .HasForeignKey("Blogs.Domain.BusinessEntities.Mentions.Response", "CommentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AssociatedComment");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.Article", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Mentions");
                });

            modelBuilder.Entity("Blogs.Domain.BusinessEntities.User", b =>
                {
                    b.Navigation("ArticleNotifications");

                    b.Navigation("MentionNotifications");

                    b.Navigation("PostedArticles");

                    b.Navigation("PostedMentions");
                });
#pragma warning restore 612, 618
        }
    }
}
