# GameOfLifeAPI
An API for creating and evolving Conway's Game of Life boards.

## Projet setup
This project requires an SQL Server instance to run. If you have Docker, just run the command below:
```docker
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=<YourStrong@Passw0rd>' -p 1433:1433 --name sql_server_instance -d mcr.microsoft.com/mssql/server:latest
```

Then open the project in Visual Studio and using the Package Manager Console, run `Update-Database`. This will create the database and tables needed.

Now you can run the API and use Swagger or Postman/Insomnia to send requests. See the detailed endpoints below.

## Endpoints

WIP