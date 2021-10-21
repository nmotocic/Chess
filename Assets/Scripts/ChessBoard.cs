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

    //Art
    [Header("Art")]
    [SerializeField] private Material _tileMaterial;
    [SerializeField] private float _tileSize = 1.0f;
    [SerializeField] private float _yOffset = 0.2f;
    [SerializeField] private Vector3  _boardCenter = Vector3.zero;
    [SerializeField] private float _eatenSize = 0.3f;
    [SerializeField] private float _deathSpacing = 0.3f;
    [SerializeField] private float _dragOffset = 0.75f;
    [SerializeField] private GameObject _victoryScreen;

    [Header("Prefabs")]
    [SerializeField] private GameObject[] _prefabs;
    [SerializeField] private Material[] _teamMaterials;

    private void Awake() {
        _isWhiteTurn = true;
        GenerateGrid(_tileSize, TILE_COUNT);
        SpawnAllPieces();
        PositionPieces();
    }

    private void Update() {
        if(!_currentCamera){
            _currentCamera = Camera.main;
            return;
        }

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
    private bool MoveTo(ChessPiece chessPiece, int x, int y)
    {
        if(!ContainsValidMove(ref _availableMoves, new Vector2Int(x,y))) return false;

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
        _moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x,y) } );

        ProcessSpecialMove();

        return true;
    }

    private void CheckMate(int team)
    {
        DisplayVicotry(team);
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
                }
            }
        }
    }

    //UI
    private void DisplayVicotry(int winningTeam){
        _victoryScreen.SetActive(true);
        _victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }

    public void OnResetButton(){
        _victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        _victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        _victoryScreen.SetActive(true);

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

    public void OnExitButton(){

    }

}
