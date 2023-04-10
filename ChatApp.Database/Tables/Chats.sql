CREATE TABLE [dbo].[Chats]
(
    [Id] INT IDENTITY (1, 1) NOT NULL, 
    [From] INT NOT NULL, 
    [To] INT NOT NULL, 
    [Content] NVARCHAR(MAX) NOT NULL, 
    [sentAt] DATETIME NOT NULL, 
    [replyToChat] INT NULL, 
    CONSTRAINT [PK_Chats] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Chats_FromUser] FOREIGN KEY ([From]) REFERENCES dbo.Profiles([Id]),
    CONSTRAINT [FK_Chats_ToUser] FOREIGN KEY ([To]) REFERENCES dbo.Profiles([Id]) 
)
