namespace GameOfLife.API.Requests
{
    public record CreateBoardRequest(
        IEnumerable<Seed> Seed
    );

    public record Seed(
        int X,
        int Y
    );
}
