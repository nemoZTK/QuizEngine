namespace QuizEngineBE.Interfaces
{
    /// <summary>
    /// intefaccia di DbBaseService, l'unico service a conoscere il contesto del db,
    /// <br/> è quello che in ultimo luogo va ad interagire con il database
    /// <br/> usa un DbContext
    /// </summary>
    public interface IDbBaseService
    {


        /// <summary>
        /// esegue un operazione sul database in sicurezza, va passata una lambda ed un cancellation token
        /// </summary>
        /// <returns>true se è andata a buon fine, false altrimenti</returns>
        Task<bool> SafeExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken ct);



        /// <summary>
        /// esegue una query nel database(in sicurezza), va passata la query come lambda ed un cancellation token
        /// </summary>
        /// <returns>un oggetto T opzionale</returns>
        Task<T?> SafeQueryAsync<T>(Func<CancellationToken, Task<T>> query, CancellationToken ct);
    }
}
