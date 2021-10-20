using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override List<Vector2Int> GetAvailableMove(ref ChessPiece[,] board, int tileCount){
        List<Vector2Int> r = new List<Vector2Int>();

        //Top right
        int x = CurrentX + 1;
        int y = CurrentY + 2;
        if(x < tileCount && y < tileCount){
            if(board[x,y] == null || board[x,y].Team != Team){
                r.Add(new Vector2Int(x,y));
            }
        }

        x = CurrentX + 2;
        y = CurrentY + 1;
        if(x < tileCount && y < tileCount){
            if(board[x,y] == null || board[x,y].Team != Team){
                r.Add(new Vector2Int(x,y));
            }
        }

        //Top left
        x = CurrentX - 1;
        y = CurrentY + 2;
        if(x >= 0 && y < tileCount){
            if(board[x,y] == null || board[x,y].Team != Team){
                r.Add(new Vector2Int(x,y));
            }
        }

        x = CurrentX - 2;
        y = CurrentY + 1;
        if(x >= 0 && y < tileCount){
            if(board[x,y] == null || board[x,y].Team != Team){
                r.Add(new Vector2Int(x,y));
            }
        }

        //Bottom right
        x = CurrentX + 1;
        y = CurrentY - 2;
        if(x < tileCount && y >= 0){
            if(board[x,y] == null || board[x,y].Team != Team){
                r.Add(new Vector2Int(x,y));
            }
        }

        x = CurrentX + 2;
        y = CurrentY - 1;
        if(x < tileCount && y >= 0){
            if(board[x,y] == null || board[x,y].Team != Team){
                r.Add(new Vector2Int(x,y));
            }
        }

        //Bottom left
        x = CurrentX - 1;
        y = CurrentY - 2;
        if(x >= 0 && y >= 0){
            if(board[x,y] == null || board[x,y].Team != Team){
                r.Add(new Vector2Int(x,y));
            }
        }

        x = CurrentX - 2;
        y = CurrentY - 1;
        if(x >= 0 && y >= 0){
            if(board[x,y] == null || board[x,y].Team != Team){
                r.Add(new Vector2Int(x,y));
            }
        }

        return r;
    }
}
