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
CREATE TABLE [Roles] (
    [RoleID] int NOT NULL IDENTITY,
    [RoleName] nvarchar(50) NOT NULL,
    [Description] nvarchar(200) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([RoleID])
);

CREATE TABLE [Users] (
    [UserID] int NOT NULL IDENTITY,
    [Username] nvarchar(100) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    [FirstName] nvarchar(150) NULL,
    [LastName] nvarchar(150) NULL,
    [Phone] nvarchar(20) NULL,
    [RoleID] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastLogin] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserID]),
    CONSTRAINT [FK_Users_Roles_RoleID] FOREIGN KEY ([RoleID]) REFERENCES [Roles] ([RoleID]) ON DELETE CASCADE
);

CREATE TABLE [Pets] (
    [PetID] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Breed] nvarchar(100) NULL,
    [Species] nvarchar(50) NOT NULL,
    [DateOfBirth] date NULL,
    [Gender] nvarchar(1) NULL,
    [Color] nvarchar(50) NULL,
    [MicrochipID] nvarchar(50) NULL,
    [PhotoPath] nvarchar(500) NULL,
    [OwnerUserID] int NOT NULL,
    [CreatedByUserID] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Pets] PRIMARY KEY ([PetID]),
    CONSTRAINT [FK_Pets_Users_CreatedByUserID] FOREIGN KEY ([CreatedByUserID]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Pets_Users_OwnerUserID] FOREIGN KEY ([OwnerUserID]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION
);

CREATE TABLE [Appointments] (
    [AppointmentID] int NOT NULL IDENTITY,
    [PetID] int NOT NULL,
    [AppointmentDateTime] datetime2 NOT NULL,
    [Reason] nvarchar(300) NULL,
    [Status] int NOT NULL,
    [Notes] nvarchar(max) NULL,
    [BookedByUserID] int NOT NULL,
    [VeterinarianUserID] int NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([AppointmentID]),
    CONSTRAINT [FK_Appointments_Pets_PetID] FOREIGN KEY ([PetID]) REFERENCES [Pets] ([PetID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Appointments_Users_BookedByUserID] FOREIGN KEY ([BookedByUserID]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Appointments_Users_VeterinarianUserID] FOREIGN KEY ([VeterinarianUserID]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION
);

CREATE TABLE [Documents] (
    [DocumentID] int NOT NULL IDENTITY,
    [PetID] int NOT NULL,
    [FileName] nvarchar(255) NOT NULL,
    [FilePath] nvarchar(500) NOT NULL,
    [FileType] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [DocumentType] nvarchar(max) NULL,
    [UploadedByUserID] int NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Documents] PRIMARY KEY ([DocumentID]),
    CONSTRAINT [FK_Documents_Pets_PetID] FOREIGN KEY ([PetID]) REFERENCES [Pets] ([PetID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Documents_Users_UploadedByUserID] FOREIGN KEY ([UploadedByUserID]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION
);

CREATE TABLE [MedicalRecords] (
    [RecordID] int NOT NULL IDENTITY,
    [PetID] int NOT NULL,
    [VisitDate] datetime2 NOT NULL,
    [Diagnosis] nvarchar(500) NULL,
    [Treatment] nvarchar(max) NULL,
    [Notes] nvarchar(max) NULL,
    [Prescriptions] nvarchar(max) NULL,
    [CreatedByUserID] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedByUserID] int NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_MedicalRecords] PRIMARY KEY ([RecordID]),
    CONSTRAINT [FK_MedicalRecords_Pets_PetID] FOREIGN KEY ([PetID]) REFERENCES [Pets] ([PetID]) ON DELETE CASCADE,
    CONSTRAINT [FK_MedicalRecords_Users_CreatedByUserID] FOREIGN KEY ([CreatedByUserID]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MedicalRecords_Users_UpdatedByUserID] FOREIGN KEY ([UpdatedByUserID]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION
);

CREATE TABLE [Vaccinations] (
    [VaccinationID] int NOT NULL IDENTITY,
    [PetID] int NOT NULL,
    [VaccineType] nvarchar(150) NOT NULL,
    [VaccinationDate] date NOT NULL,
    [BatchNumber] nvarchar(100) NULL,
    [NextDueDate] date NULL,
    [AdministeredByUserID] int NOT NULL,
    [Notes] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Vaccinations] PRIMARY KEY ([VaccinationID]),
    CONSTRAINT [FK_Vaccinations_Pets_PetID] FOREIGN KEY ([PetID]) REFERENCES [Pets] ([PetID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Vaccinations_Users_AdministeredByUserID] FOREIGN KEY ([AdministeredByUserID]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Appointments_BookedByUserID] ON [Appointments] ([BookedByUserID]);

CREATE INDEX [IX_Appointments_PetID] ON [Appointments] ([PetID]);

CREATE INDEX [IX_Appointments_VeterinarianUserID] ON [Appointments] ([VeterinarianUserID]);

CREATE INDEX [IX_Documents_PetID] ON [Documents] ([PetID]);

CREATE INDEX [IX_Documents_UploadedByUserID] ON [Documents] ([UploadedByUserID]);

CREATE INDEX [IX_MedicalRecords_CreatedByUserID] ON [MedicalRecords] ([CreatedByUserID]);

CREATE INDEX [IX_MedicalRecords_PetID] ON [MedicalRecords] ([PetID]);

CREATE INDEX [IX_MedicalRecords_UpdatedByUserID] ON [MedicalRecords] ([UpdatedByUserID]);

CREATE INDEX [IX_Pets_CreatedByUserID] ON [Pets] ([CreatedByUserID]);

CREATE INDEX [IX_Pets_OwnerUserID] ON [Pets] ([OwnerUserID]);

CREATE INDEX [IX_Users_RoleID] ON [Users] ([RoleID]);

CREATE INDEX [IX_Vaccinations_AdministeredByUserID] ON [Vaccinations] ([AdministeredByUserID]);

CREATE INDEX [IX_Vaccinations_PetID] ON [Vaccinations] ([PetID]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260706090850_InitialCreate', N'9.0.17');

COMMIT;
GO

