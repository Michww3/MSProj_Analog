using Microsoft.EntityFrameworkCore;
using MSProj_Analog.DTOs;
using System;

namespace MSProj_Analog.Helpers
{
    public class AppDbContext : DbContext
    {
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ProjectTask> Tasks { get; set; }
        public AppDbContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MSProjAnalog;Trusted_Connection=True;");
                optionsBuilder.UseSqlite("Data Source=MSPRojAnalog.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectTask>()
                .HasMany(t => t.AssignedResource)
                .WithOne(r => r.AppointedTask)
                .HasForeignKey("AppointedTaskId");

            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.PreviousTask)
                .WithOne()
                .HasForeignKey<ProjectTask>("PreviousTaskId");

            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.NextTask)
                .WithOne()
                .HasForeignKey<ProjectTask>("NextTaskId");

            modelBuilder.Entity<Resource>()
                .Property(r => r.StandardRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Resource>()
                .Property(r => r.OvertimeRate)
                .HasColumnType("decimal(18,2)");
        }
    }
}
