namespace GameTree
{
    public class GameTree<D, A> where D : IGameTreeData<A> where A : IGameTreeAction
    {
        private GameTreeNodeData<D, A> rootData;
        private GameTreeNodeColor rootColor;

        public GameTree(GameTreeNodeData<D,A> rootData, GameTreeNodeColor rootColor)
        {
            this.rootData = rootData;
            this.rootColor = rootColor;
        }

        public GameTreeAction<A> GetBestAction(int depth)
        {
            var rootNote = new GameTreeNode<D, A>(rootData, rootColor);
            return rootNote.GetBestAction(depth);
        }
    }
}
