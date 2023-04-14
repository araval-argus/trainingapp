CREATE TABLE [dbo].[Messages]
(
    [Id] INT PRIMARY KEY IDENTITY (1, 1) NOT NULL, 
    [Content] NVARCHAR(1000) NOT NULL, 
    [MessageFrom] INT NOT NULL, 
    [MessageTo] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL, 
    [RepliedTo] INT NOT NULL DEFAULT -1,  
)