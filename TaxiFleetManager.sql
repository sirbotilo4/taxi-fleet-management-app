-- =========================================
-- Created By: Themba Botilo
-- Date Created: 20 June 2026
-- Last Modified By: Themba Botilo
-- Last Modified Date: 20 June 2026
-- =========================================
-- =========================================
-- Taxi Fleet & Revenue Management System
-- =========================================

CREATE DATABASE TaxiFleetManagerDB;
GO

USE TaxiFleetManagerDB;
GO

-- =========================================
-- 1. OwnerProfile
-- =========================================
CREATE TABLE OwnerProfile (
    OwnerId INT IDENTITY(1,1) PRIMARY KEY,
    BusinessName NVARCHAR(150) NOT NULL,
    FullName NVARCHAR(150) NOT NULL,
    ContactNumber NVARCHAR(20) NOT NULL,
    BankName NVARCHAR(100) NULL,
    AccountNumber NVARCHAR(50) NULL,
    BranchCode NVARCHAR(20) NULL
);
GO

-- =========================================
-- 2. Drivers
-- =========================================
CREATE TABLE Drivers (
    DriverId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    ContactNumber NVARCHAR(20) NOT NULL,
    LicenseNumber NVARCHAR(50) NOT NULL,
    HireDate DATE NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active'
);
GO

-- =========================================
-- 3. Vehicles
-- =========================================
CREATE TABLE Vehicles (
    VehicleId INT IDENTITY(1,1) PRIMARY KEY,
    RegistrationNumber NVARCHAR(20) NOT NULL UNIQUE,
    Make NVARCHAR(50) NOT NULL,
    Model NVARCHAR(50) NOT NULL,
    Capacity INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
    DriverId INT NULL, -- 
    CONSTRAINT FK_Vehicles_Drivers FOREIGN KEY (DriverId)
        REFERENCES Drivers(DriverId)
        ON DELETE SET NULL
);
GO

-- =========================================
-- 4. Routes
-- =========================================
CREATE TABLE Routes (
    RouteId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    StartPoint NVARCHAR(100) NOT NULL,
    EndPoint NVARCHAR(100) NOT NULL,
    StandardFare DECIMAL(10,2) NOT NULL,
    RouteType NVARCHAR(20) NOT NULL DEFAULT 'Local',
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- =========================================
-- 5. Trips
-- =========================================
CREATE TABLE Trips (
    TripId INT IDENTITY(1,1) PRIMARY KEY,
    VehicleId INT NOT NULL,
    DriverId INT NOT NULL,
    RouteId INT NOT NULL,
    TripDate DATETIME NOT NULL DEFAULT GETDATE(),
    PassengerCount INT NOT NULL,
    AmountCollected DECIMAL(10,2) NOT NULL,
    PaymentMethod NVARCHAR(20) NOT NULL DEFAULT 'Cash',
    PaymentReference NVARCHAR(100) NULL,
    CONSTRAINT FK_Trips_Vehicles FOREIGN KEY (VehicleId)
        REFERENCES Vehicles(VehicleId)
        ON DELETE CASCADE,
    CONSTRAINT FK_Trips_Drivers FOREIGN KEY (DriverId)
        REFERENCES Drivers(DriverId)
        ON DELETE NO ACTION,
    CONSTRAINT FK_Trips_Routes FOREIGN KEY (RouteId)
        REFERENCES Routes(RouteId)
        ON DELETE NO ACTION
);
GO

-- =========================================
-- 6. Expenses
-- =========================================
CREATE TABLE Expenses (
    ExpenseId INT IDENTITY(1,1) PRIMARY KEY,
    VehicleId INT NOT NULL,
    ExpenseType NVARCHAR(50) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    ExpenseDate DATETIME NOT NULL DEFAULT GETDATE(),
    Description NVARCHAR(255) NULL,
    CONSTRAINT FK_Expenses_Vehicles FOREIGN KEY (VehicleId)
        REFERENCES Vehicles(VehicleId)
        ON DELETE CASCADE
);
GO

-- =========================================
-- Seed: default OwnerProfile row
-- =========================================
INSERT INTO OwnerProfile (BusinessName, FullName, ContactNumber)
VALUES ('My Taxi Business', 'Owner Name', '0000000000');
GO