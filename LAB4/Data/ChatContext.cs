using LAB4.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace LAB4.Data
{
    public class ChatContext : DbContext
    {
        public virtual DbSet<ChatInfo> ChatInfos { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public ChatContext([NotNull] DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(i => i.Name).IsUnique();
                
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Id).ValueGeneratedOnAdd();

                entity.HasMany(x => x.RUserChats)
                        .WithOne(x => x.User).OnDelete(DeleteBehavior.SetNull);
            });


            modelBuilder.Entity<ChatInfo>(entity =>
            {
                entity.HasIndex(i => i.Name).IsUnique();

                entity.HasKey(i => i.Id);
                entity.Property(i => i.Id).ValueGeneratedOnAdd();

                entity.HasMany(x => x.RUserChats)
                        .WithOne(x => x.ChatInfo).OnDelete(DeleteBehavior.SetNull);
            });

        }
    }
}
