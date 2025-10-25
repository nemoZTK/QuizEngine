using System;
using System.Collections.Generic;

namespace QuizEngineBE.Models;

public partial class User
{
    public int UserId { get; set; }

    public string NomeUtente { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime DataCreazione { get; set; } = DateTime.Now;

    public string Salt { get; set; } = null!;

    public string Ruolo { get; set; } = null!;

    public virtual ICollection<QuizSeed> QuizSeeds { get; set; } = [];

    public virtual ICollection<Quiz> Quizzes { get; set; } = [];

    public virtual ICollection<Scoreboard> Scoreboards { get; set; } = [];
}
