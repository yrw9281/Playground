# Cheat sheet

## Flyway

### CLI

```bash
./flyway-11.1.0/flyway -url="jdbc:sqlserver://localhost;databaseName=db_versioning_flyway;encrypt=false" -user="sa" -password="Passw0rd!" -schemas="dbo" -locations="filesystem:./src/sql" info

./flyway-11.1.0/flyway -url="jdbc:sqlserver://localhost;databaseName=db_versioning_flyway;encrypt=false" -user="sa" -password="Passw0rd!" -schemas="dbo" -locations="filesystem:./src/sql" migrate
 
./flyway-11.1.0/flyway -url="jdbc:sqlserver://localhost;databaseName=db_versioning_flyway;encrypt=false" -user="sa" -password="Passw0rd!" -schemas="dbo" -locations="filesystem:./src/sql" validate
```

### Docker

```bash
docker run --rm -v "$PWD/src/sql:/flyway/sql" redgate/flyway:latest -url="jdbc:sqlserver://host.docker.internal;databaseName=db_versioning_flyway;encrypt=false" -user="sa" -password="Passw0rd!" -schemas="dbo" -locations="filesystem:/flyway/sql" migrate
```

## Liquibase

```shell
docker run --rm -v "$PWD/src/sql:/liquibase/changelog" liquibase/liquibase:latest --url="jdbc:sqlserver://host.docker.internal;databaseName=db_versioning_liquibase;encrypt=false" --username="sa" --password="Passw0rd!" --searchPath="/liquibase/changelog" --changeLogFile="changelog.xml" update
```

## EF Core

### Create Project

```shell
dotnet new console
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### Initialize

```shell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Update specific version

```shell
dotnet ef database update InitialCreate
```

### Remove

```shell
dotnet ef database update 0
```
