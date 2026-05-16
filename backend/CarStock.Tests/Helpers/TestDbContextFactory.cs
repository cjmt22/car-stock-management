using CarStock.API.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace CarStock.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static AppDbContext Create()
        {
            // Each test gets a unique database name
            // so they never share data with each other
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            // Make sure the database is created fresh
            context.Database.EnsureCreated();

            return context;
        }
    }
}