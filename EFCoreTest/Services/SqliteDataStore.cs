using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EFCoreTest.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreTest.Services
{
    internal sealed class MyDbContext : DbContext
    {
        private readonly string connectionString;
        public DbSet<Item> Items { get; set; }

        public MyDbContext()
        {
            var dataDir = Xamarin.Essentials.FileSystem.AppDataDirectory;
            connectionString = $"Filename={Path.Combine(dataDir, "localdb.sqlite")}";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString);
        }
    }

    public class SqliteDataStore : IDataStore<Item>
    {
        private MyDbContext CreateDbContext()
        {
            var dbContext = new MyDbContext();
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
            return dbContext;
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            // Append new id if needed
            if (item.Id == null)
            {
                item.Id = Guid.NewGuid().ToString();
            }

            using (var dbContext = CreateDbContext())
            {
                var entry = await dbContext.Items.AddAsync(item);
                var added = entry.State == EntityState.Added;
                await dbContext.SaveChangesAsync();
                return added;
            }
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            using (var dbContext = CreateDbContext())
            {
                var item = await dbContext.Items.FindAsync(id);
                if (item == null) return false;

                var entry = dbContext.Items.Remove(item);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<Item> GetItemAsync(string id)
        {
            using (var dbContext = CreateDbContext())
            {
                return await dbContext.Items.FindAsync(id);
            }
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            using (var dbContext = CreateDbContext())
            {
                return await dbContext.Items.ToListAsync();
            }
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            using (var dbContext = CreateDbContext())
            {
                var entry = dbContext.Items.Update(item);
                var updated = entry.State == EntityState.Modified;
                await dbContext.SaveChangesAsync();
                return updated;
            }
        }
    }
}
