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

/**
 * Every time a person is added, the population counter in the country decrease.
 */
CREATE TRIGGER delete_person ON Person
AFTER DELETE AS BEGIN
	UPDATE dbo.Country SET population = population - 1
	WHERE id = (SELECT residenceCountry FROM deleted)
END
