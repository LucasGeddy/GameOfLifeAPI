using GameOfLife.Domain.Entities;
using GameOfLife.Infra.EntityFramework;
using GameOfLife.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Services
{
    public class BoardEvolutionService : IBoardEvolutionService
    {
        private readonly GameOfLifeDbContext context;
        private readonly ILogger<BoardEvolutionService> logger;
        public BoardEvolutionService(
            GameOfLifeDbContext context,
            ILogger<BoardEvolutionService> logger) 
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<Board> EvolveByNGenerationsAsync(int evolutions, int boardId)
        {
            return await EvolveAndPersistBoardAsync(evolutions, boardId);
        }

        public async Task<Board> EvolveToNextGenAsync(int boardId)
        {
            return await EvolveAndPersistBoardAsync(1, boardId);
        }

        private async Task<Board> EvolveAndPersistBoardAsync(int evolutions, int boardId)
        {
            var board = context.Boards
                                .Include(b => b.LivingCells)
                                .First(b => b.BoardId == boardId);

            if (board.GameOver)
                return board;

            var oldCells = context.Cells.Where(c => c.BoardId == boardId);

            for (var i = evolutions; i > 0; i--)
            {
                board.EvolveNextGeneration();
                if (board.GameOver)
                    break;
            }

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
        }
    }
}
