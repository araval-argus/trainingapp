CREATE TABLE [dbo].[GroupMembers]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY, 
    [ProfileID] INT NOT NULL, 
    [GroupID] INT NOT NULL, 
    [IsAdmin] BIT NOT NULL DEFAULT 0,
    [JoinedAt] DATETIME NULL, 
    FOREIGN KEY([ProfileID]) REFERENCES [Profiles]([Id]),
    FOREIGN KEY([GroupID]) REFERENCES [Groups]([Id])
)
