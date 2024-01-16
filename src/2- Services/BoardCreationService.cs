using GameOfLife.API.Requests;
using GameOfLife.Domain.Entities;
using GameOfLife.Infra.EntityFramework;
using GameOfLife.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Services
{
    public class BoardCreationService : IBoardCreationService
    {
        private readonly GameOfLifeDbContext context;
        private readonly ILogger<BoardCreationService> logger;
        public BoardCreationService(
            GameOfLifeDbContext context,
            ILogger<BoardCreationService> logger) 
        {
            this.context = context;
            this.logger = logger;
        }
        public async Task<IResult> CreateBoardAsync(CreateBoardRequest request)
        {
            var newBoard = new Board();

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

            foreach (var cell in request.Seed)
            {
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

            return Results.Ok(boardId);
        }
    }
}
