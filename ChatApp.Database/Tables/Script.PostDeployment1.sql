/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
IF NOT EXISTS(SELECT * FROM [Designations])
BEGIN
INSERT INTO [Designations] (designation) values ('Intern') , ('Probationer'), ('Programmer Analyst'), ('Solution Analyst'), ('Lead Solution Analyst'), ('Group Lead'), ('Group Director'), ('Chief Technical Officer'), ('Chief Executive Officer');
END


IF NOT EXISTS(SELECT * FROM [Notification_Types])
BEGIN
INSERT INTO [Notification_Types] ([Type]) values 
('text messsage'),
('image shared'),
('video shared'), 
('Audio Shared'),
('group text message'),
('image shared in the group'),
('video shared in the group'),
('audio shared in the group'),
('group member left'),
('group member removed'),
('group member added'),
('new admin')
END
