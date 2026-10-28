using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TechTrend.Api.Data;
using TechTrend.Api.Services;

namespace TechTrend.Api.Tests
{
    public class TechTrendTest
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        // GetTechnologyById: get not existing technology
        [Fact]
        public async Task GetTechnologyById_WhenTechnologyDoesNotExist_ReturnsNull()
        {
            using var db = CreateDbContext();
            var service = new TechnologyService(db);

            var result = await service.GetTechnologyByIdAsync(999);

            Assert.Null(result);
        }

        // GetTechnologies: get empty list from empty db 
        [Fact]
        public async Task GetTechnologies_WhenDatabaseIsEmpty_ReturnsEmptyList()
        {
            using var db = CreateDbContext();
            var service = new TechnologyService(db);

            var result = await service.GetTechnologiesAsync(null, null, null, null, null);

            Assert.NotNull(result);
            Assert.Empty((System.Collections.IEnumerable)result);
        }
    }
}
