using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private GameObject _hud;
    [SerializeField] private Text _moveRecord;
    [SerializeField] private GameObject _replayHud;
    
    [Header("Load")]
    [SerializeField] private Image loadMenu;

    [SerializeField] private GameObject _loadMenuBtn;

    [Header("Command")]
    [SerializeField] private GameObject _commandManager;

    [Header("Animation")]
    [SerializeField] private Animator menuAnimator;

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

    public Animator MenuAnimator { get => menuAnimator; set => menuAnimator = value; }
    public GameObject Hud { get => _hud; set => _hud = value; }
    public GameObject ReplayHud { get => _replayHud; set => _replayHud = value; }

    // Start is called before the first frame update
    void Start()
    {
        GetSavedGames();
    }


    //Main Menu UI Bttns
    public void OnPlayBtn(){
        MenuAnimator.SetTrigger("InGameMenu");
        ChessBoard.Instance.enableDragging = true;
        Hud.SetActive(true);
    }
    public void OnLoadBtn(){
        //change to saves list screen
        MenuAnimator.SetTrigger("LoadScreen");
    }

    public void OnLoadBackBtn(){
        //change to saves list screen
        MenuAnimator.SetTrigger("GameMenu");
    }

    public void OnExitBtn(){
         Application.Quit();
    }

    public void LoadFile(){
        List<SaveEntry> entries = new List<SaveEntry>();

        GameObject btn = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if(btn != null){

            string filename = btn.transform.GetChild(0).GetComponent<Text>().text;
            filename += ".json";
            string path = Application.persistentDataPath + "/" + filename;
            entries = FileHandler.ReadFromJSON<SaveEntry>(filename);
            
            CommandParser cp = _commandManager.gameObject.GetComponent<CommandParser>();
            cp.Entries = entries;
            cp.Parse();
        }

       loadMenu.gameObject.SetActive(false);
       _loadMenuBtn.SetActive(false);
       ReplayHud.gameObject.SetActive(true);
       ChessBoard.Instance.enableDragging = false;
    }
 
    //Helper class
    public void RecordNewMove(ChessPiece chessPiece, Vector2Int[] move)
    {
        string pieceType = chessPiece.Type.ToString();
        //Because we are looking at the board as a 8x8 matrix, we need to convert column indexes to letters as it's chess standard
        _moveRecord.text += String.Format($"{pieceType}: {ProcessColumn(move[1][1])}{move[1][0]}{ProcessColumn(move[0][1])}{move[0][0]} \n");
        
    }

    public void RecordSpecialMove(SpecialMove specialMove, Vector2Int[] move){
        string special = specialMove.ToString();

        if(specialMove == SpecialMove.EnPassant) {
            _moveRecord.text += String.Format($"{ProcessColumn(move[0][1])}{move[0][0]}x{ProcessColumn(move[1][1])}{move[1][0]} e.p.\n");
        } else if (specialMove == SpecialMove.Promotion){
            _moveRecord.text += String.Format($"{ProcessColumn(move[0][1])}{move[0][0]}x{ProcessColumn(move[1][1])}{move[1][0]}=Q \n");
        }
    }

    public void RecordCastling(int side){
        if (side == 2) { 
            //queenside
            _moveRecord.text += String.Format($"O-O-O\n");
        } else if (side == 6) {
            //kingside
            _moveRecord.text += String.Format($"O-O\n");
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

    private void GetSavedGames()
    {
        //Fetch all of the saved games
        string[] fileNames = Directory.GetFiles(Application.persistentDataPath);
        
        GameObject btnTemplate = loadMenu.transform.GetChild(0).gameObject;
        GameObject btn;

        for (int f = 0; f < fileNames.Length; f++){
            string filename = Between(fileNames[f], "\\", ".");

            btn = Instantiate(btnTemplate, loadMenu.transform);
            btn.transform.GetChild(0).GetComponent<Text>().text = filename;
        }

        Destroy(btnTemplate);
    }

    private string Between(string STR , string FirstString, string LastString)
    {       
        string FinalString;     
        int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
        int Pos2 = STR.IndexOf(LastString);
        FinalString = STR.Substring(Pos1, Pos2 - Pos1);
        return FinalString;
    }
    

}
