namespace GameOfLife.Domain.Entities
{
    public class Board
    {
        public int BoardId { get; set; }
        public List<Cell> LivingCells { get; set; } = new();
        public HashSet<(int, int)> LivingCellsCoords => LivingCells.Select(c => (c.PositionX, c.PositionY)).ToHashSet();
        public int Size { get; init; }
        public int Generation { get; set; } = 0;
        public bool GameOver { get; set; } = false;

        public Board(int size)
        {
            Size = size;
        }

        public void EvolveNextGeneration()
        {
            if (GameOver) return;

            var newLiveCells = new HashSet<(int, int)>();

            foreach (var cell in LivingCellsCoords)
            {
                var neighbors = CountAliveNeighbors(cell);

                if (neighbors == 2 || neighbors == 3)
                {
                    newLiveCells.Add(cell); // Cell survives
                }

                CheckNeighborBirths(newLiveCells, cell);
            }

            GameOver = newLiveCells.Count == 0 || LivingCellsCoords.SetEquals(newLiveCells);
            LivingCells = newLiveCells.Select(c => new Cell { PositionX = c.Item1, PositionY = c.Item2, BoardId = BoardId }).ToList();
            Generation++;
            
        }

        private int CountAliveNeighbors((int, int) cell)
        {
            var count = 0;
            var (x, y) = cell;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var neighborX = x + i;
                    var neighborY = y + j;

                    if (i == 0 && j == 0)
                        continue;

                    if (LivingCellsCoords.Contains((neighborX, neighborY)))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void CheckNeighborBirths(HashSet<(int,int)> newLiveCells, (int,int) cell)
        {
            var (x, y) = cell;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    
                    var potentialBirth = (x + i, y + j);

                    if (!LivingCellsCoords.Contains(potentialBirth) 
                        && CountAliveNeighbors(potentialBirth) == 3)
                        newLiveCells.Add(potentialBirth);
                }
            }
        }
    }
}
