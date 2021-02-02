using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TooSimple.Models.EFModels;

namespace TooSimple.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<FundingSchedule> FundingSchedules { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionCategory> TransactionCategories { get; set; }
        public DbSet<FundingHistory> FundingHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("Account");
            modelBuilder.Entity<FundingSchedule>().ToTable("FundingSchedule");
            modelBuilder.Entity<Goal>().ToTable("Goal");
            modelBuilder.Entity<Transaction>().ToTable("Transaction");
            modelBuilder.Entity<TransactionCategory>().ToTable("TransactionCategory");
            modelBuilder.Entity<FundingHistory>().ToTable("FundingHistory");

            base.OnModelCreating(modelBuilder);
        }
    }
}
