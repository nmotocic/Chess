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

    public override SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        int direction = (Team == 0) ? 1 : -1; //if white we go up, if black we go down

        //En Passant
        if(moveList.Count > 0){
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            //iF last piece was a pawn
            if(board[lastMove[1].x, lastMove[1].y].Type == PieceType.Pawn){
                //if last move was a +2 in either direction
                if(Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2){
                    //If the move was from other team
                    if(board[lastMove[1].x, lastMove[1].y].Team != Team){
                        //if both pawns are on the same y
                        if(lastMove[1].y == CurrentY){
                            //Landed left
                            if(lastMove[1].x == CurrentX - 1){
                                availableMoves.Add(new Vector2Int(CurrentX - 1, CurrentY + direction));
                                return SpecialMove.EnPassant;
                            }
                            //Landed right
                            if (lastMove[1].x == CurrentX + 1){
                                availableMoves.Add(new Vector2Int(CurrentX + 1, CurrentY + direction));
                                return SpecialMove.EnPassant;
                            }
                        }
                    }
                }
            }
        }

        //Promotion
        if((Team == 0 && CurrentY == 6) || (Team == 1 && CurrentY == 1)){
            return SpecialMove.Promotion;
        } 
        
        return SpecialMove.None;
    }
}

