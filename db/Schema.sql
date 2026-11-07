IF DB_ID('GreenNet') IS NULL CREATE DATABASE GreenNet;
GO
USE GreenNet;
GO
CREATE TABLE Users (
    Id           INT IDENTITY PRIMARY KEY,
    Username     NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE TABLE Tokens (
    Token     CHAR(36)     NOT NULL PRIMARY KEY,
    Username  NVARCHAR(50) NOT NULL,
    ExpiresAt DATETIME2    NOT NULL
);
CREATE TABLE Sessions (
    Id            INT IDENTITY PRIMARY KEY,
    Username      NVARCHAR(50)  NOT NULL,
    ServerName    NVARCHAR(50)  NOT NULL,
    FileName      NVARCHAR(260) NOT NULL,
    FileSizeBytes BIGINT        NOT NULL,
    BytesSent     BIGINT        NOT NULL,
    Algo          VARCHAR(10)   NOT NULL,
    Protocol      VARCHAR(3)    NOT NULL DEFAULT 'TCP',
    Encrypted     BIT           NOT NULL,
    DurationMs    FLOAT         NOT NULL,
    RttMs         FLOAT         NOT NULL,
    CpuMs         FLOAT         NOT NULL,
    EnergyJ       FLOAT         NOT NULL,
    GreenScore    FLOAT         NOT NULL,
    Sha256Ok      BIT           NOT NULL,
    CreatedAt     DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE INDEX IX_Sessions_User ON Sessions(Username, Id DESC);
GO
