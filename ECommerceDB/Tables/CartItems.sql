﻿CREATE TABLE [dbo].[CartItems]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[UserId] INT NOT NULL FOREIGN KEY REFERENCES Users(Id) ON UPDATE CASCADE ON DELETE CASCADE,	
	[ItemId] INT NOT NULL FOREIGN KEY REFERENCES Items(Id) ON UPDATE CASCADE ON DELETE CASCADE,
	[Quantity] INT NOT NULL,
	Unique(UserId,ItemId)
)