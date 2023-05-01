CREATE TABLE [dbo].[Salt]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [UserId] INT NOT NULL, 
    [HashSalt] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [FK_Salt_To_Profiles] FOREIGN KEY ([UserId]) REFERENCES dbo.Profiles([Id])
)
