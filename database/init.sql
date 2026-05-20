-- =============================================
-- Task Management Application
-- Database Initialization Script
-- Run this against your SQL Server / SQL Express
-- =============================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TaskManagementDB')
BEGIN
    CREATE DATABASE TaskManagementDB;
    PRINT 'Database TaskManagementDB created.';
END
ELSE
BEGIN
    PRINT 'Database TaskManagementDB already exists.';
END
GO

USE TaskManagementDB;
GO

-- =============================================
-- Create Users table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type = N'U')
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id]           INT            NOT NULL IDENTITY(1,1),
        [Username]     NVARCHAR(100)  NOT NULL,
        [PasswordHash] NVARCHAR(MAX)  NOT NULL,
        [FullName]     NVARCHAR(150)  NOT NULL,
        [Email]        NVARCHAR(200)  NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Users_Username] UNIQUE ([Username])
    );
    PRINT 'Table Users created.';
END
GO

-- =============================================
-- Create Tasks table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND type = N'U')
BEGIN
    CREATE TABLE [dbo].[Tasks] (
        [Id]          INT            NOT NULL IDENTITY(1,1),
        [Title]       NVARCHAR(200)  NOT NULL,
        [Description] NVARCHAR(1000) NULL,
        -- Status: 0=New, 1=InDevelopment, 2=InTesting, 3=InDeployment, 4=Completed
        [Status]      INT            NOT NULL DEFAULT 0,
        [CreatedAt]   DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt]   DATETIME2      NULL,
        [DueDate]     DATETIME2      NULL,
        [AssigneeId]  INT            NULL,
        CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Tasks_Users] FOREIGN KEY ([AssigneeId])
            REFERENCES [dbo].[Users] ([Id])
            ON DELETE SET NULL
    );
    PRINT 'Table Tasks created.';
END
GO

-- =============================================
-- Seed initial admin user
-- Password: Admin@123  (BCrypt work factor 11)
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'admin')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [PasswordHash], [FullName], [Email])
    VALUES (
        'admin',
        '$2a$11$N5lMeBGvmPnCH7NXLImWEe/4H7qcBdtI1df7HpPaSmKTKRuTqL6fK',
        'System Administrator',
        'admin@taskmanagement.local'
    );
    PRINT 'Admin user seeded. Username: admin | Password: Admin@123';
END
GO

-- =============================================
-- Seed sample users
-- Password for all sample users: Admin@123
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'jdoe')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [PasswordHash], [FullName], [Email])
    VALUES
    ('jdoe',   '$2a$11$N5lMeBGvmPnCH7NXLImWEe/4H7qcBdtI1df7HpPaSmKTKRuTqL6fK', 'John Doe',   'jdoe@example.com'),
    ('jsmith', '$2a$11$N5lMeBGvmPnCH7NXLImWEe/4H7qcBdtI1df7HpPaSmKTKRuTqL6fK', 'Jane Smith', 'jsmith@example.com');
    PRINT 'Sample users seeded.';
END
GO

-- =============================================
-- Seed sample tasks
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Tasks])
BEGIN
    DECLARE @AdminId INT = (SELECT [Id] FROM [dbo].[Users] WHERE [Username] = 'admin');
    DECLARE @JdoeId  INT = (SELECT [Id] FROM [dbo].[Users] WHERE [Username] = 'jdoe');

    INSERT INTO [dbo].[Tasks] ([Title], [Description], [Status], [CreatedAt], [DueDate], [AssigneeId])
    VALUES
    ('Setup project repository',    'Initialize Git repo and project structure.',  0, GETUTCDATE(), DATEADD(DAY,  3, GETUTCDATE()), @AdminId),
    ('Design database schema',      'Create ERD and finalize entity relationships.',1, GETUTCDATE(), DATEADD(DAY,  5, GETUTCDATE()), @AdminId),
    ('Implement user authentication','Build login and registration endpoints.',     1, GETUTCDATE(), DATEADD(DAY,  7, GETUTCDATE()), @JdoeId),
    ('Build task CRUD APIs',        'REST endpoints for task management.',          2, GETUTCDATE(), DATEADD(DAY, 10, GETUTCDATE()), @JdoeId),
    ('Create Angular UI',           'Frontend task list and form components.',      3, GETUTCDATE(), DATEADD(DAY, 14, GETUTCDATE()), NULL),
    ('Write unit tests',            'Cover repositories and controllers.',          0, GETUTCDATE(), DATEADD(DAY, 20, GETUTCDATE()), NULL),
    ('Deploy to staging',           'Package and deploy application.',              4, GETUTCDATE(), DATEADD(DAY, 30, GETUTCDATE()), @AdminId);
    PRINT 'Sample tasks seeded.';
END
GO

PRINT 'Database initialization complete.';
GO
