CREATE TABLE [dbo].[Profiles]
(
    [Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [FirstName] NVARCHAR(1000) NOT NULL, 
    [LastName] NVARCHAR(1000) NOT NULL, 
    [UserName] NVARCHAR(1000) NOT NULL, 
    [Email ] NVARCHAR(1000) NOT NULL, 
    [Password] NVARCHAR(MAX) NOT NULL, 
    [ProfileType] INT NOT NULL DEFAULT 1,
    [LastSeen] DATETIME2 NULL DEFAULT GETDATE(), 
    [StatusText] TEXT NULL DEFAULT 'Travelling to Afganistan',
    [IsLoggedIn] INT DEFAULT 0,
    [CreatedAt] DATETIME2 NULL DEFAULT GETDATE(), 
    [CreatedBy] INT NULL, 
    [LastUpdatedAt] DATETIME2 NULL DEFAULT GETDATE(), 
    [LastUpdatedBy] INT NULL, 
)
