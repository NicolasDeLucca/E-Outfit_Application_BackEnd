using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Blogs.DataAccess.Contexts
{
    [ExcludeFromCodeCoverage]
    public class DataContext : DbContext
    {
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Mention> Mentions { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Notification<Mention>> MentionNotifications { get; set; }
        public virtual DbSet<Notification<Article>> ArticleNotifications { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Offense> Offenses { get; set; }

        public DataContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //tables per hierarcy 

            modelBuilder.Entity<Mention>().HasDiscriminator<string>("MentionType").
                HasValue<Comment>("Comment").
                HasValue<Response>("Response");

            //one to one relationships configs

            modelBuilder.Entity<Mention>().
                HasOne<Notification<Mention>>().
                WithOne(n => n.AssociatedContent).
                HasForeignKey<Notification<Mention>>("MentionId").
                OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Article>().
                HasOne<Notification<Article>>().
                WithOne(n => n.AssociatedContent).
                HasForeignKey<Notification<Article>>("ArticleId").
                OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>().
                HasOne<Session>().
                WithOne(s => s.AuthenticatedUser).
                HasForeignKey<Session>("UserId").
                OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>().
                HasOne<Response>().
                WithOne(r => r.AssociatedComment).
                HasForeignKey<Response>("CommentId").
                OnDelete(DeleteBehavior.Restrict);

            //one to many relationships configs

            modelBuilder.Entity<Image>().
                HasOne(i => i.Article).
                WithMany(a => a.Images).
                HasForeignKey("ArticleId");

            modelBuilder.Entity<Article>().
                HasOne(a => a.Author).
                WithMany(u => u.PostedArticles).
                HasForeignKey("UserId");

            modelBuilder.Entity<Mention>().
                HasOne(m => m.Article).
                WithMany(a => a.Mentions).
                HasForeignKey("ArticleId");

            modelBuilder.Entity<Mention>().
                HasOne(m => m.Author).
                WithMany(u => u.PostedMentions).
                HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification<Mention>>().
                HasOne(n => n.Receiver).
                WithMany(u => u.MentionNotifications).
                HasForeignKey("UserId");

            modelBuilder.Entity<Notification<Article>>().
                HasOne(n => n.Receiver).
                WithMany(u => u.ArticleNotifications).
                HasForeignKey("UserId");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var directory = Directory.GetCurrentDirectory();

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(directory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString(@"BlogsDB");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
