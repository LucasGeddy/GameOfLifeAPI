using GameOfLife.API.Requests;

namespace GameOfLife.Services.Interfaces
{
    public interface IBoardCreationService
    {
        Task<IResult> CreateBoardAsync(CreateBoardRequest request);
    }
}
