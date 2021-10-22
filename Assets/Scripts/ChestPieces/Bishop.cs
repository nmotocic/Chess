using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    public override List<Vector2Int> GetAvailableMove(ref ChessPiece[,] board, int tileCount){
        List<Vector2Int> r = new List<Vector2Int>();

        //Top right
        for(int x = CurrentX + 1, y = CurrentY +1; x < tileCount && y < tileCount; x++, y++ ){
            if(board[x,y] == null) r.Add(new Vector2Int(x,y));
            else {
                if(board[x,y].Team != Team) r.Add(new Vector2Int(x,y));
                break;
            }
        }

        //Top left
        for(int x = CurrentX - 1, y = CurrentY +1; x >= 0 && y < tileCount; x--, y++ ){
            if(board[x,y] == null) r.Add(new Vector2Int(x,y));
            else {
                if(board[x,y].Team != Team) r.Add(new Vector2Int(x,y));
                break;
            }
        }

        //Bottom right
        for(int x = CurrentX + 1, y = CurrentY - 1; x < tileCount && y >= 0; x++, y-- ){
            if(board[x,y] == null) r.Add(new Vector2Int(x,y));
            else {
                if(board[x,y].Team != Team) r.Add(new Vector2Int(x,y));
                break;
            }
        }

        //Bottom left
        for(int x = CurrentX - 1, y = CurrentY - 1; x  >= 0 && y >= 0; x--, y-- ){
            if(board[x,y] == null) r.Add(new Vector2Int(x,y));
            else {
                if(board[x,y].Team != Team) r.Add(new Vector2Int(x,y));
                break;
            }
        }
        return r;
    }
}
