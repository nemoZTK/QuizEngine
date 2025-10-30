using System;
using System.Collections.Generic;

namespace QuizEngineBE.Models;

public partial class Quiz
{
    public int QuizId { get; set; }

    public int UserId { get; set; }

    public string Nome { get; set; } = null!;

    public string? ValoriDifficolta { get; set; }

    public bool Pubblico { get; set; }

    public virtual ICollection<Domanda> Domanda { get; set; } = new List<Domanda>();

    public virtual ICollection<Pull> Pulls { get; set; } = new List<Pull>();

    public virtual ICollection<QuizSeed> QuizSeeds { get; set; } = new List<QuizSeed>();

    public virtual ICollection<Scoreboard> Scoreboards { get; set; } = new List<Scoreboard>();

    public virtual User User { get; set; } = null!;
}
