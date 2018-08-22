/* Person */
USE Countries
/**
 * Person table
 * Foreign key on residence and birth country in dbo.Country
 */
CREATE TABLE Person
(
	id					INTEGER IDENTITY(1, 1)	CONSTRAINT PK_Person			PRIMARY KEY,
	idNumber			INTEGER					CONSTRAINT NN_person_id_number	NOT NULL,
	firstName			VARCHAR(50)				CONSTRAINT NN_Person_name		NOT NULL,
	lastName			VARCHAR(50)				CONSTRAINT NN_Person_lastName	NOT NULL,
	birthCountry		INTEGER					CONSTRAINT FK_PC_birth			REFERENCES Country(id),
	residenceCountry	INTEGER					CONSTRAINT FK_PC_residence		REFERENCES Country(id),
	birthdate			DATE					CONSTRAINT NN_Person_birthdate	NOT NULL,
	email				VARCHAR(60)				CONSTRAINT NN_Person_email		NOT NULL,
	photo				VARBINARY(MAX)			DEFAULT(NULL),
	interview			VARBINARY(MAX)			DEFAULT(NULL)
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
drop procedure dbo.InsertPerson
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

begin transaction
exec dbo.InsertPerson @firstname = 'Este es', @lastname = 'Chumi', @birthcountry = 1, @birthdate = '2000-01-01'

select * from dbo.Person Where residenceCountry = 1
commit
/**
 * Every time a person is added, the population counter in the country decrease.
 */
CREATE TRIGGER delete_person ON Person
AFTER DELETE AS BEGIN
	UPDATE dbo.Country SET population = population - 1
	WHERE id = (SELECT residenceCountry FROM deleted)
END

select * from dbo.Person where birthCountry = 1
select implicit_transactions from sys.databases where name = 'Country'

SET implicit_commit OFF

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SET TRANSACTION ISOLATION LEVEL REPEATABLE READ