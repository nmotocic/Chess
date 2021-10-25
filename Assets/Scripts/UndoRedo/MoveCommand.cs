using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MoveCommand : ICommand
{
    private Vector2Int[] _moveToMake; //from -> to

    public MoveCommand(Vector2Int[] move){
        _moveToMake = move;
    }
    public void Execute()
    {
        ChessBoard.Instance.Move( _moveToMake[0].x, _moveToMake[0].y , _moveToMake[1].x, _moveToMake[1].y);
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }
}
