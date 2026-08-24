using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class SyncModels : Migration
    {
        /// <summary>
        /// Sincroniza el modelo con BDs creadas manualmente por scripts SQL.
        /// Cada operación va protegida con IF NOT EXISTS para ser idempotente:
        /// en la BD donde ya se ejecutaron los scripts no hace nada, y en una BD
        /// generada solo por EF crea los objetos que faltan.
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'[dbo].[Incomes]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [Incomes] (
                        [Id] int NOT NULL IDENTITY,
                        [UserId] nvarchar(450) NOT NULL,
                        [Amount] decimal(18,2) NOT NULL,
                        [Category] int NOT NULL,
                        [Date] datetime2 NOT NULL,
                        [Note] nvarchar(500) NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        CONSTRAINT [PK_Incomes] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Incomes_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
                    );
                END;

                IF OBJECT_ID(N'[dbo].[UserBadges]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [UserBadges] (
                        [Id] int NOT NULL IDENTITY,
                        [UserId] nvarchar(450) NOT NULL,
                        [BadgeName] nvarchar(100) NOT NULL,
                        [Description] nvarchar(500) NULL,
                        [EarnedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_UserBadges] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_UserBadges_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
                    );
                END;

                IF OBJECT_ID(N'[dbo].[Incomes]', N'U') IS NOT NULL
                    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_Income_User_Date' AND [object_id] = OBJECT_ID(N'[dbo].[Incomes]'))
                    CREATE INDEX [IX_Income_User_Date] ON [Incomes] ([UserId], [Date]);

                IF OBJECT_ID(N'[dbo].[Incomes]', N'U') IS NOT NULL
                    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_Income_User_Category' AND [object_id] = OBJECT_ID(N'[dbo].[Incomes]'))
                    CREATE INDEX [IX_Income_User_Category] ON [Incomes] ([UserId], [Category]);

                IF OBJECT_ID(N'[dbo].[UserBadges]', N'U') IS NOT NULL
                    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserBadge_User' AND [object_id] = OBJECT_ID(N'[dbo].[UserBadges]'))
                    CREATE INDEX [IX_UserBadge_User] ON [UserBadges] ([UserId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns
                    WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetUsers]') AND [name] = N'DarkModeEnabled')
                    ALTER TABLE [AspNetUsers] ADD [DarkModeEnabled] bit NOT NULL CONSTRAINT [DF_AspNetUsers_DarkModeEnabled] DEFAULT (0);

                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns
                    WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetUsers]') AND [name] = N'RemindersEnabled')
                    ALTER TABLE [AspNetUsers] ADD [RemindersEnabled] bit NOT NULL CONSTRAINT [DF_AspNetUsers_RemindersEnabled] DEFAULT (1);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'[dbo].[UserBadges]', N'U') IS NOT NULL
                    DROP TABLE [UserBadges];

                IF OBJECT_ID(N'[dbo].[Incomes]', N'U') IS NOT NULL
                    DROP TABLE [Incomes];

                IF EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetUsers]') AND [name] = N'RemindersEnabled')
                    ALTER TABLE [AspNetUsers] DROP CONSTRAINT IF EXISTS [DF_AspNetUsers_RemindersEnabled];
                IF EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetUsers]') AND [name] = N'RemindersEnabled')
                    ALTER TABLE [AspNetUsers] DROP COLUMN [RemindersEnabled];

                IF EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetUsers]') AND [name] = N'DarkModeEnabled')
                    ALTER TABLE [AspNetUsers] DROP CONSTRAINT IF EXISTS [DF_AspNetUsers_DarkModeEnabled];
                IF EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetUsers]') AND [name] = N'DarkModeEnabled')
                    ALTER TABLE [AspNetUsers] DROP COLUMN [DarkModeEnabled];
            ");
        }
    }
}
