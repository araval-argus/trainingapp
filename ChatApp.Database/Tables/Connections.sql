CREATE TABLE [dbo].[Connections]
(
	[Id] INT IDENTITY (1, 1) NOT NULL , 
    [ConnectionId] NVARCHAR(100) NOT NULL, 
    [User] INT NOT NULL, 
    [loginTime] DATETIME NOT NULL, 
    CONSTRAINT [PK_Connections] PRIMARY KEY ([Id])
)
