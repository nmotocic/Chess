using System;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}
public class ChessPiece : MonoBehaviour
{
    public PieceType Type;
    public int Team; //0 - white, 1 - black
    public int CurrentX;
    public int CurrentY;

    private Vector3 _desiredPosition;
    private Vector3 _desiredScale = Vector3.one;

    private void Start(){
        transform.rotation = Quaternion.Euler((Team == 0) ? Vector3.zero : new Vector3(0,180,0));
    }
    
    private void Update(){
        transform.position = Vector3.Lerp(transform.position, _desiredPosition, Time.deltaTime * 5);
        transform.localScale = Vector3.Lerp(transform.localScale, _desiredScale, Time.deltaTime * 5);
    }
    public virtual void SetPosition(Vector3 position, bool force = false) {
        _desiredPosition = position;
        if(force) transform.position = _desiredPosition;
    }

    public virtual void SetScale (Vector3 scale, bool force = false){
        _desiredScale = scale;
        if (force) transform.localScale = _desiredScale;
    }

    public virtual List<Vector2Int> GetAvailableMove(ref ChessPiece[,] board, int tileCount){
        List<Vector2Int> r = new List<Vector2Int>();

        r.Add(new Vector2Int(3, 3));
        r.Add(new Vector2Int(3, 4));
        r.Add(new Vector2Int(4, 3));
        r.Add(new Vector2Int(4, 4));

        return r;
    }

    public virtual SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        return SpecialMove.None;
    }
}
