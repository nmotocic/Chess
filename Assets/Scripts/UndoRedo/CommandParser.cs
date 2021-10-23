using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandParser
{
   public List<ICommand> commands = new List<ICommand>();
   private List<SaveEntry> entries = new List<SaveEntry>();
   
   public CommandParser(List<SaveEntry> entries){
       this.entries = entries;
   }
   public void Parse(){
       
        //DEBUG
        foreach (SaveEntry entry in entries) Debug.Log(entry);

        //parse into commands
        foreach (SaveEntry entry in entries){
            //commands.Add(new MoveCommand(entry.chessPiece, entry.move) as MoveCommand);
        }

   } 
}
