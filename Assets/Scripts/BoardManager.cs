using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;


public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }
    public Camera camera;
    public GameObject pieceHighlight;
    private bool[,] allowedMoves { set; get; }

    public ChessPiece[,] ChessPieces { set; get; }
    private ChessPiece selectedPiece; 

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman;

    private Material previousMat;
    public Material selectedMat;

    private Quaternion orientation = Quaternion.Euler(-90, -90, 0);

    
    private Quaternion whiteCam = Quaternion.Euler(57, 0, 0);
    private Quaternion blackCam = Quaternion.Euler(57, 180, 0);

    public bool isWhiteTurn = true;

    private void Start()
    {
        Instance = this;
        SpawnAllChessman();
    }

    private void Update()
    {
        UpdateSelection();
        DrawChessboard();

        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if(selectedPiece == null)
                {
                    SelectChesspiece(selectionX, selectionY);
                } else
                {
                    MoveChesspiece(selectionX, selectionY);
                }

            }
        }

    }

    private void SelectChesspiece(int x, int y)
    {
        if (ChessPieces[x, y] == null)
            return;

        if (ChessPieces[x, y].isWhite != isWhiteTurn)
            return;
        bool hasOneMove = false;
        allowedMoves = ChessPieces[x, y].PossibleMove();
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (allowedMoves[i, j])
                    hasOneMove = true;
        if (!hasOneMove)
            return;

        


        selectedPiece = ChessPieces[x, y];
        if (selectedPiece.GetType() == typeof(Pawn))
        {
            if (y == 7)
            {
                activeChessman.Remove(selectedPiece.gameObject);
                Destroy(selectedPiece.gameObject);
                SpawnChessman(1, x, y);
                selectedPiece = ChessPieces[x, y];
            }
            else if (y == 0)
            {
                activeChessman.Remove(selectedPiece.gameObject);
                Destroy(selectedPiece.gameObject);
                SpawnChessman(7, x, y);
                selectedPiece = ChessPieces[x, y];
            }
        }
        
        pieceHighlight.SetActive(true);
        pieceHighlight.transform.position = new Vector3(x+0.5f, 0, y+0.5f);
        BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves);


    }

    private void MoveChesspiece(int x, int y)
    {
        if(allowedMoves[x,y])
        {
            ChessPiece c = ChessPieces[x, y];

            if(c != null && c.isWhite != isWhiteTurn)
            {
                //enemy piece
                if(c.GetType() == typeof(King))
                {
                    EndGame();
                    return;
                }

                activeChessman.Remove(c.gameObject);
                Destroy(c.gameObject);
            }

            ChessPieces[selectedPiece.CurrentX, selectedPiece.CurrentY] = null;
            selectedPiece.transform.position = GetTileCenter(x, y);
            selectedPiece.SetPosition(x, y);
            ChessPieces[x, y] = selectedPiece;
            isWhiteTurn = !isWhiteTurn;
            if (isWhiteTurn)
            {
                camera.transform.position = new Vector3(4.0f, 5.4f, -0.4f);
                camera.transform.rotation = whiteCam;
            }
            else
            {
                camera.transform.position = new Vector3(4.0f, 5.4f, 8.4f);
                camera.transform.rotation = blackCam;
            }
        }
        pieceHighlight.SetActive(false);
        BoardHighlights.Instance.HideHighlights();
        selectedPiece = null;
    }

    private void UpdateSelection()
    {
        if (!Camera.main)
            return;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;

        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }

    }

    private void SpawnChessman(int index, int x, int y)
    {
        if(index == 10)
        {
            GameObject go = Instantiate(chessmanPrefabs[index], GetTileCenter(x,y), Quaternion.Euler(-90, 90, 0)) as GameObject;
            go.transform.SetParent(transform);
            ChessPieces[x, y] = go.GetComponent<ChessPiece>();
            ChessPieces[x, y].SetPosition (x, y);
            activeChessman.Add(go);
        }
        else
        {
            GameObject go = Instantiate(chessmanPrefabs[index], GetTileCenter(x,y), orientation) as GameObject;
            go.transform.SetParent(transform);
            ChessPieces[x, y] = go.GetComponent<ChessPiece>();
            ChessPieces[x, y].SetPosition(x, y);
            activeChessman.Add(go);
        }

    }

    private void SpawnAllChessman()
    {
        activeChessman = new List<GameObject>();
        ChessPieces = new ChessPiece[8, 8];

        //Spawn White 
        //Queen
        SpawnChessman(1, 3,0);
        //King
        SpawnChessman(0, 4,0);
        //Rooks
        SpawnChessman(2, 0,0);
        SpawnChessman(2, 7,0);
        //Bishop
        SpawnChessman(3, 2,0);
        SpawnChessman(3, 5,0);
        //Knight
        SpawnChessman(4, 1,0);
        SpawnChessman(4, 6,0);
        //Pawn
        for(int i = 0; i < 8; i++)
        {
            SpawnChessman(5, i, 1);
        }


        //Spawn Black 
        //Queen
        SpawnChessman(7, 3,7);
        //King
        SpawnChessman(6, 4,7);
        //Rooks
        SpawnChessman(8, 0,7);
        SpawnChessman(8, 7,7);
        //Bishop
        SpawnChessman(9, 2,7);
        SpawnChessman(9, 5,7);
        //Knight
        SpawnChessman(10, 1,7);
        SpawnChessman(10, 6,7);
        //Pawn
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, i, 6);
        }

    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    private void DrawChessboard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for(int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for(int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);
            }
        }

        //Draw Selection
    
        if(selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
            Debug.DrawLine(
                Vector3.forward * (selectionY +1) + Vector3.right * selectionX,
                Vector3.forward * (selectionY) + Vector3.right * (selectionX + 1));
        }
    }

    private void EndGame()
    {
        if (isWhiteTurn)
            Debug.Log("White wins");
        else
            Debug.Log("Black wins");

        foreach (GameObject go in activeChessman)
            Destroy(go);
        isWhiteTurn = true;
        BoardHighlights.Instance.HideHighlights();
        SpawnAllChessman();
        camera.transform.position = new Vector3(4.0f, 5.4f, -0.4f);
        camera.transform.rotation = whiteCam;
    }
}
