using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuizEngineBE.Models;

public partial class QuizDbContext : DbContext
{
    public QuizDbContext()
    {
    }

    public QuizDbContext(DbContextOptions<QuizDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Domanda> Domanda { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<QuizSeed> QuizSeeds { get; set; }

    public virtual DbSet<Scoreboard> Scoreboards { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domanda>(entity =>
        {
            entity.HasKey(e => e.DomandaId).HasName("PK__Domanda__8C939E4FD316A5C1");

            entity.Property(e => e.DomandaId).HasColumnName("DomandaID");
            entity.Property(e => e.Difficolta).HasMaxLength(50);
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.Sequenza).HasMaxLength(100);
            entity.Property(e => e.Variante).HasMaxLength(100);

            entity.HasOne(d => d.Quiz).WithMany(p => p.Domanda)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Domanda_Quiz");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(e => e.QuizId).HasName("PK__Quiz__8B42AE6EF73A7DBC");

            entity.ToTable("Quiz");

            entity.HasIndex(e => e.Nome, "UQ_Quiz_Nome").IsUnique();

            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.Nome).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Quizzes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Quiz_User");
        });

        modelBuilder.Entity<QuizSeed>(entity =>
        {
            entity.HasKey(e => e.QuizSeedId).HasName("PK__QuizSeed__54456234F5C34385");

            entity.ToTable("QuizSeed");

            entity.Property(e => e.QuizSeedId).HasColumnName("QuizSeedID");
            entity.Property(e => e.Modalita).HasMaxLength(50);
            entity.Property(e => e.Nome).HasMaxLength(255);
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Quiz).WithMany(p => p.QuizSeeds)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuizSeed_Quiz");

            entity.HasOne(d => d.User).WithMany(p => p.QuizSeeds)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuizSeed_User");
        });

        modelBuilder.Entity<Scoreboard>(entity =>
        {
            entity.HasKey(e => e.ScoreboardId).HasName("PK__Scoreboa__757ACE4C06AFADB8");

            entity.ToTable("Scoreboard");

            entity.Property(e => e.ScoreboardId).HasColumnName("ScoreboardID");
            entity.Property(e => e.DataSessione).HasColumnType("datetime");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.QuizSeedId).HasColumnName("QuizSeedID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Quiz).WithMany(p => p.Scoreboards)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Scoreboard_Quiz");

            entity.HasOne(d => d.QuizSeed).WithMany(p => p.Scoreboards)
                .HasForeignKey(d => d.QuizSeedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Scoreboard_QuizSeed");

            entity.HasOne(d => d.User).WithMany(p => p.Scoreboards)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Scoreboard_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACEB38D605");

            entity.HasIndex(e => e.NomeUtente, "UQ_Users_NomeUtente").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.DataCreazione).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.NomeUtente).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Ruolo).HasMaxLength(50);
            entity.Property(e => e.Salt).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
