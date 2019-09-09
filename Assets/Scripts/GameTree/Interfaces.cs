using System.Collections.Generic;

namespace GameTree
{
    public interface IGameTreeData<A> where A: IGameTreeAction
    {
        IEnumerable<GameTreeAction<A>> GetNextActions(GameTreeNodeColor color);
        IGameTreeData<A> GetNextState(GameTreeAction<A> action);
    }

    public interface IGameTreeAction
    {
    }
}
