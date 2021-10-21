using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text moveRecord;
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
            }
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RecordNewMove(ChessPiece chessPiece, Vector2Int[] move)
    {
        string pieceType = chessPiece.Type.ToString();
        //Because we are looking at the board as a 8x8 matrix, we need to convert column indexes to letters as it's chess standard
        moveRecord.text += String.Format($"{pieceType}: {ProcessColumn(move[1][1])}{move[1][0]}{ProcessColumn(move[0][1])}{move[0][0]} \n");
        
    }

    public void RecordSpecialMove(SpecialMove specialMove, Vector2Int[] move){
        string special = specialMove.ToString();

        if(specialMove == SpecialMove.EnPassant) {
            moveRecord.text += String.Format($"{ProcessColumn(move[0][1])}{move[0][0]}x{ProcessColumn(move[1][1])}{move[1][0]} e.p.\n");
        } else if (specialMove == SpecialMove.Promotion){
            moveRecord.text += String.Format($"{ProcessColumn(move[0][1])}{move[0][0]}x{ProcessColumn(move[1][1])}{move[1][0]}=Q \n");
        }
    }

    public void RecordCastling(int side){
        if (side == 2) { 
            //queenside
            moveRecord.text += String.Format($"O-O-O\n");
        } else if (side == 6) {
            //kingside
            moveRecord.text += String.Format($"O-O\n");
        }
    }

    private string ProcessColumn(int index){
        switch(index){
            case 0:
                return "A";
            case 1:
                return "B";
            case 2:
                return "C";
            case 3:
                return "D";
            case 4:
                return "E";
            case 5:
                return "F";
            case 6:
                return "G";
            case 7:
                return "H";
        }
        return "A";
    }

}
