
--============================= tabella Users =============================
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    NomeUtente NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL, -- salvare hash della password
    Email NVARCHAR(255) NULL,
    DataCreazione DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- ============================= tabella Quiz  =============================

CREATE TABLE Quiz (
    QuizID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,                  -- proprietario del quiz
    Nome NVARCHAR(255) NOT NULL,
    ValoriDifficolta NVARCHAR(MAX) NULL, -- JSON o stringa da parsare
    Pubblico BIT NOT NULL DEFAULT 0,      -- 0 = privato, 1 = pubblico
    CONSTRAINT FK_Quiz_User FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

--============================= tabella Domanda =============================
CREATE TABLE Domanda (
    DomandaID INT IDENTITY(1,1) PRIMARY KEY,
    QuizID INT NOT NULL,
    Testo NVARCHAR(MAX) NOT NULL,
    RisposteGiuste NVARCHAR(MAX) NOT NULL,
    RisposteSbagliate NVARCHAR(MAX) NOT NULL,
    TempoRisposta INT NULL,
    Sequenza NVARCHAR(100) NULL,
    NumeroSequenza INT NULL,
    Variante NVARCHAR(100) NULL,
    Difficolta NVARCHAR(50) NULL,
    CONSTRAINT FK_Domanda_Quiz FOREIGN KEY (QuizID) REFERENCES Quiz(QuizID)
);
GO

-- ============================= tabella QuizSeed =============================

CREATE TABLE QuizSeed (
    QuizSeedID INT IDENTITY(1,1) PRIMARY KEY,
    QuizID INT NOT NULL,
    UserID INT NOT NULL,                   -- proprietario del quizseed
    Nome NVARCHAR(255) NOT NULL,
    NumeroDomande INT NOT NULL,
    Modalita NVARCHAR(50) NOT NULL,       -- Sfida / Verifica
    TempoTotale INT NULL,
    SommaTempoDomande BIT NULL,
    PossibilitaTornareIndietro BIT NULL,
    PossibilitaScartoTempo BIT NULL,
    Pubblico BIT NOT NULL DEFAULT 0,       -- 0 = privato, 1 = pubblico
    CONSTRAINT FK_QuizSeed_Quiz FOREIGN KEY (QuizID) REFERENCES Quiz(QuizID),
    CONSTRAINT FK_QuizSeed_User FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- ============================= tabella Scoreboard  =============================
CREATE TABLE Scoreboard (
    ScoreboardID INT IDENTITY(1,1) PRIMARY KEY,
    QuizSeedID INT NOT NULL,               -- riferimento alla configurazione pull
    QuizID INT NOT NULL,                   -- riferimento al quiz originale
    UserID INT NOT NULL,                   -- giocatore che ha fatto il quiz
    Punteggio INT NOT NULL,
    DataSessione DATETIME NOT NULL DEFAULT GETDATE(),
    Media FLOAT NULL,
    CONSTRAINT FK_Scoreboard_QuizSeed FOREIGN KEY (QuizSeedID) REFERENCES QuizSeed(QuizSeedID),
    CONSTRAINT FK_Scoreboard_Quiz FOREIGN KEY (QuizID) REFERENCES Quiz(QuizID),
    CONSTRAINT FK_Scoreboard_User FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO
