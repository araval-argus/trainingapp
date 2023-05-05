CREATE TABLE [dbo].[Profiles]
(
    [Id] INT IDENTITY (1, 1) PRIMARY KEY, 
    [FirstName] NVARCHAR(1000) NOT NULL, 
    [LastName] NVARCHAR(1000) NOT NULL, 
    [UserName] NVARCHAR(1000) NULL, 
    [Email ] NVARCHAR(1000) NULL, 
    [Password] NVARCHAR(MAX) NULL, 
    [ProfileType] INT NOT NULL,
    [CreatedAt] DATETIME NULL, 
    [CreatedBy] INT NULL, 
    [LastUpdatedAt] DATETIME NULL, 
    [LastUpdatedBy] INT NULL, 
    [ImageUrl] NVARCHAR(1000) NULL,
    [DesignationID] INT,
    [IsActive] BIT NOT NULL DEFAULT 1, 
    FOREIGN KEY ([DesignationID]) REFERENCES [Designations]([id])
)
