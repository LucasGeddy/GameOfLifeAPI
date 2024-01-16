using GameOfLife.API.Requests;
using GameOfLife.Services.Interfaces;

namespace GameOfLife.API.Endpoints
{
    public static class BoardEndpointMapping
    {
        public static string BaseRoute = "/GameOfLife";

        public static WebApplication? MapBoardEndpoints(this WebApplication? app)
        {
            app
                .MapBoardCreate()
                .MapEvolveEndpoints();
                
            return app;
        }

        private static WebApplication? MapBoardCreate(this WebApplication? app)
        {
            if (app == null) 
                return null;

            app.MapPost(BaseRoute, async (IBoardCreationService service, CreateBoardRequest request) =>
            {
                return await service.CreateBoardAsync(request);
            })
            .WithName("StartGameOfLife")
            .WithDescription("Seed initial living cells into a new Board. Returns BoardID to be used for other commands.")
            .WithOpenApi();

            return app;
        }

        private static WebApplication? MapEvolveEndpoints(this WebApplication? app)
        {
            if (app == null)
                return null;

            app.MapGet(BaseRoute + ":NextGen:{BoardId}", async (IBoardEvolutionService service, int boardId) =>
            {
                return await service.EvolveToNextGenAsync(boardId);
            })
            .WithName("EvolveToNextGeneration")
            .WithDescription("Evolves a given board to the next generation. Returns living cells for that board in the new state.")
            .WithOpenApi();

            app.MapGet(BaseRoute + ":EvolveGenerations:{Evolutions}:{BoardId}", async (IBoardEvolutionService service, int evolutions, int boardId) =>
            {
                return await service.EvolveByNGenerationsAsync(evolutions, boardId);
            })
            .WithName("EvolveByNGenerations")
            .WithDescription("Evolves a given board by {evolutions} generations. Returns living cells for that board in the new state.")
            .WithOpenApi();

            app.MapGet(BaseRoute + ":EvolveUntilResolved:{MaxEvolutions}:{BoardId}", async (IBoardEvolutionService service, int maxEvolutions, int boardId) =>
            {
                var board = await service.EvolveByNGenerationsAsync(maxEvolutions, boardId);

                return board.GameOver ? Results.Ok(board) : Results.BadRequest();
            })
            .WithName("EvolveUntilResolved")
            .WithDescription("Evolves a given board by up to {maxEvolutions} generations, until it is resolved. Returns living cells for that board in the final state or an error if it didn't resolve.")
            .WithOpenApi();

            return app;
        }
    }
}
