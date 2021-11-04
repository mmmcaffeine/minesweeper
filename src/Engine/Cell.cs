namespace Dgt.Minesweeper.Engine
{
    public record Cell(Location Location, bool IsMined, bool IsRevealed)
    {
        public bool IsExploded => IsMined && IsRevealed;
    }
}