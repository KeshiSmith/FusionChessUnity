namespace GameTree
{
    public class GameTreeNodeColor
    {
        public static readonly GameTreeNodeColor Red = new GameTreeNodeColor(true);
        public static readonly GameTreeNodeColor Black = new GameTreeNodeColor(false);

        public bool IsRed { get; set; }
        private GameTreeNodeColor(bool isRed)
        {
            IsRed = isRed;
        }

        public GameTreeNodeColor NotClor()
        {
            return IsRed ? Black : Red;
        }
    }
}
