namespace Dgt.Minesweeper.Engine
{
    // TODO Do we want validation on Hint i.e. it must be positive non-zero?
    public record Cell(Location Location, bool IsMined, int Hint)
    {
        public bool IsRevealed { get; init; } = false;

        public bool IsFlagged { get; init; } = false;
        
        public bool IsExploded => IsMined && IsRevealed;
        
        
    }
}