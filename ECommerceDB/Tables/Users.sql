CREATE TABLE [dbo].[Users]
(
	[Id] INT PRIMARY KEY IDENTITY,
	[FirstName] VARCHAR(24) NOT NULL,
	[LastName] VARCHAR(24) NOT NULL,
	[Email] VARCHAR(24) UNIQUE NOT NULL,
	[EmailConfirmed] BIT,
	[Password] VARCHAR(24) NULL,
	[PhoneNumber] VARCHAR(max) NULL,
	[PhoneNumberConfirmed] BIT,
	[LastSeen] DATETIME2(0),
	[AccessFailedCount] INT,
	[LockOutEnd] DATETIME2(0),
	[IsBlocked] BIT,
	[ImgUrl] VARCHAR(255),
	IsOnline BIT
)
