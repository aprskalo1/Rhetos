/*DATAMIGRATION B891788E-0BE1-4858-86E9-F47884330F0A*/ -- Change the script's code only if it needs to be executed again.

-- The following lines are generated by: EXEC Rhetos.HelpDataMigration 'Demo', 'E';
EXEC Rhetos.DataMigrationUse 'Demo', 'E', 'ID', 'uniqueidentifier';
EXEC Rhetos.DataMigrationUse 'Demo', 'E', 'A', 'nvarchar(256)';
EXEC Rhetos.DataMigrationUse 'Demo', 'E', 'B', 'nvarchar(256)';
GO

UPDATE
    _Demo.E
SET
    B = A
WHERE
    B IS NULL;

EXEC Rhetos.DataMigrationApplyMultiple 'Demo', 'E', 'ID, B';