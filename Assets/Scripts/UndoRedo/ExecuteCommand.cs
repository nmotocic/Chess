using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteCommand : MonoBehaviour
{
    private int _index;

    public State state;

    public int CommandCount => state.commands.Count;
    //Singleton
    internal static ExecuteCommand _instance;
    void Awake() {
        if(_instance == null){
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(this);
        }
    }
    void Start() {
        state = new State {
            commands = new List<ICommand>()
        };
    }
    
    void FixedUpdate(){           
        if(Input.GetKey(KeyCode.RightArrow)){
            Debug.Log("Execute commands...");
            //execute command at current index
            state.ExecuteNextCommand();
              
        }
    }

    public void AddCommand(ICommand command){
        state.commands.Add(command);
    }

   
}