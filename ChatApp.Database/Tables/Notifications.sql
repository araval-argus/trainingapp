CREATE TABLE [dbo].[Notifications]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY, 
    [Type] INT NOT NULL, 
    [RaisedFor] INT NOT NULL, 
    [RaisedBy] INT NOT NULL, 
    [CreatedAt] DATETIME NOT NULL,
    [RaisedInGroup] INT NULL, 
    FOREIGN KEY([Type]) REFERENCES [Notification_Types]([Id]),
    FOREIGN KEY([RaisedFor]) REFERENCES [Profiles]([Id]),
    FOREIGN KEY([RaisedBy]) REFERENCES [Profiles]([Id])
)
