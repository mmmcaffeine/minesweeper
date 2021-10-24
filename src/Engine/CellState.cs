namespace Dgt.Minesweeper.Engine
{
    public record CellState
    {
        public CellState(string name)
        {
            Name = name;
        }

        // Nobody is using this but we have to have _something_ on this type so two instances can be properly
        // checked for equality
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Name { get; }

        public static CellState Uncleared { get; } = new("Uncleared"); // Implies no presence of a mine
        public static CellState Mined { get; } = new("Mined"); // Implies presence of a mine
        public static CellState Cleared { get; } = new("Cleared"); // Implies no presence of a mine
        public static CellState Exploded { get; } = new("Exploded"); // Implies presence of a mine
    }
}