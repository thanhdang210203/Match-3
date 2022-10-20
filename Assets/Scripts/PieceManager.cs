using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

/********************************************************************************************
    AUTHOR: DANG CONG THANH
    DATE: 27/09/2022
    Object(s) holding this script: Piece Manager
    Summary: Fills the board at the start with random pieces / Handles the switching of game pieces
    THIS IS WHERE YOU WILL DO ALL THE THINGS THIS CLASS IS RESPONSIBLE FOR AS YOU WRITE THEM 
********************************************************************************************/



public class PieceManager : MonoBehaviour
{
    public GameObject[] gamePiecePrefab; //an array of all the game pieces in the game as GameObject
    public GamePiece[,] allGamePieces; //a 2-dimensional array holding all the game piece's GamePiece scripts
    [SerializeField] private GamePiece _clickedPiece; //a reference to the GamePiece() class 
    [SerializeField] private GamePiece _targetPiece; //a reference to the GamePiece() class 
    [SerializeField] private Tile clickedTile; //the tile player clicks on first to move the game piece 
    [SerializeField] private Tile targetTile; //the tile the player wan t the game piece to move to 
    private Board board; //reference to the Board class 
    private MatchManager _matchManager; //reference to the match manager class
    public float swapTime = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        board = GameObject.Find("Board").GetComponent<Board>(); //store the Board class 
        _matchManager = GameObject.Find("MatchManager").GetComponent<MatchManager>(); //store the MatchManager class
        allGamePieces = new GamePiece[board.width, board.height]; //construct a new array of size width and height
        FillBoard();
        // ClearPiecesAt(1, 1);
        // ClearPiecesAt(3, 5);
    }

    //Returns a random game piece from the GamePiecesPrefab array
    //Called by ****
    GameObject GetRandomGamePiece()
    {
        //get a random number between 0 and all the game pieces -1 in the gamePiecesPrefab array
        //the .Length property of an array is not inclusive to the final number 
        int randomIdx = Random.Range(0, gamePiecePrefab.Length);

        //safety check to make sure the array is populated in the Inspector panel 
        if (gamePiecePrefab[randomIdx] == null)
        {
            Debug.LogWarning("WARNING: Element " + randomIdx + " in the GamePiecesPrefab array is reading as null!");
        }

        //return the selected game piece to the function calling it
        return gamePiecePrefab[randomIdx];
    }

    //Places a game piece at the x and y passed in 
    //Adds the game piece passed in to the allGamePiece array
    //called by FillBoard() below when the board is first filled with pieces 
    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        //safety check to make sure the gamePiece passed in has a value 
        if (gamePiece == null)
        {
            Debug.LogWarning("PieceManager: Invalid gamePiece!");
            return; //break out of the method so the line dont run
        }

        //move the game piece passed into the bracket by the function call to the x and y passed in 
        gamePiece.transform.position = new Vector3(x, y, 0);

        //set the rotation back to zero if accidentally rotate it 
        gamePiece.transform.rotation = Quaternion.identity;

        //calls the function to see if the x and the y are in the boundary of the board
        //IsWithinBounds returns true or false
        if (IsWithinBounds(x, y) == true)
        {
            //assign the gamePiece to the correct place in the allGamePieces array
            allGamePieces[x, y] = gamePiece;
        }

        //call the SetCoord() method to populate GamePiece.xIndex and GamePiece.yIndex variable 
        gamePiece.SetCoord(x, y);

    }

    //Function that returns true or false depending on the x, y coordinates
    //passed in are within the boundary of the board 
    //Called by PlaceGamePiece() above when adding a piece to the allGamePieces array 
    //Called by MatchManager.FindMatches() to check the piece we start the search from is within the board
    //Called by MatchManager.FindMatches() to check whether the next piece we are checking for matches is within the board
    public bool IsWithinBounds(int x, int y)
    {
        //checks to make sure if x is between 0 and the width -1 and y is 
        //with 0 and the height -1
        //return true or false 
        if (x >= 0 && x < board.width && y >= 0 && y < board.height)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void FillBoard()
    {
        int maxLoops = 100;
        int loops = 0;
        for (int row = 0; row < board.width; row++)
        {
            for (int col = 0; col < board.height; col++)
            {
                GamePiece piece =  FillRandomAt(row, col);
                
                //keep looping until the game piece at row, col has no matches
                //HasMatchOnFill() returns true when the random piece made has matches
                while (_matchManager.HasMatchOnFill(row, col) == true)
                {
                    //clear the starting piece that has matches 
                    ClearPiecesAt(row, col);
                    
                    //Place a new random  game piece with FillRandomAt()
                    piece = FillRandomAt(row, col);
                    if (loops > maxLoops)
                    {
                         //add one to the number of loops
                         loops++;
                         Debug.LogWarning("Warning: FillBoard has exceeded the maximum of loops!");
                         break;
                    }
                }
            }
        }
    }

    //Put a random game piece at the coordinates passed in as arguments 
    //Called by FillBoard() when the board is filled at the start of the game
    private GamePiece FillRandomAt(int row, int col)
    {
        GameObject randomPieces = GetRandomGamePiece();
        //instantiate the gamePiecesPrefab at coordinates row and col
        //Instantiate() constructs an Object, so this 'cast' it instead as a GameObject
        GameObject randomPiece = Instantiate(randomPieces, new Vector3(row, col, 0), Quaternion.identity) as GameObject;

        //Set the tile name to it's coordinate
        randomPiece.name = "Piece (" + row + "," + col + ")";

        //Store the gamePiecesPrefab GamePieces script at the appropriate position in the array 
        allGamePieces[row, col] = randomPiece.GetComponent<GamePiece>();
        
        //call the SetCoord method on the tile and pass it row and col (which become SetCoord.xIndex and 
        //SetCoord.yIndex
        allGamePieces[row, col].SetCoord(row, col);

        //Defensive programming to make sure the randomPiece returned is a value
        if (randomPiece != null)
        {
            //move the game piece to the game pieces sorting layer it appears in front of the line 
            randomPiece.GetComponent<SpriteRenderer>().sortingLayerName = "Pieces";
            
            //parent gamePiece to the pieces object in the Hierarchy
            randomPiece.transform.parent = GameObject.Find("Pieces").transform;
            
            //Initialise the GamePiece to give it access to the PieceManager 
            randomPiece.GetComponent<GamePiece>().Init(this);
            
            //if it is valid, place it at the row and col of the current loop 
            PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), row, col);

            //return the GamePiece to the function calling this
            return randomPiece.GetComponent<GamePiece>();
        }

        return null;
    }

    //set the clickedTile variable to the tile passed in 
    //called by Tile.OnMouseDown() when a tile is clicked
    public void ClickTile(Tile tile)
    {
        //tiles that can be clicked always = to null
        if (clickedTile == null)
        {
            //set clickedTile to tile passed by in ****
            clickedTile = tile;
            Debug.Log("Clicked tile" + tile.name);
        }
    }

    //if a tile has been clicked, set the targetTile variable to the on passed in 
    //called by Tile.OnMouseOver() when a tile has been entered by the mouse 
    public void DragToTile(Tile tile)
    {
        //if there is a tile that has been clicked on
        if (clickedTile != null && IsNextTo(clickedTile, tile))
        {
            //set the target tile to the tile passed in 
            targetTile = tile;
            Debug.Log("Target Tile: " + targetTile.name);
        }
    }

    //Checks if clickedTile and targetTile are valid tiles and 
    //calls SwitchTile() below to swap their places
    //called by ****
    public void ReleaseTile()
    {
        //ClickedTile and TargetTile are booth valid tiles
        if (clickedTile != null && targetTile != null)
        {
            //call SwitchTile() below to switch the two's position
            SwitchTile(clickedTile, targetTile);
            //Debug.Log("tt " + targetTile.name);
        }

        //reset the clickedTile and targetTile so tiles can be clicked again 
        clickedTile = null;
        targetTile = null;
    }

    void SwitchTile(Tile tileClicked, Tile tileTargeted)
    {
        StartCoroutine(SwitchTilesRoutine(tileClicked, tileTargeted));
    }

    bool IsNextTo(Tile startTile, Tile endTile)
    {
        int xStart = startTile.xIndex;
        int xEnd = endTile.xIndex;
        int yStart = startTile.yIndex;
        int yEnd = endTile.yIndex;

        //Move 1 to the left 
        if (Mathf.Abs(Mathf.Abs(xStart) - Mathf.Abs(xEnd)) == 1 && Mathf.Abs(yStart - yEnd) == 0)
        {
            Debug.Log("Moving left right");
            return true;
        }

        //Move 1 up
        if (Mathf.Abs(Mathf.Abs(yEnd) - Mathf.Abs(yStart)) == 1 && Mathf.Abs(xStart - xEnd) == 0)
        {
            Debug.Log("Moving up down");
            return true;
        }

        return false;

    }


    //switches the places of the clickedTile and the targetTile 
    IEnumerator SwitchTilesRoutine(Tile tileClicked, Tile tileTargeted)
    {
        _clickedPiece = allGamePieces[tileClicked.xIndex, tileClicked.yIndex];

        _targetPiece = allGamePieces[tileTargeted.xIndex, tileTargeted.yIndex];

        if (_clickedPiece != null && _targetPiece != null)
        {
            //Move the clicked piece to the x and y of the targeted piece
            _clickedPiece.Move(_targetPiece.xIndex, _targetPiece.yIndex, swapTime);
            //move the targeted piece to the x and y of the clicked piece 
            _targetPiece.Move(_clickedPiece.xIndex, _clickedPiece.yIndex, swapTime);

            //yield so the pieces can move and the array updates with the new positions 
            yield return new WaitForSeconds(swapTime);

            //return a list of matches for the clicked piece 
            List<GamePiece> tileClickedMatches = _matchManager.FindMatchesAt(tileClicked.xIndex, tileClicked.yIndex);

            //return a list of matches for the targeted piece
            List<GamePiece> tileTargetedMatches = _matchManager.FindMatchesAt(tileTargeted.xIndex, tileTargeted.yIndex);

            //if neither of the list have anything in them, we havent made a match
            if (tileClickedMatches.Count == 0 && tileTargetedMatches.Count == 0)
            {
                //move the clicked piece back to the clicked tile(it's original location)
                _clickedPiece.Move(tileClicked.xIndex, tileClicked.yIndex, swapTime);

                //move the target piece back to the target tile (it's original location)
                _targetPiece.Move(tileTargeted.xIndex, tileTargeted.yIndex, swapTime);

                //yield so the pieces can move back and the array updates with the new position 
                yield return new WaitForSeconds(swapTime);
            }
            else
            {
                ClearPiecesAt(tileClickedMatches);
                ClearPiecesAt(tileTargetedMatches);
            }
        }


    }

    //Clears matched pieces at the location passed in 
    //Called by ClearBoard() to clear the entire board
    void ClearPiecesAt(int x, int y)
    {
        //store the piece at the x and the y arguments in a variable 
        GamePiece pieceToClear = allGamePieces[x, y];

        //check we have a game piece that is valid at that location 
        if (pieceToClear != null)
        {
            //destroy the piece to clear
            Destroy(pieceToClear.gameObject);

            //set that location of the allGamePieces array to null
            allGamePieces[x, y] = null;
        }
    }

    //overloaded version of CLearPiecesAt(int x, int y) above
    //Our program will know which one to use,depending on 
    //the value passed in 
    //Clear out a list of GamePieces passed in as arguments 
    //Called by 
    void ClearPiecesAt(List<GamePiece> gamePieces)
    {
        //loop through the list passed in 
        foreach (GamePiece piece in gamePieces)
        {
            //call the original ClearPiecesAt() and pass in
            //The x and the y of each piece in the list 
            ClearPiecesAt(piece.xIndex, piece.yIndex);
        }
    }
}
