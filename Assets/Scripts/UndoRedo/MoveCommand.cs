using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MoveCommand : ICommand
{
    private PieceType _pieceToMove;
    private Vector2Int[] _moveToMake; //from -> to

    public MoveCommand(PieceType piece, Vector2Int[] move){
        _pieceToMove = piece;
        _moveToMake = move;
    }
    public void Execute()
    {
        throw new NotImplementedException();
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }
}
