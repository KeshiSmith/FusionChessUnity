using System.Collections.Generic;

namespace GameTree
{
    public class GameTreeNodeData<D, A> where D : IGameTreeData<A> where A : IGameTreeAction
    {
        private D state;

        public GameTreeNodeData(D state)
        {
            this.state = state;
        }

        public IEnumerable<GameTreeAction<A>> GetNextActions(GameTreeNodeColor color)
        {
           return state.GetNextActions(color);
        }
        public GameTreeNodeData<D, A> GetNextState(GameTreeAction<A> action)
        {
            return new GameTreeNodeData<D, A>((D)state.GetNextState(action));
        }
    }
}
