IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Companies] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Departments] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [CompanyId] int NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Departments_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Sections] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [DepartmentId] int NOT NULL,
    CONSTRAINT [PK_Sections] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Sections_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [NationalId] nvarchar(20) NOT NULL,
    [Role] nvarchar(20) NOT NULL,
    [Avatar] nvarchar(max) NOT NULL,
    [DepartmentId] int NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id])
);
GO

CREATE TABLE [Items] (
    [Id] int NOT NULL IDENTITY,
    [ItemNumber] nvarchar(20) NOT NULL,
    [Type] nvarchar(20) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Importance] nvarchar(20) NOT NULL,
    [CommitteeType] nvarchar(20) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [CreatedById] int NOT NULL,
    [DepartmentId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Items] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Items_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Items_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Body] nvarchar(max) NOT NULL,
    [IsRead] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AuditEvents] (
    [Id] int NOT NULL IDENTITY,
    [ItemId] int NOT NULL,
    [Type] nvarchar(50) NOT NULL,
    [UserId] int NOT NULL,
    [MetaData] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AuditEvents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AuditEvents_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [Items] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AuditEvents_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ChatMessages] (
    [Id] int NOT NULL IDENTITY,
    [ItemId] int NOT NULL,
    [UserId] int NOT NULL,
    [Text] nvarchar(max) NOT NULL,
    [PdfAttachmentFileName] nvarchar(max) NOT NULL,
    [PdfAttachmentPath] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ChatMessages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChatMessages_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [Items] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ChatMessages_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ItemAssignees] (
    [Id] int NOT NULL IDENTITY,
    [ItemId] int NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_ItemAssignees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ItemAssignees_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [Items] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ItemAssignees_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ItemMembers] (
    [Id] int NOT NULL IDENTITY,
    [ItemId] int NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_ItemMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ItemMembers_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [Items] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ItemMembers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_AuditEvents_ItemId] ON [AuditEvents] ([ItemId]);
GO

CREATE INDEX [IX_AuditEvents_UserId] ON [AuditEvents] ([UserId]);
GO

CREATE INDEX [IX_ChatMessages_ItemId] ON [ChatMessages] ([ItemId]);
GO

CREATE INDEX [IX_ChatMessages_UserId] ON [ChatMessages] ([UserId]);
GO

CREATE INDEX [IX_Departments_CompanyId] ON [Departments] ([CompanyId]);
GO

CREATE INDEX [IX_ItemAssignees_ItemId] ON [ItemAssignees] ([ItemId]);
GO

CREATE INDEX [IX_ItemAssignees_UserId] ON [ItemAssignees] ([UserId]);
GO

CREATE INDEX [IX_ItemMembers_ItemId] ON [ItemMembers] ([ItemId]);
GO

CREATE INDEX [IX_ItemMembers_UserId] ON [ItemMembers] ([UserId]);
GO

CREATE INDEX [IX_Items_CreatedById] ON [Items] ([CreatedById]);
GO

CREATE INDEX [IX_Items_DepartmentId] ON [Items] ([DepartmentId]);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

CREATE INDEX [IX_Sections_DepartmentId] ON [Sections] ([DepartmentId]);
GO

CREATE INDEX [IX_Users_DepartmentId] ON [Users] ([DepartmentId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260331204742_InitialCreate', N'8.0.4');
GO

COMMIT;
GO

