using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MoveCommand : ICommand
{
    private ChessPiece _pieceToMove;
    private Vector2Int[] _moveToMake; //from -> to

    public MoveCommand(ChessPiece piece, Vector2Int[] move){
        _pieceToMove = piece;
        _moveToMake = move;
    }
    public void Execute(ChessBoard chessBoard)
    {
        chessBoard.MoveTo( _pieceToMove , _moveToMake[1].x, _moveToMake[1].y);
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }
}
