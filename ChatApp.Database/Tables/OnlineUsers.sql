CREATE TABLE [dbo].[OnlineUsers]
(
	[Id] INT IDENTITY(1,1), 
    [Username] VARCHAR(1000) NOT NULL, 
    [ConnectionId] VARCHAR(1000) NOT NULL
)
