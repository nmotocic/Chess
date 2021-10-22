using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SaveEntry
{
    public string chessPiece;
    public Vector2Int[] move;

    public SaveEntry(PieceType chessPiece, Vector2Int[] move){
        this.chessPiece = chessPiece.ToString();
        this.move = move;
    }   
}
