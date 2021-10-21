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

    public override SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        SpecialMove r = SpecialMove.None;
        int checkTeam = ((Team == 0) ? 0 : 7);

        var kingMove = moveList.Find(m => m[0].x == 4 && m[0].y == checkTeam); //(4,0) / (4,7) check if King moved
        var leftRook = moveList.Find(m => m[0].x == 0 && m[0].y == checkTeam); //(0,0) / (0,7) check if left Rook moved
        var rightRook = moveList.Find(m => m[0].x == 7 && m[0].y == checkTeam); //(7,0) / (7,7) check if right Rook moved

        if(kingMove == null && CurrentX == 4){
            //White team
            if(Team == 0) {
                //Left rook
                if(leftRook == null) {
                    if(board[0,0].Type == PieceType.Rook)
                        if(board[0,0].Team == 0){
                            //Obstruction in between King and Rook
                            if(board[3,0] == null && board[2,0] == null && board[1,0] == null ){
                                availableMoves.Add(new Vector2Int(2,0));
                                r = SpecialMove.Castling;
                            }
                        }
                }

                //Right rook
                if(rightRook == null) {
                    if(board[7,0].Type == PieceType.Rook)
                        if(board[7,0].Team == 0){
                            //Obstruction in between King and Rook
                            if(board[5,0] == null && board[6,0] == null){
                                availableMoves.Add(new Vector2Int(6,0));
                                r = SpecialMove.Castling;
                            }
                        }
                }
            //Black team
            } else {
                //Left rook
                if(leftRook == null) {
                    if(board[0,7].Type == PieceType.Rook)
                        if(board[0,7].Team == 1){
                            //Obstruction in between King and Rook
                            if(board[3,7] == null && board[2,7] == null && board[1,7] == null ){
                                availableMoves.Add(new Vector2Int(2,7));
                                r = SpecialMove.Castling;
                            }
                        }
                }

                //Right rook
                if(rightRook == null) {
                    if(board[7,7].Type == PieceType.Rook)
                        if(board[7,7].Team == 1){
                            //Obstruction in between King and Rook
                            if(board[5,7] == null && board[6,7] == null){
                                availableMoves.Add(new Vector2Int(6,7));
                                r = SpecialMove.Castling;
                            }
                        }
                }
            }
        }

        return r;
    }
}
