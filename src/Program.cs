using GameOfLife.API.Requests;
using GameOfLife.Domain.Entities;
using GameOfLife.Infra.EntityFramework;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<GameOfLifeDbContext>(
    options => options.UseSqlServer("name=ConnectionStrings:GameOfLifeDbConnection"));
//builder.Services.AddLogging(builder => builder.AddConsole());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapPost("/GameOfLife", async (GameOfLifeDbContext context, ILogger<Board> logger, CreateBoardRequest request) =>
{
    var newBoard = new Board(request.Size);

    logger.LogInformation("Starting persistence of new board");
    try
    {
        context.Boards.Add(newBoard);
        await context.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
        logger.LogError(ex, "Error while saving new board to the database.");
        throw;
    }

    var boardId = newBoard.BoardId;
    var newCells = new HashSet<Cell>();

    foreach (var cell in request.Seed) {
        newBoard.LivingCells.Add(new Cell { BoardId = boardId, PositionX = cell.X, PositionY = cell.Y });
    }

    logger.LogInformation("Starting persistence of cells for generation 0 of board {boardId}", boardId);
    try
    {
        context.Cells.AddRange(newBoard.LivingCells);
        await context.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
        logger.LogError(ex, "Error while saving cells for generation 0 of board {boardId} to the database.", boardId);
        throw;
    }

    return boardId;
})
.WithName("StartGameOfLife")
.WithDescription("Set a max board size and seed initial living cells into it. Returns BoardID to be used for other commands.")
.WithOpenApi();

app.MapGet("/GameOfLife:NextGen:{BoardId}", async (GameOfLifeDbContext context, ILogger<Board> logger, int boardId) =>
{
    var board = context.Boards
        .Include(b => b.LivingCells)
        .First(b => b.BoardId == boardId);

    if (board.GameOver)
        return board;

    var oldCells = context.Cells.Where(c => c.BoardId == boardId).ToList();

    board.EvolveNextGeneration();
    var newCells = board.LivingCells.ToList();

    logger.LogInformation("Starting persistence of cells for generation {generation} of board {boardId}", board.Generation, boardId);
    try
    {
        context.Cells.RemoveRange(oldCells);
        await context.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
        logger.LogError(ex, "Error while saving new state for board {boardID} and its cells for generation {generation} to the database.", boardId, board.Generation);
        throw;
    }

    return board;
})
.WithName("EvolveToNextGeneration")
.WithDescription("Evolves a given board to the next generation. Returns living cells for that board in the new state.")
.WithOpenApi();

app.MapGet("/GameOfLife:EvolveGenerations:{Evolutions}:{BoardId}", async (GameOfLifeDbContext context, ILogger<Board> logger, int evolutions, int boardId) =>
{
    var board = context.Boards
        .Include(b => b.LivingCells)
        .First(b => b.BoardId == boardId);

    if (board.GameOver)
        return board;

    var oldCells = context.Cells.Where(c => c.BoardId == boardId);

    for (var i = evolutions; i > 0; i--)
        board.EvolveNextGeneration();

    var newCells = board.LivingCells.ToList();

    logger.LogInformation("Starting persistence of cells for generation {generation} of board {boardId}", board.Generation, boardId);
    try
    {
        context.Cells.RemoveRange(oldCells);
        await context.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
        logger.LogError(ex, "Error while saving new state for board {boardID} and its cells for generation {generation} to the database.", boardId, board.Generation);
        throw;
    }

    return board;
})
.WithName("EvolveByNGenerations")
.WithDescription("Evolves a given board by {evolutions} generations. Returns living cells for that board in the new state.")
.WithOpenApi();

//app.MapGet("/GameOfLife:EvolveUntilResolved:{MaxEvolutions}:{BoardId}", async (GameOfLifeDbContext context, ILogger<Board> logger, int maxEvolutions, int boardId) =>
//{
//    var board = context.Boards
//        .Include(b => b.LivingCells)
//        .First(b => b.BoardId == boardId);

//    if (board.GameOver)
//        return board;

//    var oldCells = context.Cells.Where(c => c.BoardId == boardId);

//    for (var i = maxEvolutions; i > 0; i--)
//    {
//        board.EvolveNextGeneration();
//        if (board.GameOver)
//            break;
//    }

//    var newCells = board.LivingCells.ToList();

//    logger.LogInformation("Starting persistence of cells for generation {generation} of board {boardId}", board.Generation, boardId);
//    try
//    {
//        context.Cells.RemoveRange(oldCells);
//        await context.SaveChangesAsync();
//    }
//    catch (DbUpdateException ex)
//    {
//        logger.LogError(ex, "Error while saving new state for board {boardID} and its cells for generation {generation} to the database.", boardId, board.Generation);
//        throw;
//    }

//    return board.GameOver ? Results.Ok(board) : Results.BadRequest();
//})
//.WithName("EvolveUntilResolved")
//.WithDescription("Evolves a given board by up to {maxEvolutions} generations, until it is resolved. Returns living cells for that board in the final state or an error if it didn't resolve.")
//.WithOpenApi(); WIP


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
