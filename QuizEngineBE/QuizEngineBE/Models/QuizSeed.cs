using System;
using System.Collections.Generic;

namespace QuizEngineBE.Models;

public partial class QuizSeed
{
    public int QuizSeedId { get; set; }

    public int QuizId { get; set; }

    public int UserId { get; set; }

    public string Nome { get; set; } = null!;

    public int NumeroDomande { get; set; }

    public string Modalita { get; set; } = null!;

    public int? TempoTotale { get; set; }

    public bool? SommaTempoDomande { get; set; }

    public bool? PossibilitaTornareIndietro { get; set; }

    public bool? PossibilitaScartoTempo { get; set; }

    public bool Pubblico { get; set; }

    public virtual ICollection<Pull> Pulls { get; set; } = new List<Pull>();

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual ICollection<Scoreboard> Scoreboards { get; set; } = new List<Scoreboard>();

    public virtual User User { get; set; } = null!;
}
