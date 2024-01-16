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

### POST - /GameOfLife
Seed initial living cells into a new Board. Returns BoardID to be used for other commands.

Example body:
```json
{
  "seed": [
    {
      "x": 0,
      "y": 0
    },
    {
      "x": 0,
      "y": 1
    },
    {
      "x": 1,
      "y": 0
    },
    {
      "x": 1,
      "y": 1
    }
  ]
}
```

### GET - /GameOfLife:NextGen:{BoardId}
Evolves a given board to the next generation. Returns living cells for that board in the new state.

Send boardId after the last : in the URL. You will receive as a response the full data of the new state of the board.

### GET - /GameOfLife:EvolveGenerations:{Evolutions}:{BoardId}
Evolves a given board by {evolutions} generations. Returns living cells for that board in the new state.

Send the number of {Evolutions} you want, and the {BoardId} of the board you're evolving. You will receive as a response the full data of the new state of the board.

### GET - /GameOfLife:EvolveUntilResolved:{MaxEvolutions}:{BoardId}
Evolves a given board by up to {maxEvolutions} generations, until it is resolved. Returns living cells for that board in the final state or an error if it didn't resolve.

Send the number of {MaxEvolutions} you want to try until the board is resolved, and the {BoardId} of the board you're resolving. 
If the board resolves within the limit, you will receive as a response the full data of the resolved board. Otherwise, returns BadRequest