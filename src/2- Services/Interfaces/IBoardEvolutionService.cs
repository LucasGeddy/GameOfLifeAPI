using GameOfLife.Domain.Entities;

namespace GameOfLife.Services.Interfaces
{
    public interface IBoardEvolutionService
    {
        Task<Board> EvolveToNextGenAsync(int boardId);
        Task<Board> EvolveByNGenerationsAsync(int evolutions, int boardId);
    }
}
