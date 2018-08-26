/* Person */
USE Countries
/**
 * Person table
 * Foreign key on residence and birth country in dbo.Country
 */
CREATE TABLE Person
(
	id					INTEGER IDENTITY(1, 1)	CONSTRAINT PK_Person PRIMARY KEY,
	idNumber			INTEGER,
	firstName			VARCHAR(50),
	lastName			VARCHAR(50),
	birthCountry		INTEGER,
	residenceCountry	INTEGER,
	birthdate			DATE,
	email				VARCHAR(60),
	photo				VARBINARY(MAX) DEFAULT(NULL),
	interview			VARBINARY(MAX) DEFAULT(NULL)
)

-- Procedures
go
-- Triggers
/**
 * Every time a person is added, the population counter in the country increase.
 */
CREATE TRIGGER insert_person ON Person
AFTER INSERT AS BEGIN
	UPDATE dbo.Country SET population = population + 1
	WHERE id = (SELECT residenceCountry FROM inserted)
END go

CREATE PROCEDURE [dbo].[InsertPerson]
	@firstname		VARCHAR(30),
	@lastname		VARCHAR(30),
	@birthcountry	INTEGER,
	@birthdate		DATE
AS
BEGIN
	-- Set the idNumber
	DECLARE @idNumber	INTEGER
	SELECT @idNumber = MAX(idNumber) FROM dbo.Person P WHERE P.birthCountry = @birthcountry
	SET @idNumber = @idNumber + 1

	-- Set the email
	DECLARE @email	CHAR(50)
	SET @email = @firstname + @lastname + '@gmail.com'

	INSERT INTO dbo.Person VALUES (@idNumber, @firstname, @lastname, @birthcountry, @birthcountry, @birthdate, @email, NULL, NULL)
END

/**
 * Every time a person is added, the population counter in the country decrease.
 */
CREATE TRIGGER delete_person ON Person
AFTER DELETE AS BEGIN
	UPDATE dbo.Country SET population = population - 1
	WHERE id = (SELECT residenceCountry FROM deleted)
END







ALTER PROCEDURE FullBackupCountries
AS
BEGIN
 BACKUP DATABASE Countries  
 TO DISK = 'C:\bd\Backup\CountriesDataBackup.BAK'  
    WITH FORMAT,  
    MEDIANAME = 'CountriesDataBackup',  
    NAME = 'Full Data Backup of Countries';  
 BACKUP LOG Countries  
 TO DISK = 'C:\bd\Backup\CountriesLogBackup.BAK'  
    WITH FORMAT,  
    MEDIANAME = 'CountriesLogBackup',  
    NAME = 'Full Log Backup of Countries';  
END

GO


USE msdb ;  
GO  
EXEC dbo.sp_add_job  
    @job_name = N'Country Backups' ;  
GO  
EXEC sp_add_jobstep  
    @job_name = N'Country Backups',  
    @step_name = N'Backup Data',  
    @subsystem = N'TSQL',  
    @command = N'EXEC FullBackupCountries',   
    @retry_attempts = 5,  
    @retry_interval = 5 ;  
GO  
EXEC dbo.sp_add_schedule  
    @schedule_name = N'RunOnce',  
    @freq_type = 1,  
    @active_start_time = 183000 ;  
USE msdb ;  
GO  
EXEC sp_attach_schedule  
   @job_name = N'Country Backups',  
   @schedule_name = N'RunOnce';  
GO  
EXEC dbo.sp_add_jobserver  
    @job_name = N'Country Backups';  
GO


 BACKUP LOG Countries  
 TO DISK = 'C:\bd\Backup\CountriesLogBackup.BAK'  
    WITH FORMAT,  
    MEDIANAME = 'CountriesLogBackup',  
    NAME = 'Full Log Backup of Countries'; 

exec dbo.fullbackupcountries

 BACKUP LOG Countries  
 TO DISK = 'C:\bd\Backup\CountriesLogBackup.BAK'  
    WITH FORMAT,  
    MEDIANAME = 'CountriesLogBackup',  
    NAME = 'Full Log Backup of Countries'; 