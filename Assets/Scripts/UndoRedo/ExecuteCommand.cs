using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteCommand : MonoBehaviour
{
    [Header("Hud")]
    [SerializeField] private GameObject _playButton;
    [SerializeField] private GameObject _forwardButton;
    [SerializeField] private GameObject _backwardsButton;
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


    public void AddCommand(ICommand command){
        state.commands.Add(command);
    }

   //Replay screen
    public void OnPlayBtn(){
        //Execute next command
        state.ExecuteNextCommand();
    }
    public void OnForwardBtn(){
        //Redo
        state.RedoCommand();
    }

    public void OnBackwardsBtn(){
        //Undo
        state.UndoCommand();
    }

}