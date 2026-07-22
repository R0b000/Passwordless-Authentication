CREATE TABLE [dbo].[Permissions] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Module] NVARCHAR(100) NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Permissions_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Permissions_Name] ON [dbo].[Permissions] ([Name]);
GO

CREATE INDEX [IX_Permissions_Module] ON [dbo].[Permissions] ([Module]);
GO
