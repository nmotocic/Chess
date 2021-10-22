using UnityEngine;
using System.Collections.Generic;

public class Queen : ChessPiece
{
    public override List<Vector2Int> GetAvailableMove(ref ChessPiece[,] board, int tileCount){
        List<Vector2Int> r = new List<Vector2Int>();

        //Straight
            // Down
            for (int i = CurrentY -1; i >= 0; i--){
                if(board[CurrentX, i] == null) r.Add(new Vector2Int(CurrentX, i));

                if(board[CurrentX, i] != null){
                    if(board[CurrentX, i].Team != Team)
                        r.Add(new Vector2Int(CurrentX, i));
                    break;
                } 
            }

            // Up
            for (int i = CurrentY + 1; i < tileCount; i++){
                if(board[CurrentX, i] == null) r.Add(new Vector2Int(CurrentX, i));

                if(board[CurrentX, i] != null){
                    if(board[CurrentX, i].Team != Team)
                        r.Add(new Vector2Int(CurrentX, i));
                    break;
                } 
            }

            // Left
            for (int i = CurrentX -1; i >= 0; i--){
                if(board[i, CurrentY] == null) r.Add(new Vector2Int(i, CurrentY));

                if(board[i, CurrentY] != null){
                    if(board[i, CurrentY].Team != Team)
                        r.Add(new Vector2Int(i, CurrentY));
                    break;
                } 
            }

            // Right
            for (int i = CurrentX + 1; i < tileCount; i--){
                if(board[i, CurrentY] == null) r.Add(new Vector2Int(i, CurrentY));

                if(board[i, CurrentY] != null){
                    if(board[i, CurrentY].Team != Team)
                        r.Add(new Vector2Int(i, CurrentY));
                    break;
                } 
            }

        //Diagonals
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
