using global::QuizEngineBE.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace QuizEngineBE.Tests
{
    public class DbBaseServiceTests
    {
        private readonly ITestOutputHelper _output;

        public DbBaseServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        //======================== Test DbContext ========================
        private class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
            public DbSet<TestEntity> TestEntities => Set<TestEntity>();
        }

        private class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        //======================== Derivata per test ========================
        private class TestDbService : DbBaseService<TestDbContext>
        {
            public TestDbService(TestDbContext db) : base(db) { }

            public Task<bool> TestSafeExecuteAsync(Func<CancellationToken, Task> op, CancellationToken ct = default)
                => SafeExecuteAsync(op, ct);

            public Task<T?> TestSafeQueryAsync<T>(Func<CancellationToken, Task<T>> q, CancellationToken ct = default)
                => SafeQueryAsync(q, ct);

            public Task TestSaveChangesAsync(CancellationToken ct = default)
                => SaveChangesAsync(ct);
        }

        //========================1 Test SafeExecuteAsync ========================
        [Fact]
        public async Task SafeExecuteAsync_Tests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase("SafeExecuteAsync_Tests")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            await using var db = new TestDbContext(options);
            var service = new TestDbService(db);

            bool isPassed;

            //=========1-1 operazione normale
            isPassed = await service.TestSafeExecuteAsync(async ct =>
            {
                db.TestEntities.Add(new TestEntity { Name = "Entity1" });
                await Task.CompletedTask;
            });
            Assert.True(isPassed, "DB TEST FAILED SafeExecuteAsync -->1-1\nproblem -> operazione sicura fallita");

            //=========1-2 verifica salvataggio
            var saved = await db.TestEntities.FirstOrDefaultAsync();
            isPassed = saved != null && saved.Name == "Entity1";
            Assert.True(isPassed, "DB TEST FAILED SafeExecuteAsync -->1-2\nproblem -> entity non salvata correttamente");

            //=========1-3 con InvalidOperationException
            isPassed = await service.TestSafeExecuteAsync(async ct =>
            {
                throw new InvalidOperationException("simulated invalid operation");
            });
            Assert.False(isPassed, "DB TEST FAILED SafeExecuteAsync -->1-3\nproblem -> eccezione non gestita correttamente");

            //=========1-4 con DbUpdateException simulata
            isPassed = await service.TestSafeExecuteAsync(async ct =>
            {
                db.TestEntities.Add(new TestEntity { Name = "EntityDup" });
                throw new DbUpdateException("Simulated DbUpdateException", new Exception());
            });
            Assert.False(isPassed, "DB TEST FAILED SafeExecuteAsync -->1-4\nproblem -> DbUpdateException non gestita correttamente");

            //=========1-5 rollback su eccezione generica
            isPassed = await service.TestSafeExecuteAsync(async ct =>
            {
                db.TestEntities.Add(new TestEntity { Name = "EntityBeforeError" });
                await Task.CompletedTask;
                throw new Exception("Force rollback");
            });
            Assert.False(isPassed, "DB TEST FAILED SafeExecuteAsync -->1-5\nproblem -> rollback non avvenuto con eccezione generica");

            var totalEntities = await db.TestEntities.CountAsync();
            isPassed = totalEntities == 1; // solo Entity1 deve rimanere
            Assert.True(isPassed, "DB TEST FAILED SafeExecuteAsync -->1-6\nproblem -> rollback non ha annullato le modifiche come previsto");
        }

        //========================2 Test SafeQueryAsync ========================
        [Fact]
        public async Task SafeQueryAsync_Tests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase("SafeQueryAsync_Tests")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            await using var db = new TestDbContext(options);
            var service = new TestDbService(db);

            db.TestEntities.Add(new TestEntity { Name = "Entity1" });
            await db.SaveChangesAsync();

            bool isPassed;

            //=========2-1 query normale
            var count = await service.TestSafeQueryAsync(async ct =>
            {
                return await db.TestEntities.CountAsync(ct);
            });
            isPassed = count == 1;
            Assert.True(isPassed, "DB TEST FAILED SafeQueryAsync -->2-1\nproblem -> query non ha restituito il numero corretto");

            //=========2-2 query con errore
            var resultError = await service.TestSafeQueryAsync<int>(async ct =>
            {
                throw new InvalidOperationException("simulated query error");
            });
            isPassed = resultError == default;
            Assert.True(isPassed, "DB TEST FAILED SafeQueryAsync -->2-2\nproblem -> query con errore non ha restituito default");
        }

        //========================3 Test SaveChangesAsync ========================
        [Fact]
        public async Task SaveChangesAsync_Tests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase("SaveChangesAsync_Tests")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            await using var db = new TestDbContext(options);
            var service = new TestDbService(db);

            //=========3-1 salvataggio normale
            db.TestEntities.Add(new TestEntity { Name = "EntityFinal" });
            await service.TestSaveChangesAsync();

            var saved = await db.TestEntities.FirstOrDefaultAsync(e => e.Name == "EntityFinal");
            bool isPassed = saved != null;
            Assert.True(isPassed, "DB TEST FAILED SaveChangesAsync -->3-1\nproblem -> SaveChangesAsync non ha salvato correttamente");
        }
    }
}
