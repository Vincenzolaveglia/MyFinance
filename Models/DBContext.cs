using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace MyFinance.Models
{
    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DBContext")
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(e => e.Transactions)
                .WithOptional(e => e.Categories)
                .HasForeignKey(e => e.Id_Category);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Transactions)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.Id_User);
        }
    }
}
