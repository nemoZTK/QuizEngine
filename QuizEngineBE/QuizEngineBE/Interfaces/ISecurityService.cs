namespace QuizEngineBE.Interfaces
{
    /// <summary>
    /// service che si occupa della sicurezza del sito, crea gli hash delle password, genera e controlla la validità dei token
    /// </summary>
    public interface ISecurityService
    {

        /// <summary>
        /// genera un jwt token dato username, ruolo e scadenza in minuti
        /// </summary>
        /// <returns>una stringa col jwt token</returns>
        string GenerateJwtToken(string username, string role, int expiryMinutes);

        /// <summary>
        /// controlla la validità del token e la corrispondenza con l'username passato
        /// </summary>
        /// <returns>una tupla con un bool e una stringa (success e message)</returns>
        (bool Success, string Message) ValidateJwtTokenForUser(string username, string token);
        //TODO: ci starebbe l'aggiunta di ruoli (è tutto già pronto nel db, ma per ora son tutti ruolo "1")

        /// <summary>
        /// Estrae il token JWT dall'header Authorization, rimuovendo "Bearer " se presente.
        /// va passato l' authHeader cosi come lo estrai
        /// </summary>
        /// <returns>Il token JWT pulito, oppure null se mancante o vuoto</returns>
        string? GetBearerToken(string authHeader);


        /// <summary>
        /// genera un sale casualmente, data la sua grandezza in byte 
        /// </summary>
        /// <returns>stringa col sale generato</returns>
        string GenerateSalt(int length);


        /// <summary>
        /// cripta una parola (la trasforma in hash fa l'encode in base64
        /// </summary>
        /// <returns>una stringa con l'hash della parola fornita base64 encoded </returns>
        string EncryptSHA256xBase64(string word);
    }
}
