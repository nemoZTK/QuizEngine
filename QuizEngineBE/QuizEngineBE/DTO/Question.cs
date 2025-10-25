using Microsoft.IdentityModel.Tokens;

namespace QuizEngineBE.DTO
{
    public class Question
    {
        public string Testo { get; set; } = null!;

        public List<string> RisposteGiuste { get; set; } = null!;

        public List<string> RisposteSbagliate  { get; set; } = null!;

        public int? Tempo { get; set; }

        public string? Difficolta { get; set; }

        public string? Sequenza { get; set; }

        public int? NumeroSequenza { get; set; }

        public string? Variante { get; set; }




        public bool CheckFields =>
            !string.IsNullOrWhiteSpace(Testo) ||
            !List<string>.Equals(RisposteGiuste,"") ||
            !List<string>.Equals(RisposteSbagliate,"")||
            (Sequenza==null||(!Sequenza.IsWhiteSpace()))||
            (Variante==null || (!Variante.IsWhiteSpace()))||
            (NumeroSequenza==null || (NumeroSequenza<0));

    }



}
