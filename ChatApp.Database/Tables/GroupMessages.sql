CREATE TABLE [dbo].[GroupMessages]
(
    [Id] INT PRIMARY KEY IDENTITY (1, 1) NOT NULL, 
    [GrpId] INT NOT NULL,
    [Content] NVARCHAR(1000) NOT NULL, 
    [MessageFrom] INT NOT NULL, 
    [CreatedAt] DATETIME2 NOT NULL, 
    [RepliedTo] INT NOT NULL DEFAULT -1, 
    [Type] NVARCHAR(30) NULL ,  
    CONSTRAINT [FK_GroupMessages_GrpId_To_Groups] FOREIGN KEY (GrpId) REFERENCES dbo.Groups(Id),
)
