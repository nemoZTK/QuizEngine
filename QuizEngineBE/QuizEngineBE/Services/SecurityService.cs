namespace QuizEngineBE.Services
{
    public class SecurityService
    {
        public string Encrypt(string word)
        {
            return Convert.ToBase64String(
                   System.Security.Cryptography.SHA256.HashData(
                   System.Text.Encoding.UTF8.GetBytes(word)));

        }



    }
}
