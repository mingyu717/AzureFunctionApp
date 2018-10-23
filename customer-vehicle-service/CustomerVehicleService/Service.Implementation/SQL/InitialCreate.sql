/****** Object:  Table [dbo].[CustomerVehicles]    Script Date: 10/08/2018 2:55:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Customers'))
BEGIN
	CREATE TABLE [dbo].[Customers](
		[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY,
		[CustomerNo] [int] NOT NULL,
		[CustomerEmail] [nvarchar](150) NULL,
		[FirstName] [nvarchar](50) NOT NULL,
		[Surname] [nvarchar](50) NOT NULL,
		[PhoneNumber] [nvarchar](50) NULL,
		[CommunityId] [nvarchar](50) NOT NULL,
		[RooftopId] [nvarchar](50) NOT NULL
	)
	PRINT N'Create table : Customers'; 
END

IF NOT (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CustomerVehicles'))
BEGIN
	CREATE TABLE [dbo].[CustomerVehicles](
		[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY,
		[CustomerId] [int] NOT NULL,
		[VehicleNo] [int] NOT NULL,
		[RegistrationNo] [nvarchar](10) NOT NULL,
		[VinNumber] [nvarchar](50) NOT NULL,
		[MakeCode] [nvarchar](5) NULL,
		[ModelCode] [nvarchar](20) NULL,
		[ModelYear] [nvarchar](10) NULL,
		[ModelDescription] [nvarchar](100) NULL,
		[LastServiceDate] [nvarchar](50) NULL,
		[NextServiceDate] [nvarchar](50) NULL,
		[LastKnownMileage] [int] NULL,
		[NextServiceMileage] [int] NULL,
		[VariantCode] [nvarchar](50) NULL,
		[IsProcessed] [bit] NOT NULL,
		[InvitationTime] [datetime] NULL
	)
	PRINT N'Create table : CustomerVehicles'; 
END

IF NOT (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Invitations'))
BEGIN
	CREATE TABLE [dbo].[Invitations](
		[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY,
		[CustomerId] [int] NOT NULL,
		[DealerId] [int] NOT NULL,
		[Method] [int] NOT NULL,
		[ContactDetail] [nvarchar](150) NULL,
		[Timestamp] [datetime] NULL
	)
	PRINT N'Create table : Invitations'; 
END

IF NOT(EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceBookings'))
BEGIN
CREATE TABLE [dbo].[ServiceBookings](
	[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[CustomerNo] [int] NOT NULL,
	[VehicleNo] [int] NOT NULL,
	[DealerId] [int] NOT NULL,
	[BookingReference] [nvarchar] (50) NOT NULL,
	[CreateTime] [datetime] NOT NULL
    )
    PRINT N'Create table : ServiceBookings';
END