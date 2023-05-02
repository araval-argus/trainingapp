CREATE TABLE [dbo].[Notifications]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [FromId] INT NOT NULL, 
    [ToId] INT NOT NULL, 
    [GroupId] INT NULL, 
    [Content] NVARCHAR(50) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL, 
    CONSTRAINT [FK_Notifications_FromId_To_Profiles] FOREIGN KEY (FromId) REFERENCES dbo.PRofiles(Id),
    CONSTRAINT [FK_Notifications_ToId_To_Profiles] FOREIGN KEY (ToId) REFERENCES dbo.Profiles(Id),
    CONSTRAINT [FK_Notifications_GroupID_To_Groups] FOREIGN KEY (GroupId) REFERENCES dbo.Groups(Id),
)
