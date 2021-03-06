using System;
using System.Collections.Generic;
using UnityEngine;

public enum SpecialMove{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}
public class ChessBoard : MonoBehaviour
{
    //Logic
    private ChessPiece[,] _chessPieces;
    private ChessPiece _currentlyDragging;
    private List<Vector2Int> _availableMoves = new List<Vector2Int>();
    private const int TILE_COUNT = 8;
    private GameObject[,] _tiles;
    private Camera _currentCamera;
    private Vector2Int _currentHover;
    private Vector3 _bounds;
    private List<ChessPiece> _eatenWhites = new List<ChessPiece>();
    private List<ChessPiece> _eatenBlacks = new List<ChessPiece>();
    private List<Vector2Int[]> _moveList = new List<Vector2Int[]>();
    private SpecialMove _specialMove;
    private bool _isWhiteTurn;

    public bool enableDragging = false;

    //Saving logic
    private List<SaveEntry> entries = new List<SaveEntry>();

    //Art
    [Header("Art")]
    [SerializeField] private Material _tileMaterial;
    [SerializeField] private float _tileSize = 1.0f;
    [SerializeField] private float _yOffset = 0.2f;
    [SerializeField] private Vector3  _boardCenter = Vector3.zero;
    [SerializeField] private float _eatenSize = 0.3f;
    [SerializeField] private float _deathSpacing = 0.3f;
    [SerializeField] private float _dragOffset = 0.75f;

    [Header("UI")]

    [SerializeField] private GameObject _victoryScreen;
    [SerializeField] private GameObject _resultScreen;

    [Header("Prefabs")]
    [SerializeField] private GameObject[] _prefabs;
    [SerializeField] private Material[] _teamMaterials;


