using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMove(ref ChessPiece[,] board, int tileCount){
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (Team == 0) ? 1 : -1; //if white we go up, if black we go down
        
        //One in front
        if(board[CurrentX, CurrentY + direction] == null) r.Add(new Vector2Int(CurrentX, CurrentY + direction));

        //Two in front, if we are at starting point
        if(board[CurrentX, CurrentY + direction] == null){
            //White
            if(Team == 0 && CurrentY == 1 && board[CurrentX, CurrentY + (direction * 2)] == null){
                r.Add(new Vector2Int( CurrentX, CurrentY + (direction * 2) ));
            }
            //Black
            if(Team == 1 && CurrentY == 6 && board[CurrentX, CurrentY + (direction * 2)] == null){
                r.Add(new Vector2Int( CurrentX, CurrentY + (direction * 2) ));
            }
        }

        //Eat move
        if(CurrentX != tileCount -1){
            if(board[CurrentX + 1, CurrentY + direction] != null && board[CurrentX + 1, CurrentY + direction].Team != Team)
                r.Add(new Vector2Int(CurrentX + 1, CurrentY + direction));
        }
        if(CurrentX != 0){
            if(board[CurrentX - 1, CurrentY + direction] != null && board[CurrentX - 1, CurrentY + direction].Team != Team)
                r.Add(new Vector2Int(CurrentX - 1, CurrentY + direction));
        }

        return r;
    }
}
