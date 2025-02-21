using Microsoft.EntityFrameworkCore;
using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using System;
using System.Windows;

namespace MSProj_Analog.Helpers
{
    public class AppDbContext : DbContext
    {
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ProjectTask> Tasks { get; set; }

        public AppDbContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConfigOptions.ConnectionString);
            }
        }
    }
}
