Add a new EF Core migration.

If the migration name is not provided in the args, ask the user for it.

Run from `/Users/goktugibolar/dev/document-intelligence`:
```
dotnet ef migrations add {MigrationName} \
  --project src/Infrastructure \
  --startup-project src/Web.Api \
  --output-dir Database/Migrations
```

After creating the migration, show the generated Up/Down methods so the user can verify the schema changes are correct.
