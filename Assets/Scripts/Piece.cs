using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MoveRules : MonoBehaviour
{
    public InternalState state = new InternalState(InternalState.initialTable);

    public void movePiece(Move move)
    {
        movePiece(move, state);
    }

    public static void movePiece(Move move, InternalState state)
    {
        if (getPlayer(move.piece) != state.currentPlayer)
        {
            return;
        }

        state.table[move.x2, move.y2] = state.table[move.x, move.y];
        state.table[move.x, move.y] = 0;
        state.nextPlayer();
    }


    static int[] tower1 = { 4, 3 };
    static int[] tower2 = { 4, 7 };

    static List<List<Move>> filterOutBoundsAndOccupied(InternalState state, List<List<Move>> moves)
    {
        int[,] t = state.table;
        List<List<Move>> validMoves = new List<List<Move>>();
        for (int i = 0; i < moves.Count; i++)
        {
            List<Move> validMoveSequence = new List<Move>();
            for (int j = 0; j < moves[i].Count; j++)
            {
                // 1 continua, 0 para antes dessa casa, 2 para nessa casa
                int add;
                if (moves[i][j].x2 >= 0 && moves[i][j].x2 < 9 && moves[i][j].y2 >= 0 && moves[i][j].y2 < 11)
                {
                    add = 1;
                }
                else if (getPlayer(moves[i][j].piece) == getPlayer((PieceType)t[moves[i][j].x2, moves[i][j].y2]))
                {
                    add = 0;

                // encontrou inimigo
                } else if (getPlayer((PieceType)t[moves[i][j].x2, moves[i][j].y2]) != null)
                {
                    add = 2;
                    // para no inimigo
                    // todo considerar caso de tiro no elefante
                } else if (moves[i][j].x2==tower1[0] && moves[i][j].y2 == tower1[1] ||
                    moves[i][j].x2 == tower2[0] && moves[i][j].y2 == tower2[1])
                {
                    //corta movimento quando bate na torre
                    add = 2;
                }
                else
                {
                    add = 0;
                }

                if (add != 0)
                {
                    validMoveSequence.Add(moves[i][j]);
                }

                if (add != 1)
                {
                    break;
                }

            }
            validMoves.Add(validMoveSequence);
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

    static List<List<Move>> getMoves(InternalState state, int x, int y)
    {
        return filterOutBoundsAndOccupied(state, getRawMoves(state.table, x, y));
    }


    static List<List<BaseMove>> laserBeam(int x, int y, int ox, int oy, int t)
    {   
        List<BaseMove> moves = new List<BaseMove>();
        for (int i=1; i<=t; i++)
        {
            moves.Add(new BaseMove(x, y, x + i*ox, y + i*oy));
        }
        return new List<List<BaseMove>>{moves};
    }



    static List<List<Move>> getRawMoves(int[,] t, int x, int y)
    {
        bool specialMove = false;


        List<List<BaseMove>> elephant1Moves = new List<List<BaseMove>>
        {
            new List<BaseMove> { new BaseMove(x, y, x, y + 1) },
            new List<BaseMove> { new BaseMove(x, y, x + 1, y + 1) },
            new List<BaseMove> { new BaseMove(x, y, x - 1, y + 1) },
        };

        List<List<BaseMove>> elephant2Moves = new List<List<BaseMove>>
        {
            new List<BaseMove> { new BaseMove(x, y, x, y - 1) },
            new List<BaseMove> { new BaseMove(x, y, x - 1, y - 1) },
            new List<BaseMove> { new BaseMove(x, y, x + 1, y - 1) },
        };  

        List<List<BaseMove>> lateralMoves = new List<List<BaseMove>>
        {
            new List<BaseMove> { new BaseMove(x, y, x + 1, y) },
            new List<BaseMove> { new BaseMove(x, y, x - 1, y) },
        };

        List<List<BaseMove>> baseMoves = new List<List<BaseMove>>();
        baseMoves.AddRange(elephant1Moves);
        baseMoves.AddRange(elephant2Moves);
        baseMoves.AddRange(lateralMoves);


        // Se não for peça
        if (t[x, y] < 2)
        {
            return new List<List<Move>>();
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
        List<List<Move>> moves = new List<List<Move>>();

        switch ((PieceType)t[x, y])
        {
            case PieceType.DRUID1:
            case PieceType.DRUID2:
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], laserBeam(x, y, 1, 0, 2)));
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], laserBeam(x, y, -1, 0, 2)));
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], laserBeam(x, y, 0, 1, 2)));
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], laserBeam(x, y, 0, -1, 2)));
                return moves;
            case PieceType.KNIGHT1:
            case PieceType.KNIGHT2:
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], laserBeam(x, y, 1, 0, 10)));
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], laserBeam(x, y, -1, 0, 10)));
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], laserBeam(x, y, 0, 1, 8)));
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], laserBeam(x, y, 0, -1, 8)));
                return moves;
            case PieceType.ARCHER1:
            case PieceType.ARCHER2:
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], lateralMoves));
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], laserBeam(x, y, 1, 1, 2)));
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], laserBeam(x, y, -1, -1, 2)));
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], laserBeam(x, y, -1, 1, 2)));
                moves.AddRange(BaseMove.toMoves((PieceType)t[x, y], laserBeam(x, y, 1, -1, 2)));
                return moves;
            case PieceType.ELEPHANT1:
                return BaseMove.toMoves((PieceType)t[x, y], elephant1Moves);
            case PieceType.ELEPHANT2:
                return BaseMove.toMoves((PieceType)t[x, y], elephant2Moves);
            case PieceType.PRINCE1:
            case PieceType.PRINCE2:
                return BaseMove.toMoves((PieceType)t[x, y], baseMoves);
            default:
                return moves;
        }

    }
}


