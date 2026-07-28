CREATE TABLE [dbo].[MailLogs] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [ToEmail] NVARCHAR(300) NOT NULL,
    [Subject] NVARCHAR(500) NOT NULL,
    [Body] NVARCHAR(MAX) NOT NULL,
    [OtpCode] NVARCHAR(10) NULL,
    [Status] NVARCHAR(20) NOT NULL CONSTRAINT [DF_MailLogs_Status] DEFAULT ('Sent'),
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_MailLogs_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_MailLogs] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_MailLogs_ToEmail_CreatedAt] ON [dbo].[MailLogs] ([ToEmail], [CreatedAt]);
GO