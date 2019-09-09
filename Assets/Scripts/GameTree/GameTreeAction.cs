namespace GameTree
{
    public class GameTreeAction<A> where A : IGameTreeAction
    {
        public A action;
        public int score;

        public GameTreeAction(A action, int score)
        {
            this.action = action;
            this.score = score;
        }
    }
}
