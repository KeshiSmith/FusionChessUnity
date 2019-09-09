namespace GameTree
{
    public class GameTreeNode<D, A> where D : IGameTreeData<A> where A : IGameTreeAction
    {
        private GameTreeNode<D, A> nodeParent = null;
        private GameTreeAction<A> nodeAction = null;
        private GameTreeNodeData<D, A> nodeData = null;
        private GameTreeNodeColor nodeColor = null;
        private int? nodeThreshold = null;

        public GameTreeNodeData<D, A> NodeData
        {
            get
            {
                if(nodeData == null)
                    nodeData = nodeParent.NodeData.GetNextState(nodeAction);
                return nodeData;
            }
        }
        public GameTreeNodeColor NodeColor
        {
            get
            {
                if (nodeColor == null)
                    nodeColor = nodeParent.NodeColor.NotClor();
                return nodeColor;
            }
        }
        public int CurrentScore
        {
            get
            {
                if (nodeAction == null)
                    return 0;
                return nodeAction.score;
            }
        }
        public int? NodeThreshold
        {
            get
            {
                if (nodeThreshold == null)
                    nodeThreshold = NodeColor.IsRed ? int.MinValue : int.MaxValue;
                return nodeThreshold;
            }
            set
            {
                nodeThreshold = value;
            }
        }

        public GameTreeNode(GameTreeNodeData<D, A> data, GameTreeNodeColor color)
        {
            nodeData = data;
            nodeColor = color;
        }
        private GameTreeNode(GameTreeNode<D,A> parent, GameTreeAction<A> action)
        {
            nodeParent = parent;
            nodeAction = action;
        }

        public GameTreeAction<A> GetBestAction(int depth)
        {
            if (depth != 0)
            {
                GameTreeAction<A> bestAction = null;
                var nextDepth = depth - 1;
                foreach (var nextAction in NodeData.GetNextActions(NodeColor))
                {
                    nextAction.score = CurrentScore + (NodeColor.IsRed ? nextAction.score : -nextAction.score);
                    var nextNode = new GameTreeNode<D, A>(this, nextAction);
                    var bestNextAction = nextNode.GetBestAction(nextDepth);
                    if (TestAndUpdateThreshold(bestNextAction.score))
                    {
                        bestAction = bestNextAction;
                        if (NeedsCutTreeNode())
                            break;
                    }
                }
                if (nodeParent == null)
                    return bestAction;
                if(bestAction != null)
                    nodeAction.score = bestAction.score;
            }
            return nodeAction;
        }

        private bool TestThreshold(int? threshold)
        {
            if (NodeColor.IsRed)
                return threshold > NodeThreshold;
            return threshold < NodeThreshold;
        }
        private bool TestAndUpdateThreshold(int? threshold)
        {
            if(TestThreshold(threshold))
            {
                NodeThreshold = threshold;
                return true;
            }
            return false;
        }
        private bool NeedsCutTreeNode()
        {
            if (nodeParent == null)
                return false;
            return !nodeParent.TestThreshold(NodeThreshold);
        }
    }
}
