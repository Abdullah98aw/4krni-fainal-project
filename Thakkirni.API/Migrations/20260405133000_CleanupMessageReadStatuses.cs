using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thakkirni.API.Migrations
{
    public partial class CleanupMessageReadStatuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- Guard: table must exist
IF OBJECT_ID('dbo.MessageReadStatuses','U') IS NULL
BEGIN
    THROW 51000, 'MessageReadStatuses table does not exist; cannot proceed with cleanup.', 1;
END

-- Ensure no NULLs in crucial columns
IF EXISTS (SELECT 1 FROM dbo.MessageReadStatuses WHERE ItemId IS NULL OR UserId IS NULL)
BEGIN
    THROW 50000, 'Cannot proceed: NULL values exist in MessageReadStatuses.ItemId or MessageReadStatuses.UserId. Please fix data before running this migration.', 1;
END

-- Ensure no duplicate (ItemId, UserId) pairs
IF EXISTS (
    SELECT ItemId, UserId
    FROM dbo.MessageReadStatuses
    GROUP BY ItemId, UserId
    HAVING COUNT(*) > 1
)
BEGIN
    THROW 50000, 'Cannot proceed: duplicate (ItemId, UserId) rows exist in MessageReadStatuses. Please deduplicate before running this migration.', 1;
END

-- Drop existing primary key if present
DECLARE @pkname sysname;
SELECT @pkname = kc.name
FROM sys.key_constraints kc
WHERE kc.parent_object_id = OBJECT_ID('dbo.MessageReadStatuses') AND kc.type = 'PK';
IF @pkname IS NOT NULL
BEGIN
    EXEC('ALTER TABLE dbo.MessageReadStatuses DROP CONSTRAINT [' + @pkname + ']');
END

-- Drop legacy Id column if exists
IF COL_LENGTH('dbo.MessageReadStatuses', 'Id') IS NOT NULL
BEGIN
    ALTER TABLE dbo.MessageReadStatuses DROP COLUMN Id;
END

-- Drop legacy MessageId column if exists
IF COL_LENGTH('dbo.MessageReadStatuses', 'MessageId') IS NOT NULL
BEGIN
    ALTER TABLE dbo.MessageReadStatuses DROP COLUMN MessageId;
END

-- Add composite primary key (ItemId, UserId)
IF NOT EXISTS (SELECT 1 FROM sys.key_constraints kc WHERE kc.parent_object_id = OBJECT_ID('dbo.MessageReadStatuses') AND kc.type = 'PK')
BEGIN
    ALTER TABLE dbo.MessageReadStatuses ADD CONSTRAINT PK_MessageReadStatuses PRIMARY KEY (ItemId, UserId);
END

-- Add foreign key to Items if not exists
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = 'FK_MRS_Items')
BEGIN
    ALTER TABLE dbo.MessageReadStatuses ADD CONSTRAINT FK_MRS_Items FOREIGN KEY (ItemId) REFERENCES dbo.Items(Id);
END

-- Add foreign key to Users if not exists
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = 'FK_MRS_Users')
BEGIN
    ALTER TABLE dbo.MessageReadStatuses ADD CONSTRAINT FK_MRS_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id);
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty: cleanup migration is not reversible automatically.
        }
    }
}
