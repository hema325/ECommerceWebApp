﻿/*
Deployment script for ECommerceDB

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "ECommerceDB"
:setvar DefaultFilePrefix "ECommerceDB"
:setvar DefaultDataPath "C:\Users\hp\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\"
:setvar DefaultLogPath "C:\Users\hp\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Dropping Foreign Key unnamed constraint on [dbo].[OrderItems]...';


GO
ALTER TABLE [dbo].[OrderItems] DROP CONSTRAINT [FK__OrderItem__Order__0E04126B];


GO
PRINT N'Dropping Foreign Key unnamed constraint on [dbo].[OrderItems]...';


GO
ALTER TABLE [dbo].[OrderItems] DROP CONSTRAINT [FK__OrderItem__ItemI__0EF836A4];


GO
PRINT N'Starting rebuilding table [dbo].[OrderItems]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_OrderItems] (
    [Id]       INT             IDENTITY (1, 1) NOT NULL,
    [OrderId]  INT             NOT NULL,
    [ItemId]   INT             NOT NULL,
    [Price]    DECIMAL (10, 2) NOT NULL,
    [Quantity] INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([OrderId] ASC, [ItemId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[OrderItems])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_OrderItems] ([OrderId], [ItemId], [Price], [Quantity])
        SELECT [OrderId],
               [ItemId],
               [Price],
               [Quantity]
        FROM   [dbo].[OrderItems];
    END

DROP TABLE [dbo].[OrderItems];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_OrderItems]', N'OrderItems';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating Foreign Key unnamed constraint on [dbo].[OrderItems]...';


GO
ALTER TABLE [dbo].[OrderItems] WITH NOCHECK
    ADD FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating Foreign Key unnamed constraint on [dbo].[OrderItems]...';


GO
ALTER TABLE [dbo].[OrderItems] WITH NOCHECK
    ADD FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating Foreign Key unnamed constraint on [dbo].[Categories]...';


GO
ALTER TABLE [dbo].[Categories] WITH NOCHECK
    ADD FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Categories] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating Foreign Key unnamed constraint on [dbo].[ItemOptions]...';


GO
ALTER TABLE [dbo].[ItemOptions] WITH NOCHECK
    ADD FOREIGN KEY ([OptionId]) REFERENCES [dbo].[Options] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating Foreign Key unnamed constraint on [dbo].[Messages]...';


GO
ALTER TABLE [dbo].[Messages] WITH NOCHECK
    ADD FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[Conversations] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating Foreign Key unnamed constraint on [dbo].[Products]...';


GO
ALTER TABLE [dbo].[Products] WITH NOCHECK
    ADD FOREIGN KEY ([DiscountId]) REFERENCES [dbo].[Discounts] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE;


GO
PRINT N'Creating Foreign Key unnamed constraint on [dbo].[Tokens]...';


GO
ALTER TABLE [dbo].[Tokens] WITH NOCHECK
    ADD FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating Foreign Key unnamed constraint on [dbo].[UserConversations]...';


GO
ALTER TABLE [dbo].[UserConversations] WITH NOCHECK
    ADD FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating Foreign Key unnamed constraint on [dbo].[UserConversations]...';


GO
ALTER TABLE [dbo].[UserConversations] WITH NOCHECK
    ADD FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[Conversations] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating Foreign Key unnamed constraint on [dbo].[UserRoles]...';


GO
ALTER TABLE [dbo].[UserRoles] WITH NOCHECK
    ADD FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating Foreign Key unnamed constraint on [dbo].[UserRoles]...';


GO
ALTER TABLE [dbo].[UserRoles] WITH NOCHECK
    ADD FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
CREATE TABLE [#__checkStatus] (
    id           INT            IDENTITY (1, 1) PRIMARY KEY CLUSTERED,
    [Schema]     NVARCHAR (256),
    [Table]      NVARCHAR (256),
    [Constraint] NVARCHAR (256)
);

SET NOCOUNT ON;

DECLARE tableconstraintnames CURSOR LOCAL FORWARD_ONLY
    FOR SELECT SCHEMA_NAME([schema_id]),
               OBJECT_NAME([parent_object_id]),
               [name],
               0
        FROM   [sys].[objects]
        WHERE  [parent_object_id] IN (OBJECT_ID(N'dbo.OrderItems'), OBJECT_ID(N'dbo.Categories'), OBJECT_ID(N'dbo.ItemOptions'), OBJECT_ID(N'dbo.Messages'), OBJECT_ID(N'dbo.Products'), OBJECT_ID(N'dbo.Tokens'), OBJECT_ID(N'dbo.UserConversations'), OBJECT_ID(N'dbo.UserRoles'))
               AND [type] IN (N'F', N'C')
                   AND [object_id] IN (SELECT [object_id]
                                       FROM   [sys].[check_constraints]
                                       WHERE  [is_not_trusted] <> 0
                                              AND [is_disabled] = 0
                                       UNION
                                       SELECT [object_id]
                                       FROM   [sys].[foreign_keys]
                                       WHERE  [is_not_trusted] <> 0
                                              AND [is_disabled] = 0);

DECLARE @schemaname AS NVARCHAR (256);

DECLARE @tablename AS NVARCHAR (256);

DECLARE @checkname AS NVARCHAR (256);

DECLARE @is_not_trusted AS INT;

DECLARE @statement AS NVARCHAR (1024);

BEGIN TRY
    OPEN tableconstraintnames;
    FETCH tableconstraintnames INTO @schemaname, @tablename, @checkname, @is_not_trusted;
    WHILE @@fetch_status = 0
        BEGIN
            PRINT N'Checking constraint: ' + @checkname + N' [' + @schemaname + N'].[' + @tablename + N']';
            SET @statement = N'ALTER TABLE [' + @schemaname + N'].[' + @tablename + N'] WITH ' + CASE @is_not_trusted WHEN 0 THEN N'CHECK' ELSE N'NOCHECK' END + N' CHECK CONSTRAINT [' + @checkname + N']';
            BEGIN TRY
                EXECUTE [sp_executesql] @statement;
            END TRY
            BEGIN CATCH
                INSERT  [#__checkStatus] ([Schema], [Table], [Constraint])
                VALUES                  (@schemaname, @tablename, @checkname);
            END CATCH
            FETCH tableconstraintnames INTO @schemaname, @tablename, @checkname, @is_not_trusted;
        END
END TRY
BEGIN CATCH
    PRINT ERROR_MESSAGE();
END CATCH

IF CURSOR_STATUS(N'LOCAL', N'tableconstraintnames') >= 0
    CLOSE tableconstraintnames;

IF CURSOR_STATUS(N'LOCAL', N'tableconstraintnames') = -1
    DEALLOCATE tableconstraintnames;

SELECT N'Constraint verification failed:' + [Schema] + N'.' + [Table] + N',' + [Constraint]
FROM   [#__checkStatus];

IF @@ROWCOUNT > 0
    BEGIN
        DROP TABLE [#__checkStatus];
        RAISERROR (N'An error occurred while verifying constraints', 16, 127);
    END

SET NOCOUNT OFF;

DROP TABLE [#__checkStatus];


GO
PRINT N'Update complete.';


GO
