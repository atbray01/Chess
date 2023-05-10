using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        ChessPiece c;
        int i, j;

        //up
        i = CurrentX - 1;
        j = CurrentY + 1;
        if(CurrentY != 7)
        {
            for(int k = 0; k < 3; k++)
            {
                if(i >= 0 || i < 8)
                {
                    c = BoardManager.Instance.ChessPieces[i, j];
                    if (c == null)
                        r[i, j] = true;
                    else if (isWhite != c.isWhite)
                        r[i, j] = true;
                }

                i++;
            }
        }
        //down
        i = CurrentX - 1;
        j = CurrentY - 1;
        if (CurrentY != 0)
        {
            for (int k = 0; k < 3; k++)
            {
                if (i >= 0 || i < 8)
                {
                    c = BoardManager.Instance.ChessPieces[i, j];
                    if (c == null)
                        r[i, j] = true;
                    else if (isWhite != c.isWhite)
                        r[i, j] = true;
                }

                i++;
            }
        }
        //middle left
        if (CurrentX != 0)
        {
            c = BoardManager.Instance.ChessPieces[CurrentX - 1, CurrentY];
            if (c == null)
                r[CurrentX - 1, CurrentY] = true;
            else if (isWhite != c.isWhite)
                r[CurrentX - 1, CurrentY] = true;
        }
        //middle right
        if (CurrentX != 7)
        {
            c = BoardManager.Instance.ChessPieces[CurrentX + 1, CurrentY];
            if (c == null)
                r[CurrentX + 1, CurrentY] = true;
            else if (isWhite != c.isWhite)
                r[CurrentX + 1, CurrentY] = true;
        }

        return r;
    }

}
