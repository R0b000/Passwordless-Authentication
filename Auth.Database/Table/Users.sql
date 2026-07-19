CREATE TABLE [dbo].[Users] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Username] NVARCHAR(200) NOT NULL,
    [Email] NVARCHAR(300) NULL,
    [PasswordHash] NVARCHAR(200) NULL,
    [Phone] NVARCHAR(50) NULL,
    [Bio] NVARCHAR(500) NULL,
    [AvatarUrl] NVARCHAR(500) NULL,
    [Timezone] NVARCHAR(100) NULL DEFAULT ('UTC'),
    [Language] NVARCHAR(10) NULL DEFAULT ('en'),
    [EmailPreferences] BIT NULL DEFAULT (1),
    [EmailNotifications] BIT NULL DEFAULT (1),
    [PushNotifications] BIT NULL DEFAULT (0),
    [SmsAlerts] BIT NULL DEFAULT (0),
    [MarketingEmails] BIT NULL DEFAULT (0),
    [ProfileVisibility] NVARCHAR(50) NULL DEFAULT ('private'),
    [DataSharing] BIT NULL DEFAULT (0),
    [ThirdPartyConnections] BIT NULL DEFAULT (1),
    [CookiePreferences] NVARCHAR(50) NULL DEFAULT ('essential'),
    [TwoFactorEnabled] BIT NULL DEFAULT (0),
    [TwoFactorMethod] NVARCHAR(50) NULL DEFAULT ('authenticator'),
    [AlertOnNewDevice] BIT NULL DEFAULT (1),
    [RequirePasswordForSensitive] BIT NULL DEFAULT (1),
    [AccountStatus] NVARCHAR(50) NULL DEFAULT ('active'),
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Users_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Users_UpdatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Users_Username] ON [dbo].[Users] ([Username]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]) WHERE [Email] IS NOT NULL;
GO
