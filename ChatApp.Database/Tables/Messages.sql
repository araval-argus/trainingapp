CREATE TABLE [dbo].[Messages]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY, 
    [Message] VARCHAR(MAX) NOT NULL, 
    [SenderID] INT NOT NULL, 
    [RecieverID] INT NOT NULL, 
    [CreatedAt] DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, 
    [isSeen] BIT NOT NULL DEFAULT 0,
    FOREIGN KEY(SenderID) REFERENCES [Profiles]([Id]),
    FOREIGN KEY(RecieverID) REFERENCES [Profiles]([Id])
)
