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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE TABLE [UniCare_Roles] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_UniCare_Roles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE TABLE [UniCare_Users] (
        [Id] uniqueidentifier NOT NULL,
        [FullName] nvarchar(100) NOT NULL,
        [UniversityName] nvarchar(150) NULL,
        [FacultyName] nvarchar(150) NULL,
        [ProfilePictureUrl] nvarchar(500) NULL,
        [VerificationStatus] nvarchar(30) NOT NULL,
        [IsVerifiedStudent] bit NOT NULL,
        [VerificationBadgeGrantedAt] datetime2 NULL,
        [RegistrationMethod] nvarchar(20) NOT NULL,
        [GoogleSubjectId] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_UniCare_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE TABLE [UniCare_RoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_UniCare_RoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UniCare_RoleClaims_UniCare_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [UniCare_Roles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE TABLE [UniCare_StudentVerifications] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [DocumentType] nvarchar(30) NOT NULL,
        [DocumentUrl] nvarchar(500) NOT NULL,
        [OcrExtractedName] nvarchar(100) NULL,
        [OcrExtractedUniversity] nvarchar(150) NULL,
        [OcrExtractedFaculty] nvarchar(150) NULL,
        [OcrExpiryDate] datetime2 NULL,
        [OcrRawResponse] nvarchar(max) NULL,
        [SubmittedAt] datetime2 NOT NULL,
        [ReviewedAt] datetime2 NULL,
        [ReviewNotes] nvarchar(500) NULL,
        CONSTRAINT [PK_UniCare_StudentVerifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UniCare_StudentVerifications_UniCare_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [UniCare_Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE TABLE [UniCare_UserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_UniCare_UserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UniCare_UserClaims_UniCare_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [UniCare_Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE TABLE [UniCare_UserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UniCare_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_UniCare_UserLogins_UniCare_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [UniCare_Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE TABLE [UniCare_UserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UniCare_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UniCare_UserRoles_UniCare_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [UniCare_Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UniCare_UserRoles_UniCare_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [UniCare_Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE TABLE [UniCare_UserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_UniCare_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_UniCare_UserTokens_UniCare_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [UniCare_Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE INDEX [IX_UniCare_RoleClaims_RoleId] ON [UniCare_RoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [UniCare_Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UniCare_StudentVerifications_UserId] ON [UniCare_StudentVerifications] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE INDEX [IX_UniCare_UserClaims_UserId] ON [UniCare_UserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE INDEX [IX_UniCare_UserLogins_UserId] ON [UniCare_UserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE INDEX [IX_UniCare_UserRoles_RoleId] ON [UniCare_UserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [UniCare_Users] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [UniCare_Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419163244_authentication'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260419163244_authentication', N'8.0.0');
END;
GO

COMMIT;
GO

