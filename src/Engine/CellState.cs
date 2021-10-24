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

        public static CellState Uncleared { get; } = new("Uncleared"); // Implies _not_ mined
        public static CellState Mined { get; } = new("Mined"); // Implies _not_ cleared
    }
}