SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TableStorage')
BEGIN
  CREATE DATABASE TableStorage;
END
GO

USE TableStorage;
GO

IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'Row' AND SCHEMA_NAME(schema_id) = 'dbo')
BEGIN
	CREATE TABLE Row (
		Id int IDENTITY(1,1) PRIMARY KEY,
		tpep_pickup_datetime datetime2(0) NOT NULL,
		tpep_dropoff_datetime datetime2(0) NOT NULL,
		passenger_count int NULL,
		trip_distance float NULL,
		store_and_fwd_flag varchar(10) NULL,
		PULocationID int NULL,
		DOLocationID int NULL,
		fare_amount float  NULL,
		tip_amount float NULL
	);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PULocationId_tip_amount'
    AND object_id = OBJECT_ID('Row'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_PULocationId_tip_amount
	ON Row (PULocationID, tip_amount) 
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PULocationId'
    AND object_id = OBJECT_ID('Row'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_PULocationId
	ON Row (PULocationId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_trip_distance_fare_amount'
    AND object_id = OBJECT_ID('Row'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_trip_distance_fare_amount
	ON Row (trip_distance, fare_amount) 
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tpep_dropoff_datetime_tpep_pickup_datetime_fare_amount'
    AND object_id = OBJECT_ID('Row'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_tpep_dropoff_datetime_tpep_pickup_datetime_fare_amount
	ON Row (tpep_dropoff_datetime, tpep_pickup_datetime, fare_amount) 
END
GO


/* -- Rollback

SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET QUOTED_IDENTIFIER ON;
GO

IF  EXISTS(SELECT * FROM sys.tables WHERE name = 'Row' AND SCHEMA_NAME(schema_id) = 'dbo')
TRUNCATE TABLE Row
GO

DROP INDEX IF EXISTS Row.IX_PULocationId_tip_amount;
GO

DROP INDEX IF EXISTS Row.IX_PULocationId;
GO

DROP INDEX IF EXISTS Row.IX_trip_distance_fare_amount;
GO

DROP INDEX IF EXISTS Row.IX_tpep_dropoff_datetime_tpep_pickup_datetime_fare_amount;
GO

DROP TABLE IF EXISTS Row;
GO

USE master;
GO

ALTER DATABASE TableStorage SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

DROP DATABASE IF EXISTS TableStorage;
GO

--*/