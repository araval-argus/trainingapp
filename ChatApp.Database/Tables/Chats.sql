CREATE TABLE [dbo].[Chats]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1), 
    [MessageFrom] INT NOT NULL, 
    [MessageTo] INT NOT NULL, 
    [Type] NVARCHAR(50) NOT NULL, 
    [Content] TEXT NOT NULL, 
    [CreatedAt] DATETIME2 NOT NULL, 
    [UpdatedAt] DATETIME2 NULL, 
    [DeletedAt] DATETIME2 NULL,
    CONSTRAINT [FK_Chats_MessageFrom_To_Profiles] FOREIGN KEY (MessageFrom) REFERENCES dbo.Profiles(Id),
    CONSTRAINT [FK_Chats_MessgeTo_To_Profiles] FOREIGN KEY (MessageTo) REFERENCES dbo.Profiles(Id),
)
