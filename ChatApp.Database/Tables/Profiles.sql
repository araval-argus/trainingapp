CREATE TABLE [dbo].[Profiles]
(
    [Id] INT IDENTITY (1, 1) NOT NULL, 
    [FirstName] NVARCHAR(1000) NOT NULL, 
    [LastName] NVARCHAR(1000) NOT NULL, 
    [UserName] NVARCHAR(1000) NULL, 
    [Email ] NVARCHAR(1000) NULL, 
    [Password] NVARCHAR(MAX) NULL, 
    [ProfileType] INT NOT NULL,
    [CreatedAt] DATETIME2 NULL, 
    [CreatedBy] INT NULL, 
    [LastUpdatedAt] DATETIME2 NULL, 
    [LastUpdatedBy] INT NULL, 
    [imagePath] NVARCHAR(1000) NULL, 
    [status] INT NOT NULL DEFAULT 6, 
    [isDeleted] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Profiles] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Profiles_To_Status] FOREIGN KEY ([status]) REFERENCES dbo.Status(Id), 
    CONSTRAINT [FK_Profiles_To_ProfileType] FOREIGN KEY ([ProfileType]) REFERENCES dbo.ProfileType(Id)
)
