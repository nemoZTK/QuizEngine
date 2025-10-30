using System;
using System.Collections.Generic;

namespace QuizEngineBE.Models;

public partial class PullDomande
{
    public int PullDomandeId { get; set; }

    public int PullId { get; set; }

    public int DomandaId { get; set; }

    public virtual Domanda Domanda { get; set; } = null!;

    public virtual Pull Pull { get; set; } = null!;
}
