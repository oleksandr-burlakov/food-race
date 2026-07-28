# AuthModule migration

dotnet ef migrations add 'MigrationName' --context AppIdentityDbContext --project Modules.Authentication --startup-project API -o .\Infrastructure\DB\Migrations
dotnet ef database update --project Modules.Authentication --startup-project API
