CREATE TABLE [dbo].[Groups]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(200) NULL, 
    [ProfileImage] NVARCHAR(100) NULL, 
    [Admin] INT NOT NULL, 
    CONSTRAINT [PK_Group] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Group_To_AdminUser] FOREIGN KEY ([Admin]) REFERENCES dbo.Profiles([Id]), 
)
