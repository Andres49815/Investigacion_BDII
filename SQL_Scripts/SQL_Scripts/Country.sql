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

-- Triggers
