CREATE TABLE [dbo].[Profiles]
(
	[Id] INT NOT NULL PRIMARY KEY ,
	[FirstName] VARCHAR(25) NOT NULL ,
	[LastName] VARCHAR(25) NOT NULL ,
	[UserName] VARCHAR(35) UNIQUE NOT NULL ,
	[ProfileImageAddress] VARCHAR(200) NOT NULL ,

)
