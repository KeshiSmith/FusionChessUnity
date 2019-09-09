using FusionChess;
using GameTree;
using System.Collections.Generic;

namespace AI
{
    public class AiTree
    {
        private GameTree<AiData, AiAction> gameTree;

        public AiTree(ChessBoard board, PieceColor color)
        {
            var rootData = new GameTreeNodeData<AiData, AiAction>(new AiData(board));
            var rootColor = color == PieceColor.Red ? GameTreeNodeColor.Red : GameTreeNodeColor.Black;
            gameTree = new GameTree<AiData, AiAction>(rootData, rootColor);
        }
        
        public ChessRecord GetBestRecord(int depth)
        {
            var action = gameTree.GetBestAction(depth);
            return action.action.Record;
        }
    }

    class AiAction : IGameTreeAction
    {
        public ChessRecord Record { get; private set; }

        public AiAction(ChessRecord record)
        {
            Record = record;
        }
    }

    class AiData : IGameTreeData<AiAction>
    {
        public ChessBoard Board { get; private set; }

        public AiData(ChessBoard board)
        {
            Board = board;
        }

        public IEnumerable<GameTreeAction<AiAction>> GetNextActions(GameTreeNodeColor nodeColor)
        {
            var actions = new List<GameTreeAction<AiAction>>();
            var color = nodeColor.IsRed ? PieceColor.Red : PieceColor.Black;
            foreach (var record in Board.GetChessRecords(color))
            {
                if (Board.MovePieceIsOK(record, color))
                {
                    var aiAction = new AiAction(record);
                    // TODO score 计算
                    var score = ValueRule.GetValueOfRecord(record);
                    var action = new GameTreeAction<AiAction>(aiAction, score);
                    actions.Add(action);
                }
            }
            actions.Sort((x, y) =>
            {
                return y.score.CompareTo(x.score);
            });
            return actions;
        }
        public IGameTreeData<AiAction> GetNextState(GameTreeAction<AiAction> action)
        {
            var newBoardData = (PieceInfo[,])Board.BoardPieces.Clone();
            var newBoard = new ChessBoard(newBoardData);
            newBoard.MovePiece(action.action.Record);
            return new AiData(newBoard);
        }
    }
}
