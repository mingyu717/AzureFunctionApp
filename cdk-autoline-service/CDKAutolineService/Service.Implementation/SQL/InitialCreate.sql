SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AppTokens'))
BEGIN
	CREATE TABLE [dbo].[AppTokens](
		[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY,
		[CommunityId] [nvarchar](50) NOT NULL,
		[Token] [uniqueidentifier] NOT NULL
	)
	PRINT N'Create table : AppTokens'; 
END



IF NOT (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CdkCustomer'))
BEGIN
	CREATE TABLE [dbo].CdkCustomer(
		[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY,
		[CommunityId] [nvarchar](50) NOT NULL,
		[CustomerNo] [int] NOT NULL,
		[CustomerLoginId] [nvarchar](150) NOT NULL,
		[Password] [nvarchar](255) NOT NULL,
		[Token] [uniqueidentifier] NULL
	)
	PRINT N'Create table : CdkCustomer'; 
END

IF NOT (EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'DealerCDKConfigurations'))
BEGIN
    CREATE TABLE DealerCDKConfigurations(
        Id Int IDENTITY(1,1) NOT NULL,
        CommunityId NVARCHAR(50) NOT NULL,
        RoofTopId NVARCHAR(50) NOT NULL,
        PartnerId NVARCHAR(50) NOT NULL,
        PartnerKey NVARCHAR(150) NOT NULL,
        PartnerVersion NVARCHAR(50) NOT NULL
    )
    PRINT N'Create table : DealerCDKConfiguratins';
END