using System;
using System.Collections.Generic;

namespace QuizEngineBE.Models;

public partial class Pull
{
    public int PullId { get; set; }

    public int QuizId { get; set; }

    public int? QuizSeedId { get; set; }

    public string Nome { get; set; } = null!;

    public DateTime DataCreazione { get; set; }

    public virtual ICollection<PullDomande> PullDomandes { get; set; } = new List<PullDomande>();

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual QuizSeed? QuizSeed { get; set; }
}
