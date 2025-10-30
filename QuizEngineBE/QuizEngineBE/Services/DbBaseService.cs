using Microsoft.EntityFrameworkCore;
using QuizEngineBE.Interfaces;
using QuizEngineBE.Models;
using Serilog;
using System.Data.Common;

namespace QuizEngineBE.Services
{
        public class DbBaseService<TContext>(TContext db) : IDbBaseService  where TContext: DbContext
        {
            protected readonly TContext _db = db;

        //============================== METODI PER OPERAZIONI SICURE ===============================================
        public async Task<bool> SafeExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                await operation(ct);                   // passa il Token all'operazione
                Log.Debug("action completed, saving in db");
                await SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync(ct);
                Log.Warning(ex, "concurrency conflict: {Message}", ex.Message);
                return false;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync(ct);
                Log.Error(ex, "db update failed: {Message}", ex.Message);
                return false;
            }
            catch (DbException ex)
            {
                await transaction.RollbackAsync(ct);
                Log.Error(ex, "db provider error: {Message}", ex.Message);
                return false;
            }
            catch (TimeoutException ex)
            {
                await transaction.RollbackAsync(ct);
                Log.Error(ex, "operation timed out: {Message}", ex.Message);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                await transaction.RollbackAsync(ct);
                Log.Warning(ex, "operation canceled: {Message}", ex.Message);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync(ct);
                Log.Error(ex, "invalid operation: {Message}", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                Log.Error(ex, "unexpected error {Message}",ex.Message);
                return false;
            }
        }

        public async Task<T?> SafeQueryAsync<T>(Func<CancellationToken, Task<T>> query, CancellationToken ct = default)
        {

            try
            {
                return await query(ct);
            }
            catch (DbException ex)
            {
                Log.Error(ex, "db provider error: {Message}", ex.Message);
                return default;
            }
            catch (TimeoutException ex)
            {
                Log.Error(ex, "query timed out: {Message}", ex.Message);
                return default;
            }
            catch (OperationCanceledException ex)
            {
                Log.Warning(ex, "query canceled: {Message}", ex.Message);
                return default;
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, "invalid operation: {Message}", ex.Message);
                return default;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "unexpected error executing query: {Message} ",ex.Message);
                return default;
            }
        }



        /// <summary>
        /// salva le modifiche apportate al contesto del database.
        /// </summary>
        protected async Task SaveChangesAsync(CancellationToken ct = default)
        {
            try
            {

                Log.Debug("saving in db");
                await _db.SaveChangesAsync(ct);
                
            }
            catch(Exception ex)
            {
                Log.Warning("error during save : {Message}", ex.Message);
            }


        }

    }



}
