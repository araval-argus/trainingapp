CREATE TABLE [dbo].[Notification]
(
    [Id] INT IDENTITY (1, 1) NOT NULL, 
    [Content] NVARCHAR(50) NOT NULL, 
    [Time] DATETIME2 NOT NULL, 
    [User] INT NOT NULL, 
    [isSeen] INT NOT NULL DEFAULT 0, 
    [isGroup] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Notification] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Notification_To_Profile] FOREIGN KEY ([User]) REFERENCES dbo.Profiles(Id),
)
