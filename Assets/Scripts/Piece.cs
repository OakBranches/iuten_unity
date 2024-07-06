using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Piece
{
    static int[] tower1 = {0, 0};
    static int[] tower2 = {0, 0};

    static List<List<Move>> filterOutBoundsAndOccuped(int[,] t, Move[,] moves)
    {
        List<List<Move>> validMoves = new List<List<Move>>();
        for (int i = 0; i < moves.GetLength(0); i++)
        {
            List<Move> validMoveSequence = new List<Move>();
            for (int j = 0; j < moves.GetLength(1); j++)
            {
                bool add;
                if (moves[i, j].x2 >= 0 && moves[i, j].x2 < 9 && moves[i, j].y2 >= 0 && moves[i, j].y2 < 11)
                {
                    add = true;
                }
                else if (getPlayer(moves[i,j].piece) == getPlayer((PieceType)t[moves[i,j].x2, moves[i, j].y2])){
                    add = false;
                }
                else
                {
                    add = false;
                }

                if (add)
                {
                    validMoveSequence.Add(moves[i, j]);
                } else
                {
                    break;
                }

            }
            if (validMoveSequence.Count > 0)
            {
                validMoves.Add(validMoveSequence);
            }
        }

        return validMoves;
    }

    static Player? getPlayer(PieceType piece)
    {
        if ((int) piece < 2)
        {
            return null;
        } else if ((int) piece < 7)
        {
            return Player.PLAYER1;
        } else
        {
            return Player.PLAYER2;
        }
    }

    static List<List<Move>> getMoves(int[,] t, int x, int y)
    {
        return filterOutBoundsAndOccuped(t, getRawMoves(t, x, y));
    }




    static Move[,] getRawMoves(int[,] t, int x, int y)
    {
        Move[] moves = new Move[0];
        bool specialMove = false;
        BaseMove[,] baseMoves ={
                {new BaseMove(x,y, x+1,y)},
                {new BaseMove(x,y, x-1,y)},
                {new BaseMove(x,y, x,y+1)},
                {new BaseMove(x,y, x,y-1)},
                {new BaseMove(x,y, x-1,y-1)},
                {new BaseMove(x,y, x+1,y+1)}
            };

        // Se não for peça
        if (t[x, y] < 2)
        {
            return new Move[0,0];
        }

        // Se estiver na torre
        if ( tower2[0] == x && tower2[1] == y || tower1[0] == x && tower1[1] == y)
        {
            // Se não for arqueiro
            if (t[x, y] != (int)PieceType.ARCHER1 && t[x, y] != (int)PieceType.ARCHER2){
                return BaseMove.toMoves((PieceType)t[x, y], baseMoves);
            }else
            {
                specialMove = true;
            }

        }
 

        switch ((PieceType)t[x, y])
        {
            case PieceType.DRUID1:
            case PieceType.DRUID2:
                return new Move[0,0];
            case PieceType.KNIGHT1:
            case PieceType.KNIGHT2:
                return new Move[0,0];
            case PieceType.ARCHER1:
            case PieceType.ARCHER2:
                return new Move[0,0];
            case PieceType.ELEPHANT1:

            case PieceType.ELEPHANT2:
                
                return new Move[0,0];
            case PieceType.PRINCE1:
            case PieceType.PRINCE2:
                return BaseMove.toMoves((PieceType)t[x, y], baseMoves);
            default:
                return new Move[0,0];
        }

    }
}


public struct BaseMove
{
    public int x;
    public int y;
    public int x2;
    public int y2;
    public bool special;
    public BaseMove(int x, int y, int x2, int y2, bool special = false)
    {
        this.x = x;
        this.y = y;
        this.x2 = x2;
        this.y2 = y2;
        this.special = special;
    }

    public Move toMove(PieceType piece)
    {
        return new Move(this, piece);
    }

    public static Move[,] toMoves(PieceType piece, BaseMove[,]  moves)
    {
        Move[,] newMoves = new Move[moves.GetLength(0), moves.GetLength(1)];
        for (int i = 0; i < moves.GetLength(0); i++)
        {
            for (int j = 0; j < moves.GetLength(1); j++)
            {
                newMoves[i, j] = moves[i, j].toMove(piece);
            }
        }
        return newMoves;
    }

}

public struct Move
{
    public int x;
    public int y;
    public int x2;
    public int y2;
    public PieceType piece;
    public bool special;
    public Move(int x, int y, int x2, int y2, PieceType piece, bool special = false)
    {
        this.x = x;
        this.y = y;
        this.x2 = x2;
        this.y2 = y2;
        this.piece = piece;
        this.special = special;
    }

    public Move(BaseMove move, PieceType piece)
    {
        this.x = move.x;
        this.y = move.y;
        this.x2 = move.x2;
        this.y2 = move.y2;
        this.piece = piece;
        this.special = move.special;
    }
}   

public enum PieceType
{
    OUTOFBOUNDS,
    NONE,
    ARCHER1,
    DRUID1,
    KNIGHT1,
    ELEPHANT1,
    PRINCE1,
    ARCHER2,
    DRUID2,
    KNIGHT2,
    ELEPHANT2,
    PRINCE2
}

public enum Player
{
    PLAYER1,
    PLAYER2
}
public class Table
{

}



