using System;
using System.Collections.Generic;

namespace QuizEngineBE.Models;

public partial class Scoreboard
{
    public int ScoreboardId { get; set; }

    public int QuizSeedId { get; set; }

    public int QuizId { get; set; }

    public int UserId { get; set; }

    public int Punteggio { get; set; }

    public DateTime DataSessione { get; set; }

    public double? Media { get; set; }

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual QuizSeed QuizSeed { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
