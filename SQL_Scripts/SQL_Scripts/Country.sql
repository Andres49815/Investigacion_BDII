USE Countries

/* Countries */

/**
 * Country table
 * Foreign key on presidenteID in dbo.Person
 */
CREATE TABLE Country
(
	id			INTEGER IDENTITY(1, 1)	CONSTRAINT PK_country PRIMARY KEY,
	name		VARCHAR(60)				CONSTRAINT NN_country_name NOT NULL,
	area		NUMERIC(11, 3)			CONSTRAINT NN_country_area NOT NULL,
	population	NUMERIC(12)				CONSTRAINT NN_country_population NOT NULL,
	flag		VARBINARY(MAX)			DEFAULT(NULL),
	anthem		VARBINARY(MAX)			DEFAULT(NULL)
)

-- Procedures
go
/**
 * Update Population
 * Recalculate the population for each country
 */
ALTER PROCEDURE dbo.updatePopulation AS
BEGIN
	UPDATE dbo.Country SET population = 0
	-- Cursor to travel the Person Table
	DECLARE		peopleCursor	CURSOR LOCAL FOR
	(
		SELECT id, residenceCountry
		FROM dbo.Person
	)
	OPEN		peopleCursor

	-- Variables for the cursor
	DECLARE		@id INTEGER
	DECLARE		@residence INTEGER

	FETCH NEXT FROM peopleCursor INTO @id, @residence

	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		UPDATE Country SET population = population + 1 WHERE id = @residence
		FETCH NEXT FROM peopleCursor INTO @id, @residence
	END

	CLOSE peopleCursor
	DEALLOCATE peopleCursor
END

go

/**
 * Set up presidents in order to assign president from the same nationality and with a 31
 * years and up
 */
ALTER PROCEDURE dbo.setPresidents AS
BEGIN
	-- Counter Regulator
	DECLARE	@counter		INTEGER
	SELECT	@counter = COUNT(*) FROM dbo.Country

	-- Travel Country table
	WHILE (@counter > 0)
	BEGIN
		UPDATE Country SET presidentID =
		(
			SELECT TOP 1 id
			FROM dbo.Person
			WHERE (birthCountry = @counter) AND DATEDIFF(YEAR, birthdate, GETDATE()) > 31
		)
		WHERE id = @counter
		SET @counter = @counter - 1
	END
END

GO

CREATE PROCEDURE [dbo].SelectCountry
	@index	INTEGER
AS
BEGIN
	-- Index Off set
	IF (@index > (SELECT COUNT(*) FROM dbo.Country))
	BEGIN
		SET @index = 1
	END
	IF (@index < 1)
	BEGIN
		SELECT @index = COUNT(*) FROM dbo.Country
	END

	-- SELECT
	SELECT cntry.id, cntry.name, cntry.area, cntry.population, cntry.flag, cntry.anthem, cntry.presidentID, @index AS idx
	FROM
	(
		SELECT TOP (@index) *
		FROM dbo.Country
	) cntry LEFT OUTER JOIN
	(
		SELECT TOP (@index - 1) *
		FROM dbo.Country
	) offset ON (cntry.id = offset.id)
	WHERE offset.id IS NULL
END
SELECT 45 % 10


GO
drop procedure SelectPeople
CREATE PROCEDURE SelectPeople
	@country	INTEGER,
	@start		INTEGER
AS
BEGIN
	DECLARE @count INTEGER
	SELECT @count = COUNT(*) FROM dbo.Person WHERE residenceCountry = @country
	IF (@start * 10 > @count)
	BEGIN
		SET @start = 0
	END
	IF (@start < 0)
	BEGIN
		SET @start = (@count - (@count % 10)) / 10
	END

	SELECT P1.id, P1.idNumber, P1.firstName, P1.lastName, P1.birthCountry, P1.residenceCountry, P1.birthdate, P1.email, P1.photo, P1.interview, @start AS idx
	FROM
	(
		SELECT TOP (@start * 10 + 10) *
		FROM dbo.Person
		WHERE residenceCountry = @country
		ORDER BY lastName ASC
	) P1 LEFT OUTER JOIN
	(
		SELECT TOP (@start * 10) *
		FROM dbo.Person
		WHERE residenceCountry = @country
		ORDER BY lastName ASC
	) offset ON (P1.id = offset.id)
	WHERE offset.id IS NULL
END

exec dbo.SelectPeople @country = 1, @start = 0
select count(*) from dbo.Person where residenceCountry = 1
exec dbo.SelectCountry @index = 1

select * from dbo.Country C INNER JOIN Person P ON(C.presidentID = P.id) WHERE presidentID = 153