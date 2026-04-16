IF COL_LENGTH('dbo.dt311_Invoice', 'FuelPhotoAttId') IS NULL
BEGIN
    ALTER TABLE dbo.dt311_Invoice
    ADD FuelPhotoAttId INT NULL;
END
