CREATE TABLE [dbo].[Chats]
(
    [Id] INT IDENTITY (1, 1) NOT NULL, 
    [From] NVARCHAR(50) NOT NULL, 
    [To] NVARCHAR(50) NOT NULL, 
    [Content] NVARCHAR(MAX) NOT NULL, 
    [sentAt] DATETIME NOT NULL
)
