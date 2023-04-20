CREATE TABLE [dbo].[GroupMessage]
(
    [Id] INT IDENTITY (1, 1) NOT NULL, 
    [SenderId] INT NOT NULL, 
    [GroupID] INT NOT NULL, 
    [Content] NVARCHAR(MAX) NOT NULL, 
    [ReplyMessageID] INT NULL , 
    [Type] NVARCHAR(50) NOT NULL DEFAULT 'text', 
    [Time] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_GroupMessage] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_GroupMessage_To_Profile] FOREIGN KEY ([SenderId]) REFERENCES dbo.Profiles(Id), 
    CONSTRAINT [FK_GroupMessage_To_Group] FOREIGN KEY ([GroupID]) REFERENCES dbo.Groups(Id) 
)
