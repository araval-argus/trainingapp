CREATE TABLE [dbo].[GeneralMessages]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Role] NVARCHAR(100) NOT NULL, 
    [Desc] NVARCHAR(100) NOT NULL, 
    [PayPerHour] INT NULL DEFAULT NULL, 
    [AreaId] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [MessageFrom] int NOT NULL,
    CONSTRAINT [FK_GeneralMessages_AreaId_To_Areas] FOREIGN KEY (AreaId) REFERENCES dbo.Areas(Id), 
    CONSTRAINT [FK_GeneralMessages_MessageFrom_To_Profiles] FOREIGN KEY (MessageFrom) REFERENCES dbo.Profiles(Id),
)


