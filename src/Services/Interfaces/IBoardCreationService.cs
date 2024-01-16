using GameOfLife.API.Requests;

namespace GameOfLife.Services.Interfaces
{
    public interface IBoardCreationService
    {
        IResult CreateBoard(CreateBoardRequest request);
    }
}
