using System;
using System.Collections.Generic;

namespace QuizEngineBE.Models;

public partial class Domanda
{
    public int DomandaId { get; set; }

    public int QuizId { get; set; }

    public string Testo { get; set; } = null!;

    public string RisposteGiuste { get; set; } = null!;

    public string RisposteSbagliate { get; set; } = null!;

    public int? TempoRisposta { get; set; }

    public string? Sequenza { get; set; }

    public int? NumeroSequenza { get; set; }

    public string? Variante { get; set; }

    public string? Difficolta { get; set; }

    public virtual ICollection<PullDomande> PullDomanda { get; set; } = new List<PullDomande>();

    public virtual Quiz Quiz { get; set; } = null!;
}
