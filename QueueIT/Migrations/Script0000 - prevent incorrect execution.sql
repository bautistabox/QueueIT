if OBJECT_ID(N'MyTable') IS NOT NULL
BEGIN
	throw 51000, 'Baseline migrations are only valid in a fresh database, but it was detected that other migrations have already run.  You must either (1) reset the database or (2) manually insert script names into the SchemaVersions table to skip them.', 1
END