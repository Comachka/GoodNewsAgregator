using Microsoft.EntityFrameworkCore;
using myProject.Data.Entities;
using System.Text.RegularExpressions;

namespace myProject.Data
{
    public class MyProjectContext: DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<NewsResource> NewsResources { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserCategory> UserCategories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public MyProjectContext(DbContextOptions<MyProjectContext> options) 
            : base (options)
        {
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>()
                        .HasOne(s => s.FollowOn)
                        .WithMany(u => u.MyFollows)
                        .HasForeignKey(s => s.FollowOnId)
                        .OnDelete(DeleteBehavior.Restrict);
                        //.WillCascadeOnDelete(false);

            modelBuilder.Entity<Subscription>()
                        .HasOne(s => s.Follower)
                        .WithMany(u => u.MyFollowers)
                        .HasForeignKey(s => s.FollowerId)
                        .OnDelete(DeleteBehavior.Restrict);
                        //.WillCascadeOnDelete(false);
        }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}