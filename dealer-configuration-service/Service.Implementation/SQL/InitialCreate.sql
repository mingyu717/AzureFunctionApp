SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DealerConfiguration'))
BEGIN
	CREATE TABLE [dbo].[DealerConfiguration](
		[DealerId] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](50) NULL,
		[RooftopId] [nvarchar](50) NOT NULL,
		[CommunityId] [nvarchar](50) NOT NULL,
		[Address] [nvarchar](100) NULL,
		[PhoneNumber] [nvarchar](50) NULL,
		[EmailAddress] [nvarchar](150) NOT NULL,
		[Latitude] [float] NOT NULL,
		[Longitude] [float] NOT NULL,
		[AppThemeName] [nvarchar](50) NULL,
		[CommunicationMethod] [int] NOT NULL,
        [CsvSource] [int] NOT NULL,
		[ShowTransportations] [bit] NOT NULL,
		[ShowAdvisors] [bit] NOT NULL,
		[ShowPrice] [bit] NOT NULL,
		[EmailContent] [nvarchar](MAX) NULL,
		[EmailSubject] [nvarchar](MAX) NULL,
		[SmsContent] [nvarchar](MAX) NULL,
        [MinimumFreeCapacity] [int] NOT NULL,
	 CONSTRAINT [PK_dbo.DealerConfiguration] PRIMARY KEY CLUSTERED 
	(
		[DealerId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
	PRINT N'Create table : DealerConfiguration'; 
END