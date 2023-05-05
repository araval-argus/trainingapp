CREATE TABLE [dbo].[Groups]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY, 
    [Name] NVARCHAR(1000) NOT NULL, 
    [Description] NVARCHAR(1000) NULL, 
    [GroupIcon] NVARCHAR(1000) NULL, 
    [CreatedAt] DATETIME NOT NULL, 
    [CreatedBy] INT NOT NULL, 
    [LastUpdatedAt] DATETIME NOT NULL, 
    [LastUpdatedBy] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1, 
    FOREIGN KEY([CreatedBy]) REFERENCES [Profiles]([Id])
)
