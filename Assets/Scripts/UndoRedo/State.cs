using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct State
{
    public List<ICommand> commands;
    public int index;

    public void ExecuteNextCommand()
    {
        if (index >= commands.Count) return;
        //Debug.Log("Executing... "+ commands[index].GetType().Name);
        commands[index].Execute();
        index++;
    }

    public void UndoCommand(){
        if(commands.Count == 0){
            return;
        }

        if(index > 0){
            commands[index - 1].Undo();
            index--;
        }
    }

    public void RedoCommand(){
        if(commands.Count == 0){
            return;
        }

        if(index < commands.Count){
            index++;
            commands[index - 1].Execute();
        }
    }
}