using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMove(ref ChessPiece[,] board, int tileCount){
        List<Vector2Int> r = new List<Vector2Int>();

        //Right
        if(CurrentX + 1 < tileCount){
            if(board[CurrentX + 1, CurrentY] == null) r.Add(new Vector2Int(CurrentX + 1, CurrentY));
            else if (board[CurrentX + 1, CurrentY].Team != Team) r.Add(new Vector2Int(CurrentX + 1, CurrentY));
            //Top right
            if(CurrentY + 1 < tileCount){
                if(board[CurrentX + 1, CurrentY + 1] == null) r.Add(new Vector2Int(CurrentX + 1, CurrentY + 1));
            else if (board[CurrentX + 1, CurrentY + 1].Team != Team) r.Add(new Vector2Int(CurrentX + 1, CurrentY + 1));
            }
            //Bottom right
            if(CurrentY - 1 >= 0){
                if(board[CurrentX + 1, CurrentY - 1] == null) r.Add(new Vector2Int(CurrentX + 1, CurrentY - 1));
                else if (board[CurrentX + 1, CurrentY - 1].Team != Team) r.Add(new Vector2Int(CurrentX + 1, CurrentY - 1));
            }
        }
        //Left
        if(CurrentX + 1 >= 0){
            if(board[CurrentX - 1, CurrentY] == null) r.Add(new Vector2Int(CurrentX - 1, CurrentY));
            else if (board[CurrentX - 1, CurrentY].Team != Team) r.Add(new Vector2Int(CurrentX - 1, CurrentY));
            //Top right
            if(CurrentY - 1 < tileCount){
                if(board[CurrentX - 1, CurrentY + 1] == null) r.Add(new Vector2Int(CurrentX - 1, CurrentY + 1));
            else if (board[CurrentX - 1, CurrentY + 1].Team != Team) r.Add(new Vector2Int(CurrentX - 1, CurrentY + 1));
            }
            //Bottom right
            if(CurrentY - 1 >= 0){
                if(board[CurrentX - 1, CurrentY - 1] == null) r.Add(new Vector2Int(CurrentX - 1, CurrentY - 1));
                else if (board[CurrentX - 1, CurrentY - 1].Team != Team) r.Add(new Vector2Int(CurrentX - 1, CurrentY - 1));
            }
        }

        //Up
        if(CurrentY + 1 < tileCount){
            if(board[CurrentX, CurrentY + 1] == null || board[CurrentX, CurrentY + 1].Team != Team) r.Add(new Vector2Int(CurrentX, CurrentY + 1));
        }

        //Down
        if(CurrentY - 1 >= 0){
            if(board[CurrentX, CurrentY - 1] == null || board[CurrentX, CurrentY - 1].Team != Team) r.Add(new Vector2Int(CurrentX, CurrentY - 1));
        }

        return r;
    }
}
