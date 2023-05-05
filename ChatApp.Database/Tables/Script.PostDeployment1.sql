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
IF NOT EXISTS(SELECT * FROM Designations)
BEGIN
INSERT INTO Designations (designation) values ('Intern') , ('Probationer'), ('Programmer Analyst'), ('Solution Analyst'), ('Lead Solution Analyst'), ('Group Lead'), ('Group Director'), ('Chief Technical Officer'), ('Chief Executive Officer');
END
