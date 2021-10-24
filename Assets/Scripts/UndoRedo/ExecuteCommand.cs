using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteCommand : MonoBehaviour
{
    public ChessBoard chessBoard;
    private List<MoveCommand> _commandList = new List<MoveCommand>();

    public int index = 0;

    void Update(){
        if(_commandList.Count > 0){            
            if(Input.GetKey(KeyCode.RightArrow)){
                Debug.Log("Execute commands...");
                //execute command at current index
              
            }
            
        }
    }

    public void SetCommandList(List<MoveCommand> commandList){
        _commandList = commandList;
    }

   
}