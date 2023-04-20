CREATE TABLE [dbo].[GroupMember]
(    
    [Id] INT IDENTITY (1, 1) NOT NULL, 
	[GroupId] INT NOT NULL , 
    [MemberId] INT NOT NULL, 
    [AddedDate] DATETIME2 NOT NULL, 
    CONSTRAINT [FK_GroupMember_ToGroup] FOREIGN KEY ([GroupId]) REFERENCES dbo.Groups, 
    CONSTRAINT [FK_GroupMember_ToUser] FOREIGN KEY ([MemberId]) REFERENCES dbo.Profiles, 
    CONSTRAINT [PK_GroupMember] PRIMARY KEY ([Id])
)
