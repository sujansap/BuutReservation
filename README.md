# Rise - GENT2

## Team Members


- Bram Rampelberg - [MEMBER1_EMAIL] - [MEMBER1_GITHUB_USERNAME]
- Xan Pinson - xan.pinson@student.hogent.be - Snowyxa
- Pushwant Sagoo - [MEMBER3_EMAIL] - [MEMBER3_GITHUB_USERNAME]
- Sujan Sapkota - sujan.sapkota@student.hogent.be - sujansapkota2
- Simon De Roeve - simon.deroeve@student.hogent.be - SimonDeRoeve
- Bas Stokmans - [MEMBER6_EMAIL] - [MEMBER6_GITHUB_USERNAME]
- Bindo Thorpe - bindo.thorpe@student.hogent.be - bindothorpe

## Technologies & Packages Used
- [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) - Frontend
- [ASP.NET 8](https://dotnet.microsoft.com/en-us/apps/aspnet) - Backend
- [Entity Framework 8](https://learn.microsoft.com/en-us/ef/) - Database Access
- [EntityFrameworkCore Triggered](https://github.com/koenbeuk/EntityFrameworkCore.Triggered) - Database Triggers
- [User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) - Securely store secrets in DEV.
- [GuardClauses](https://github.com/ardalis/GuardClauses) - Validation Helper
- [Playwright](https://playwright.dev/dotnet/) - E2E tests
- [xUnit](https://xunit.net) - (Unit) Testing
- [nSubstitute](https://nsubstitute.github.io) - Mocking for testing
- [Shouldly](https://docs.shouldly.org) - Helper for testing

## Installation Instructions
1. Clone the repository
2. Open the `Rise.sln` file in Visual Studio or Visual Studio Code
3. Set up the database connection
4. Run the project using the `Rise.Server` project as the startup project
5. The project should open in your default browser on port 5001.
6. Initially the database will not exist, so you will need to run the migrations to [create the database](#creation-of-the-database).

## Database connection

Add the database connection string as a secret in the `Rise.Server` project via [.NET core User secrets](https://marketplace.visualstudio.com/items?itemName=adrianwilczynski.user-secrets) extension, by right-click on the `Rise.Server.csproj` and selecting `Manage User Secrets`.
Add in given values and alter where needed in the connection string:
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "User ID=[USER];Password=[PASSWORD];Host=localhost;Port=5432;Database=Hogent.Rise;Connection Lifetime=0;"
  }
}
```
Alternative you could achieve the same via the CLI. Be present in the `Rise.Server` project and again alter values where needed in the connection string:
```Bash
dotnet user-secrets set ConnectionStrings:PostgreSQL "User ID=[USER];Password=[PASSWORD];Host=localhost;Port=5432;Database=Hogent.Rise;Connection Lifetime=0;"
```

## Creation of the database
To create the database, run the following command in the main folder `Rise`
```bash
dotnet ef database update --startup-project Rise.Server --project Rise.Persistence
```
> Make sure your connection string is correct in the `Rise/Server/appsettings.json` file.

## Migrations
Adapting the database schema can be done using migrations. To create a new migration, run the following command:
```bash
dotnet ef migrations add [MIGRATION_NAME] --startup-project Rise.Server --project Rise.Persistence
```
And then update the database using the following command:
```bash
dotnet ef database update --startup-project Rise.Server --project Rise.Persistence
```
## Testing
### E2E testing via playwright
Install via the [instructions](https://playwright.dev/dotnet/docs/intro) on playwright docs
To test the E2E tests, be sure that the project is running.