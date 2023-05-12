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

IF NOT EXISTS(SELECT * FROM UserStatus)
BEGIN
INSERT INTO UserStatus (Status) values ('Available'), ('Do Not Disturb'), ('Busy'), ('Be Right Back'), ('Away'), ('Offline');
END


IF NOT EXISTS(SELECT * FROM Designation)
BEGIN
INSERT INTO Designation (Position) values ('Intern'), ('Probationer'), ('Programmer Analyst'), ('Solution Analyst'), ('Group Lead'), ('AVP'), ('VP'), ('CTO'), ('CEO');
END


IF NOT EXISTS(SELECT * FROM Profiles)
BEGIN
INSERT INTO Profiles (FirstName,LastName,UserName,Email,Password,ProfileType,ImagePath,Designation,Status,IsDeleted) values ('Dipesh','Kumar','dipeshk','dipesh@gmail.com','password',1,'/images/default.png',9,6,0);
END