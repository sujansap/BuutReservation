# Rise - GENT2

## Team Members

- Bram Rampelberg - [MEMBER1_EMAIL] - [MEMBER1_GITHUB_USERNAME]
- Xan Pinson - <xan.pinson@student.hogent.be> - Snowyxa
- Pushwant Sagoo - [MEMBER3_EMAIL] - [MEMBER3_GITHUB_USERNAME]
- Sujan Sapkota - <sujan.sapkota@student.hogent.be> - sujansapkota2
- Simon De Roeve - <simon.deroeve@student.hogent.be> - SimonDeRoeve
- Bas Stokmans - <bas.stokmans@student.hogent.be> - baziniser
- Bindo Thorpe - <bindo.thorpe@student.hogent.be> - bindothorpe

## Technologies & Packages Used

- [Postgres 16.4](https://www.postgresql.org/) - Database
- [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) - Frontend
- [ASP.NET 8](https://dotnet.microsoft.com/en-us/apps/aspnet) - Backend
- [Entity Framework 8](https://learn.microsoft.com/en-us/ef/) - Database Access
- [EntityFrameworkCore Triggered](https://github.com/koenbeuk/EntityFrameworkCore.Triggered) - Database Triggers
- [Npgsql Entity Framework Core](https://www.npgsql.org/index.html)
- [User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) - Securely store secrets in DEV.
- [GuardClauses](https://github.com/ardalis/GuardClauses) - Validation Helper
- [Playwright](https://playwright.dev/dotnet/) - E2E tests
- [xUnit](https://xunit.net) - (Unit) Testing
- [nSubstitute](https://nsubstitute.github.io) - Mocking for testing
- [Shouldly](https://docs.shouldly.org) - Helper for testing
- [AspNetCore MVC testing](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Testing) - Integration testing

## Running the application

### Installation Instructions

> Note: Initially the database will not exist, so you will need to run the migrations to [create the database](#creation-of-the-database).

1. Clone the repository
2. Open the `Rise.sln` file in Visual Studio or Visual Studio Code
3. Set up the database connection
4. Run the project using the `Rise.Server` project as the startup project: `dotnet run --project Rise.Server`
5. The project should open in your default browser on port 5001.

### Running In Production

To run the application in the `production` environment, add the following argument to the `dotnet run` command:

```bash
--environment Production
```

For changing the urls the server is running can be specified via the argument `--urls` like so:

```bash
--urls "https://localhost:5100;http://localhost:5200"
```

So the resulting `run` command would be something along the lines of:

```bash
dotnet run --project Rise.Server --environment Production --urls "https://0.0.0.0:5100"
```

For more info on running the application in a specifying environment, check out the ASP.NET docs on [Using multiple environments in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-8.0) and the general [`dotnet run`](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-run) commando.

## Database

### Database connection

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

### Creation of the database

To create the database, run the following command in the main folder `Rise`

```bash
dotnet ef database update --startup-project Rise.Server --project Rise.Persistence
```

> Make sure your database configuration is [correctly setup](#database-connection).

Note: if you have troubles with EF, don't forget to install it via `dotnet tool install --global dotnet-ef`

### Migrations

#### Adding Migrations

Adapting the database schema can be done using migrations. To create a new migration, run the following command:

```bash
dotnet ef migrations add [MIGRATION_NAME] --startup-project Rise.Server --project Rise.Persistence
```

And then update the database to the latest migration using the following command:

```bash
dotnet ef database update --startup-project Rise.Server --project Rise.Persistence
```

#### Removing Migrations

Updating to a specific migration using:

```bash
dotnet ef database update [MIGRATION_NAME] --startup-project Rise.Server --project Rise.Persistence
```

Now the latest migration can be removed via:

```bash
dotnet ef migrations remove --startup-project Rise.Server --project Rise.Persistence
```

## Testing

| Type of test  | Project  | Reason        | Framework        | Additional setup        |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| Unit          | `Rise.Domain.Tests`          | Testing the domain | [xUnit](https://xunit.net) | None |
| Integration   | `Rise.Server.Tests`   | Testing the back-end | [AspNetCore MVC testing](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Testing) | [Test Postgres database set-up](#test-database-setup) |
| E2E  | `Rise.Client.Tests`  | Testing the front-end | [Playwright](https://playwright.dev/dotnet/) | [Playwright must be installed](https://playwright.dev/dotnet/docs/intro) and that application must be [fully up and running](#installation-instructions) |

Additional tools used to help write tests:

- [nSubstitute](https://nsubstitute.github.io) - Mocking for testing
- [Shouldly](https://docs.shouldly.org) - Helper for testing (asserts)

To test globally with everything correctly setup use the following command in the project root:

```bash
dotnet test
```

!! Be sure that !!

1) [Application is fully running](#running-the-application)
2) [Test database is setup](#test-database-setup)
3) [Playwright is fully installed](https://playwright.dev/dotnet/docs/intro)
4) The Test database is ***NOT*** the production database is it will be dropped!!!

If you want to test only a specific part of type, change the working directory to the preferred project and run the aforementioned test command earlier.

### Test database setup

This setup is very similar to the setup of the [application's database](#database-connection). Only difference is that the secrets need to be added to the `Rise.Server.Tests` project. Preferably with the database being `Hogent.Rise.Test` to make a distinction. It is important that the database is different from the application database is this will be ***dropped*** and re-created automatically during tests to ensure the correct state!!
