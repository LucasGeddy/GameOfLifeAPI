namespace GameOfLife.API.Requests
{
    public record CreateBoardRequest(
        int Size,
        IEnumerable<Seed> Seed
    );

    public record Seed(
        int X,
        int Y
    );
}