    private static ChessBoard _instance;
    public static ChessBoard Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ChessBoard>();
            }
            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }

    void Start() {
        _isWhiteTurn = true;
        GenerateGrid(_tileSize, TILE_COUNT);
        SpawnAllPieces();
        PositionPieces();
    }
    void Update() {
        if(!_currentCamera){
            _currentCamera = Camera.main;
            return;
        }

        if(enableDragging){
            RaycastHit info;
            Ray ray = _currentCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight"))){
                //Get the indexes of tile I've hit
                Vector2Int hitPosition = LookUpTileIndex(info.transform.gameObject);

                if(_currentHover == -Vector2Int.one) {
                    _currentHover = hitPosition;
                    _tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }

                //If we already hovered...
                if(_currentHover != hitPosition) {

                    _tiles[_currentHover.x, _currentHover.y].layer = (ContainsValidMove(ref _availableMoves, _currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                    _currentHover = hitPosition;
                    _tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }

                //If mouse pressed
                if(Input.GetMouseButtonDown(0)){
                    if(_chessPieces[hitPosition.x, hitPosition.y] != null) {
                        //Our turn?
                        if((_chessPieces[hitPosition.x, hitPosition.y].Team == 0 && _isWhiteTurn) ||
                        (_chessPieces[hitPosition.x, hitPosition.y].Team == 1 && !_isWhiteTurn)) {
                            _currentlyDragging = _chessPieces[hitPosition.x, hitPosition.y];
                            
                            //Get the list of available moves and highlight the tiles
                            _availableMoves = _currentlyDragging.GetAvailableMove(ref _chessPieces, TILE_COUNT);
                            //Get a list of special moves
                            _specialMove = _currentlyDragging.GetSpecialMove(ref _chessPieces, ref _moveList, ref _availableMoves);

                            PreventCheck();
                            HighlightTiles();
                        }
                    }
                }

                //If mouse released
                if(Input.GetMouseButtonUp(0) && _currentlyDragging != null){
                    Vector2Int previousPosition = new Vector2Int(_currentlyDragging.CurrentX, _currentlyDragging.CurrentY);

                    bool validMove = MoveTo(_currentlyDragging, hitPosition.x, hitPosition.y);
                    if(!validMove){
                        _currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
                    }
                        
                    _currentlyDragging = null;
                    RemoveHighlightTiles();
                }
            } else {
                if(_currentHover != -Vector2Int.one) {
                    _tiles[_currentHover.x, _currentHover.y].layer =  (ContainsValidMove(ref _availableMoves, _currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                    _currentHover = -Vector2Int.one;
                }


                if(_currentlyDragging && Input.GetMouseButtonUp(0)){
                    _currentlyDragging.SetPosition(GetTileCenter(_currentlyDragging.CurrentX, _currentlyDragging.CurrentY));
                    _currentlyDragging = null;
                    RemoveHighlightTiles();
                }
            }

            if(_currentlyDragging){
                Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * _yOffset);
                float distance = 0.0f;
                if(horizontalPlane.Raycast(ray, out distance)){
                    _currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * _dragOffset);
                }
            }
        }
    }


    //Highlights
    private void HighlightTiles()
    {
        for(int i = 0; i < _availableMoves.Count; i++){
            _tiles[_availableMoves[i].x, _availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }

    private void RemoveHighlightTiles(){
         for(int i = 0; i < _availableMoves.Count; i++){
            _tiles[_availableMoves[i].x, _availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        _availableMoves.Clear();
    }

    //Generate board
    private void GenerateGrid(float tileSize, int tileCount){
        _yOffset += transform.position.y;
        _bounds = new Vector3((tileCount / 2) * tileSize, 0 , (tileCount / 2) * tileSize) + _boardCenter;

        _tiles = new GameObject[tileCount, tileCount];

        for(int x = 0; x < tileCount; x++){
            for(int y = 0; y < tileCount; y++){
                _tiles[x,y] = GenerateSingleTile(tileSize, x, y);
            }

        }
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tile = new GameObject(string.Format("X:{0}, Y:{1}", x+1, y+1));
        tile.transform.parent = transform; 

        Mesh mesh = new Mesh();
        tile.AddComponent<MeshFilter>().mesh = mesh;
        tile.AddComponent<MeshRenderer>().material = _tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, _yOffset, y * tileSize) - _bounds;
        vertices[1] = new Vector3(x * tileSize, _yOffset, (y + 1) * tileSize) - _bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, _yOffset, y * tileSize) - _bounds; 
        vertices[3] = new Vector3((x + 1) * tileSize, _yOffset, (y + 1) * tileSize) - _bounds;

        int[] triangles = new int[] {0,1,2,1,3,2};
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        
        tile.layer = LayerMask.NameToLayer("Tile");
        tile.AddComponent<BoxCollider>();
        return tile;
    }

    //Spawn Pieces
    private void SpawnAllPieces(){
        _chessPieces = new ChessPiece[TILE_COUNT, TILE_COUNT];
        int white = 0, black = 1;

        //White
        _chessPieces[0, 0] = SpawnSinglePiece(PieceType.Rook, white);
        _chessPieces[1, 0] = SpawnSinglePiece(PieceType.Knight, white);
        _chessPieces[2, 0] = SpawnSinglePiece(PieceType.Bishop, white);
        _chessPieces[3, 0] = SpawnSinglePiece(PieceType.Queen, white);
        _chessPieces[4, 0] = SpawnSinglePiece(PieceType.King, white);
        _chessPieces[5, 0] = SpawnSinglePiece(PieceType.Bishop, white);
        _chessPieces[6, 0] = SpawnSinglePiece(PieceType.Knight, white);
        _chessPieces[7, 0] = SpawnSinglePiece(PieceType.Rook, white);

        for(int i = 0; i < TILE_COUNT; i++){
            _chessPieces[i,1] = SpawnSinglePiece(PieceType.Pawn, white);
        }

        //Black
        _chessPieces[0, 7] = SpawnSinglePiece(PieceType.Rook, black);
        _chessPieces[1, 7] = SpawnSinglePiece(PieceType.Knight, black);
        _chessPieces[2, 7] = SpawnSinglePiece(PieceType.Bishop, black);
        _chessPieces[3, 7] = SpawnSinglePiece(PieceType.Queen, black);
        _chessPieces[4, 7] = SpawnSinglePiece(PieceType.King, black);
        _chessPieces[5, 7] = SpawnSinglePiece(PieceType.Bishop, black);
        _chessPieces[6, 7] = SpawnSinglePiece(PieceType.Knight, black);
        _chessPieces[7, 7] = SpawnSinglePiece(PieceType.Rook, black);

        for(int i = 0; i < TILE_COUNT; i++){
            _chessPieces[i, 6] = SpawnSinglePiece(PieceType.Pawn, black);
        }
    }
    private ChessPiece SpawnSinglePiece(PieceType type, int team){
        ChessPiece piece = Instantiate(_prefabs[(int) type - 1], transform).GetComponent<ChessPiece>();
        piece.Type = type;
        piece.Team = team;

        piece.GetComponent<MeshRenderer>().material = _teamMaterials[team];
        

        return piece;
    }
    private void PositionPieces(){
        for (int x = 0; x < TILE_COUNT; x ++){
            for(int y = 0; y < TILE_COUNT; y++){
                if(_chessPieces[x,y] != null){
                    PositionSinglePiece(x,y, true);
                }
            }
        }
    }
    private void PositionSinglePiece(int x, int y, bool force = false){

        _chessPieces[x, y].CurrentX = x;
        _chessPieces[x, y].CurrentY = y;
        _chessPieces[x,y].SetPosition(GetTileCenter(x,y), force);
    }

    private Vector3 GetTileCenter(int x, int y){
        return new Vector3(x * _tileSize, _yOffset, y * _tileSize) - _bounds + new Vector3(_tileSize / 2, 0, _tileSize / 2);
    }
    private Vector2Int LookUpTileIndex (GameObject hitInfo){

        for(int x = 0; x < TILE_COUNT; x++){
            for(int y = 0; y < TILE_COUNT; y++){
                if(_tiles[x,y] == hitInfo) return new Vector2Int(x,y);
            }
        }
        return -Vector2Int.one; //Invalid
    }

   //Moving
    private bool MoveTo(ChessPiece chessPiece, int x, int y)
    {
        if(!ContainsValidMove(ref _availableMoves, new Vector2Int(x,y))){
            return false;
        } 

        Vector2Int previousPosition = new Vector2Int(chessPiece.CurrentX, chessPiece.CurrentY);
        
        if(_chessPieces[x,y] != null){
            ChessPiece otherPiece = _chessPieces[x,y];
            if(chessPiece.Team == otherPiece.Team) return false;

            //Eating
            if(otherPiece.Team == 0){
                if(otherPiece.Type == PieceType.King) CheckMate(1);

                _eatenWhites.Add(otherPiece);
                otherPiece.SetScale(Vector3.one * _eatenSize);
                otherPiece.SetPosition(new Vector3(8 * _tileSize, _yOffset, -1 * _tileSize) 
                                     - _bounds 
                                     + new Vector3(_tileSize / 2, 0, _tileSize / 2)
                                     + (Vector3.forward * _deathSpacing) * _eatenWhites.Count);
            } else {
                if(otherPiece.Type == PieceType.King) CheckMate(0);

                _eatenBlacks.Add(otherPiece);
                otherPiece.SetScale(Vector3.one * _eatenSize);
                otherPiece.SetPosition(new Vector3(-1 * _tileSize, _yOffset, 8 * _tileSize) 
                                     - _bounds 
                                     + new Vector3(_tileSize / 2, 0, _tileSize / 2)
                                     + (Vector3.back * _deathSpacing) * _eatenBlacks.Count);
                
            }
        }
        _chessPieces[x,y] = chessPiece;
        _chessPieces[previousPosition.x, previousPosition.y] = null;
        PositionSinglePiece(x,y);

        _isWhiteTurn = !_isWhiteTurn;
        Vector2Int[] move = new Vector2Int[] { previousPosition, new Vector2Int(x,y) };
        _moveList.Add(move);
        
        ProcessSpecialMove();
        if(_specialMove == SpecialMove.None) RecordMove(chessPiece, move);
        
        if(CheckForCheckMate()) CheckMate(chessPiece.Team);
        return true;
    }

    //Move command
    public void Move(int FromX, int FromY, int ToX, int ToY){
        //from->to
        ChessPiece pieceToMove = _chessPieces[FromX, FromY];
        _availableMoves = pieceToMove.GetAvailableMove(ref _chessPieces, TILE_COUNT);
        
        Vector2Int previousPosition = new Vector2Int(pieceToMove.CurrentX, pieceToMove.CurrentY);

        Vector2Int hitPosition = LookUpTileIndex(_tiles[ToX, ToY]);

        bool validMove = MoveTo(pieceToMove, hitPosition.x, hitPosition.y);
        
        if(!validMove){
            pieceToMove.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
        }
    }

    private void CheckMate(int team)
    {
        if(enableDragging){
            DisplayVictory(team);
        } else {
            DisplayResult(team);
        }
        
    }
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 position){
        for (int i = 0; i< moves.Count; i++){
            if(moves[i].x == position.x && moves[i].y == position.y){
                return true;
            }
        }
        return false;
    }

    //Special Moves
    private void ProcessSpecialMove(){
        if(_specialMove == SpecialMove.EnPassant){
            Vector2Int[] newMove = _moveList[_moveList.Count - 1];
            ChessPiece myPawn = _chessPieces[newMove[1].x, newMove[1].y];

            Vector2Int[] targetPawn = _moveList[_moveList.Count - 2];
            ChessPiece enemyPawn = _chessPieces[targetPawn[1].x, targetPawn[1].y];

            if(myPawn.CurrentX == enemyPawn.CurrentX){
                if(myPawn.CurrentY == enemyPawn.CurrentY - 1 || 
                myPawn.CurrentY == enemyPawn.CurrentY + 1) {
                    if(enemyPawn.Team == 0){
                        _eatenWhites.Add(enemyPawn);
                        enemyPawn.SetScale(Vector3.one * _eatenSize);
                        enemyPawn.SetPosition(new Vector3(8 * _tileSize, _yOffset, -1 * _tileSize) 
                                     - _bounds 
                                     + new Vector3(_tileSize / 2, 0, _tileSize / 2)
                                     + (Vector3.forward * _deathSpacing) * _eatenWhites.Count);

                    } else {
                        _eatenBlacks.Add(enemyPawn);
                        enemyPawn.SetScale(Vector3.one * _eatenSize);
                        enemyPawn.SetPosition(new Vector3(-1 * _tileSize, _yOffset, 8 * _tileSize) 
                                     - _bounds 
                                     + new Vector3(_tileSize / 2, 0, _tileSize / 2)
                                     + (Vector3.back * _deathSpacing) * _eatenBlacks.Count);

                    }
                    _chessPieces[enemyPawn.CurrentX, enemyPawn.CurrentY] = null;
                    UIManager.Instance.RecordSpecialMove(SpecialMove.EnPassant, newMove);
                }
            }
            
        }
        
        if(_specialMove == SpecialMove.Castling){
            Vector2Int[] lastMove = _moveList[_moveList.Count - 1];
            //Left Rook
            if(lastMove[1].x == 2){
                switch(lastMove[1].y){
                    case 0: //White
                        ChessPiece wRook = _chessPieces[0,0];
                        _chessPieces[3,0] = wRook;
                        PositionSinglePiece(3,0);
                        _chessPieces[0,0] = null;
                        break;
                    case 7: //Black
                        ChessPiece bRook = _chessPieces[0,7];
                        _chessPieces[3,7] = bRook;
                        PositionSinglePiece(3,7);
                        _chessPieces[0,7] = null;
                        break;
                }
                UIManager.Instance.RecordCastling(2);
                //Right Rook
            } else if (lastMove[1].x == 6) {
                switch(lastMove[1].y){
                    case 0: //White
                        ChessPiece wRook = _chessPieces[7,0];
                        _chessPieces[5,0] = wRook;
                        PositionSinglePiece(5,0);
                        _chessPieces[7,0] = null;
                        break;
                    case 7: //Black
                        ChessPiece bRook = _chessPieces[7,7];
                        _chessPieces[5,7] = bRook;
                        PositionSinglePiece(5,7);
                        _chessPieces[7,7] = null;
                        break;
                }
                UIManager.Instance.RecordCastling(6);
            }
        }

        if(_specialMove == SpecialMove.Promotion){
            Vector2Int[] lastMove = _moveList[_moveList.Count - 1];
            ChessPiece myPawn = _chessPieces[lastMove[1].x, lastMove[1].y];
            
            if(myPawn.Type == PieceType.Pawn){
                if(myPawn.Team == 0 && lastMove[1].y == 7){
                    ChessPiece newQueen = SpawnSinglePiece(PieceType.Queen, 0);
                    newQueen.transform.position = _chessPieces[lastMove[1].x, lastMove[1].y].transform.position;

                    Destroy(_chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                    _chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;

                    PositionSinglePiece(lastMove[1].x, lastMove[1].y, true);
                }

                if(myPawn.Team == 1 && lastMove[1].y == 0){
                    ChessPiece newQueen = SpawnSinglePiece(PieceType.Queen, 1);
                    newQueen.transform.position = _chessPieces[lastMove[1].x, lastMove[1].y].transform.position;

                    Destroy(_chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                    _chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;

                    PositionSinglePiece(lastMove[1].x, lastMove[1].y, true);
                }
                UIManager.Instance.RecordSpecialMove(SpecialMove.Promotion, lastMove);
            }

        }
    }
    private void PreventCheck()
    {
        ChessPiece targetKing = null;
        for (int x = 0; x < TILE_COUNT; x++){
            for(int y = 0; y < TILE_COUNT; y++){
                if(_chessPieces[x,y] != null){
                    if(_chessPieces[x,y].Type == PieceType.King && _chessPieces[x,y].Team == _currentlyDragging.Team){
                        targetKing = _chessPieces[x,y];
                    }
                }
            }
        }

        SimulateMove(_currentlyDragging, ref _availableMoves, targetKing);

    }
    private void SimulateMove(ChessPiece chessPiece, ref List<Vector2Int> moves, ChessPiece targetKing){
        // Save current values
        int actualX = chessPiece.CurrentX;
        int actualY = chessPiece.CurrentY;
        List<Vector2Int> movesToRemove = new List<Vector2Int>();

        // Going through all the moves and check for check position
        for(int i = 0; i < moves.Count; i++){
            int simX = moves[i].x;
            int simY = moves[i].y;

            Vector2Int kingPositionThisSim = new Vector2Int(targetKing.CurrentX, targetKing.CurrentY);
            //Did we simulate the king's move?
            if(chessPiece.Type == PieceType.King) kingPositionThisSim = new Vector2Int(simX, simY);
            //Copy the board
            ChessPiece[,] simulation = new ChessPiece[TILE_COUNT, TILE_COUNT];
            List<ChessPiece> simAttack = new List<ChessPiece>();
            for (int x = 0; x < TILE_COUNT; x ++){
                for(int y = 0; y < TILE_COUNT; y++){
                    if (_chessPieces[x,y] != null){
                        simulation[x,y] = _chessPieces[x,y];
                        if(simulation[x,y].Team != chessPiece.Team) simAttack.Add(simulation[x,y]);
                    }
                }
            }
            //Simulate the move
            simulation[actualX, actualY] = null;
            chessPiece.CurrentX = simX;
            chessPiece.CurrentY = simY;
            simulation[simX,simY] = chessPiece;

            //Did one of the pieces got taken down during the simulation??
            var deadPiece = simAttack.Find(x => x.CurrentX == simX && x.CurrentY == simY);
            if(deadPiece != null) simAttack.Remove(deadPiece);

            //Get all of the simulated attacking pieces' moves
            List<Vector2Int> simMoves = new List<Vector2Int>();
            for(int ap = 0; ap < simAttack.Count; ap++){
                var pieceMoves = simAttack[ap].GetAvailableMove(ref simulation, TILE_COUNT);
                for (int p = 0; p < pieceMoves.Count; p++) simMoves.Add(pieceMoves[p]);
            }

            //Is the king in trouble?
            if(ContainsValidMove(ref simMoves, kingPositionThisSim)){
                movesToRemove.Add(moves[i]);
            }
            //Restore the cp data
            chessPiece.CurrentX = actualX;
            chessPiece.CurrentY = actualY;
        }

        // Remove from current available move list
        for(int i = 0; i<movesToRemove.Count; i++) moves.Remove(movesToRemove[i]);
    }
    private bool CheckForCheckMate(){

        var lastMove = _moveList[_moveList.Count - 1];
        int targetTeam = (_chessPieces[lastMove[1].x, lastMove[1].y].Team == 0) ? 1 : 0;

        ChessPiece targetKing = null;
        List<ChessPiece> attackingPieces = new List<ChessPiece>();
        List<ChessPiece> defendingPieces = new List<ChessPiece>();

        for (int x = 0; x < TILE_COUNT; x++){
            for(int y = 0; y < TILE_COUNT; y++){
                if(_chessPieces[x,y] != null){
                    if( _chessPieces[x,y].Team == targetTeam){
                        defendingPieces.Add(_chessPieces[x,y]);
                        if(_chessPieces[x,y].Type == PieceType.King){
                            targetKing = _chessPieces[x,y];
                        }
                    } else {
                        attackingPieces.Add(_chessPieces[x,y]);
                    }
                }
            }
        }

        //Is the king attacked rn?
        List<Vector2Int> currentAvailableMoves = new List<Vector2Int>();
        for(int i = 0; i < attackingPieces.Count; i++){
            var pieceMoves = attackingPieces[i].GetAvailableMove(ref _chessPieces, TILE_COUNT);
                for (int p = 0; p < pieceMoves.Count; p++) currentAvailableMoves.Add(pieceMoves[p]);
        }

        //Are we in check?
        if(ContainsValidMove(ref currentAvailableMoves, new Vector2Int(targetKing.CurrentX, targetKing.CurrentY))){
            //King is under the attack! Help incoming!
            for (int i = 0; i < defendingPieces.Count; i++){
                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMove(ref _chessPieces, TILE_COUNT);
                SimulateMove(defendingPieces[i], ref defendingMoves, targetKing);

                if (defendingMoves.Count != 0) return false; //in check, but not in checkmate

            }
            return true; //Checkmate exit
        }
        return false;
    }
    //UI 
    private void RecordMove(ChessPiece chessPiece, Vector2Int[] move)
    {
        
        //Simple move record
        UIManager.Instance.RecordNewMove(chessPiece, move);
        entries.Add(new SaveEntry(chessPiece, move));
        
    }
    private void DisplayVictory(int winningTeam){
        _victoryScreen.SetActive(true);
        _victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
        UIManager.Instance.Hud.SetActive(false);
    }

    private void DisplayResult(int winningTeam){
        _resultScreen.SetActive(true);
        _resultScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
        UIManager.Instance.ReplayHud.SetActive(false);
    }
    public void OnResetButton(){
        _victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        _victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        _victoryScreen.SetActive(true);

        ResetBoard();

        FileHandler.SaveToJSON<SaveEntry>(entries);
    }
    public void OnBackButton(){
        if(enableDragging){
             _victoryScreen.SetActive(false);
             
        } else {
            _resultScreen.SetActive(false);
            ResetBoard();
        }
        UIManager.Instance.MenuAnimator.SetTrigger("GameMenu");
    }

    private void ResetBoard(){
        _currentlyDragging = null;
        _availableMoves .Clear();
        _moveList.Clear();

        for(int x = 0; x < TILE_COUNT; x++){
            for (int y = 0; y < TILE_COUNT; y++){
                if(_chessPieces[x,y] != null) Destroy(_chessPieces[x,y].gameObject);
                _chessPieces[x,y] = null;
            }
        }

        for (int i = 0; i < _eatenWhites.Count; i++) Destroy(_eatenWhites[i].gameObject);
        for (int i = 0; i < _eatenBlacks.Count; i++) Destroy(_eatenBlacks[i].gameObject);
        _eatenWhites.Clear();
        _eatenBlacks.Clear();

        SpawnAllPieces();
        PositionPieces();
        _isWhiteTurn = true;

    }

}
