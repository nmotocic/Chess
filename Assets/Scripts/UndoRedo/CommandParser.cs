using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandParser : MonoBehaviour
{
   public List<MoveCommand> commands = new List<MoveCommand>();
   private List<SaveEntry> entries = new List<SaveEntry>();
   private ExecuteCommand  _executeCommand;

    public List<SaveEntry> Entries { get => entries; set => entries = value; }

    void Awake(){
       _executeCommand = GetComponent<ExecuteCommand>();
   }
   
   public void Parse(){
       
        //DEBUG
        foreach (SaveEntry entry in Entries) Debug.Log(entry);

        //parse into commands
        foreach (SaveEntry entry in Entries){
            commands.Add(new MoveCommand(entry.chessPiece, entry.move));
        }
        _executeCommand.SetCommandList(commands);

   } 
}