public class InternalState
{
    public int elephant1Buffer;
    public int elephant2Buffer;
    public int knightEspecial;
    public Player currentPlayer;
    public Player? winner;
    public Move? lastMove;
    public static int[,] initialTable = new int[9, 11]
    {
        { 6, 2, 0, 0, 0, 0, 0, 0, 0, 7, 11},
        { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11},
        { 3, 4, 0, 0, 0, 0, 0, 0, 0, 9, 8},
        { 6, 6, 0, 0, 0, 0, 0, 0, 0, 11, 11},
        { 0, 5, 0, 0, 0, 0, 0, 0, 0, 10, 0},
        { 6, 6, 0, 0, 0, 0, 0, 0, 0, 11, 11},
        { 3, 4, 0, 0, 0, 0, 0, 0, 0, 9, 8},
        { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11},
        { 6, 2, 0, 0, 0, 0, 0, 0, 0, 7, 11}
    };

    public InternalState(
        int[,] table,
        int elephant1Buffer = 0,
        int elephant2Buffer = 0,
        int knightEspecial = 0,
        Player currentPlayer = Player.PLAYER1,
        Player? winner = null)
    {
        this.elephant1Buffer = elephant1Buffer;
        this.elephant2Buffer = elephant2Buffer;
        this.knightEspecial = knightEspecial;
        this.currentPlayer = currentPlayer;
        this.winner = winner;
        this.table = table;
    }
    public int[,] table;

    public void reset()
    {
        setTable(initialTable);
        currentPlayer = Player.PLAYER1;
        winner = null;
        elephant1Buffer = 0;
        elephant2Buffer = 0;
        knightEspecial = 0;

    }

    public void setTable(int[,] table)
    {
        this.table = table;
    }

    public void nextPlayer()
    {
        if (knightEspecial == 2)
        {
            knightEspecial = 1;
            return;
        }
        if (currentPlayer == Player.PLAYER1)
        {
            currentPlayer = Player.PLAYER2;
        } else
        {
            currentPlayer = Player.PLAYER1;
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

    public static List<List<Move>> toMoves(PieceType piece, List<List<BaseMove>>  moves)
    {
        List<List<Move>> newMoves = new List<List<Move>>();

        for (int i = 0; i < moves.Count; i++)
        {
            for (int j = 0; j < moves[i].Count; j++)
            {
                newMoves[i][j] = moves[i][j].toMove(piece);
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
    NONE,
    OUTOFBOUNDS,
    ARCHER1,
    DRUID1,
    ELEPHANT1,
    KNIGHT1,
    PRINCE1,
    ARCHER2,
    DRUID2,
    ELEPHANT2,
    KNIGHT2,
    PRINCE2
}

public enum Player
{
    PLAYER1,
    PLAYER2
}