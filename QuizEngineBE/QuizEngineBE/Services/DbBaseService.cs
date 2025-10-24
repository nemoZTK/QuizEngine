using Microsoft.EntityFrameworkCore;
using QuizEngineBE.Models;
using Serilog;
using System.Data.Common;

namespace QuizEngineBE.Services
{
        public class DbBaseService<TContext>(TContext db) where TContext : DbContext
        {
            protected readonly TContext _db = db;

        //============================== METODI PER OPERAZIONI SICURE ===============================================
        protected async Task<bool> SafeExecuteAsync(Func<Task> operation)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                await operation();
                Log.Debug("action completed, saving in db");
                await SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                Log.Warning(ex, "concurrency conflict while performing database operation: {Message}", ex.Message);
                return false;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                Log.Error(ex, "database update failed: {Message}", ex.Message);
                return false;
            }
            catch (DbException ex)
            {
                await transaction.RollbackAsync();
                Log.Error(ex, "database provider error: {Message}", ex.Message);
                return false;
            }
            catch (TimeoutException ex)
            {
                await transaction.RollbackAsync();
                Log.Error(ex, "database operation timed out: {Message}", ex.Message);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                await transaction.RollbackAsync();
                Log.Warning(ex, "database operation was canceled: {Message}", ex.Message);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync();
                Log.Error(ex, "invalid operation during database action: {Message}", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(ex, "unexpected error during database operation");
                return false;
            }
        }

        protected async Task<T?> SafeQueryAsync<T>(Func<Task<T>> query)
        {
            try
            {
                return await query();
            }
            catch (DbException ex)
            {
                Log.Error(ex, "database provider error during query: {Message}", ex.Message);
                return default;
            }
            catch (TimeoutException ex)
            {
                Log.Error(ex, "query timed out: {Message}", ex.Message);
                return default;
            }
            catch (OperationCanceledException ex)
            {
                Log.Warning(ex, "query was canceled: {Message}", ex.Message);
                return default;
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, "invalid operation during query: {Message}", ex.Message);
                return default;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "unexpected error executing query");
                return default;
            }
        }



        /// <summary>
        /// salva le modifiche apportate al contesto del database.
        /// </summary>
        protected async Task SaveChangesAsync()
        {
            Log.Debug("saving in db");
            await _db.SaveChangesAsync();

        }



    }
}
