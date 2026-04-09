using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thakkirni.API.Migrations
{
    public partial class FixMessageReadStatuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID('dbo.MessageReadStatuses','U') IS NULL
BEGIN
    -- Table does not exist: create it safely
    CREATE TABLE dbo.MessageReadStatuses (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        MessageId INT NOT NULL,
        UserId INT NOT NULL,
        ItemId INT NULL,
        ReadAt DATETIME2 NOT NULL CONSTRAINT DF_MessageReadStatuses_ReadAt DEFAULT (GETUTCDATE())
    );
END
ELSE
BEGIN
    -- Ensure ItemId column exists
    IF COL_LENGTH('dbo.MessageReadStatuses','ItemId') IS NULL
    BEGIN
        ALTER TABLE dbo.MessageReadStatuses ADD ItemId INT NULL;
    END

    -- Ensure ReadAt column exists (nullable first) and default constraint for future inserts
    IF COL_LENGTH('dbo.MessageReadStatuses','ReadAt') IS NULL
    BEGIN
        ALTER TABLE dbo.MessageReadStatuses ADD ReadAt DATETIME2 NULL;
        IF NOT EXISTS (
            SELECT 1 FROM sys.default_constraints dc
            JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
            WHERE OBJECT_NAME(dc.parent_object_id) = 'MessageReadStatuses' AND c.name = 'ReadAt')
        BEGIN
            ALTER TABLE dbo.MessageReadStatuses ADD CONSTRAINT DF_MessageReadStatuses_ReadAt DEFAULT (GETUTCDATE()) FOR ReadAt;
        END
    END

    -- Migrate MessageId -> ItemId when MessageId exists
    IF COL_LENGTH('dbo.MessageReadStatuses','MessageId') IS NOT NULL
    BEGIN
        UPDATE mrs
        SET mrs.ItemId = cm.ItemId
        FROM dbo.MessageReadStatuses mrs
        INNER JOIN dbo.ChatMessages cm ON mrs.MessageId = cm.Id
        WHERE mrs.ItemId IS NULL AND mrs.MessageId IS NOT NULL;
    END

    -- Populate ReadAt for existing rows
    UPDATE dbo.MessageReadStatuses SET ReadAt = GETUTCDATE() WHERE ReadAt IS NULL;

    -- Make ReadAt NOT NULL if currently allows NULL
    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID('dbo.MessageReadStatuses') AND name = 'ReadAt' AND is_nullable = 1
    )
    BEGIN
        ALTER TABLE dbo.MessageReadStatuses ALTER COLUMN ReadAt DATETIME2 NOT NULL;
    END
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}