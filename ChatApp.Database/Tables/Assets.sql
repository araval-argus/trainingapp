CREATE TABLE [dbo].[Assets]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1), 
    [ProfileId] INT NOT NULL, 
    [FileName] NVARCHAR(256) NOT NULL, 
    [FileExtension] NCHAR(10) NOT NULL, 
    [FileType] NCHAR(256) NOT NULL, 
    [FileSize] BIGINT NOT NULL, 
    [FilePath] VARCHAR(MAX) NOT NULL, 
    [CreatedAt] DATETIME2 NOT NULL, 
    [UpdatedAt] DATETIME2 NOT NULL, 
    CONSTRAINT [FK_Assets_To_Profiles] FOREIGN KEY (ProfileId) REFERENCES dbo.Profiles(Id)
)
