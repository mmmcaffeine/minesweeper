namespace Dgt.Minesweeper.Engine
{
    // TODO Do we want validation on Hint i.e. it must be positive non-zero?
    public record Cell(Location Location, bool IsMined, bool IsRevealed, int Hint)
    {
        public bool IsExploded => IsMined && IsRevealed;
    }
}