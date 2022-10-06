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
    [SerializeField] private Tile targetTile; //the tile the player want the game piece to move to 
    private Board board; //reference to the Board class 
    public float swapTime = 0.5f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        allGamePieces = new GamePiece[board.width, board.height]; //construct a new array of size width and height
        FillRandom();   
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    //called by FillRandom() below when the board is first filled with pieces 
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
    private void FillRandom()
    {
        for (int row = 0; row < board.width; row++)
        {
            for (int col = 0; col < board.height; col++)
            { 
                GameObject randomPieces = GetRandomGamePiece();
                //instantiate the gamePiecesPrefab at coordinates row and col
                //Instantiate() constructs an Object, so this 'cast' it instead as a GameObject
                GameObject randomPiece = Instantiate(randomPieces, new Vector3(row, col, 0), Quaternion.identity) as GameObject;
                
                //Set the tile name to it's coordinate
                randomPiece.name = "Piece (" + row + "," + col + ")";
                
                //Store the gamePiecesPrefab GamePieces script at the appropriate position in the array 
                allGamePieces[row, col] = randomPiece.GetComponent<GamePiece>();
                
                //parent gamePiece to the pieces object in the Hierarchy
                randomPiece.transform.parent = GameObject.Find("Pieces").transform;
                
                //call the SetCoord method on the tile and pass it row and col (which become SetCoord.xIndex and 
                //SetCoord.yIndex
                allGamePieces[row, col].SetCoord(row, col);

                //Defensive programming to make sure the randomPiece returned is a value
                if (randomPiece != null)
                {
                    //Initialise the GamePiece to give it access to the PieceManager 
                    randomPiece.GetComponent<GamePiece>().Init(this);
                }
            }
        }
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
       
        //add code to switch tiles
        if (_clickedPiece == null) //wont run if _clickedPiece is null 
        {
            _clickedPiece = allGamePieces[tileClicked.xIndex, tileClicked.yIndex];
            Debug.Log("Clicking piece");
        }

        if (_targetPiece == null)
        {
            _targetPiece = allGamePieces[tileTargeted.xIndex, tileTargeted.yIndex];
            Debug.Log("Targeting piece");
        }

        if (_clickedPiece != null && _targetPiece != null)
        {
            _clickedPiece.Move(_targetPiece.xIndex, _targetPiece.yIndex, swapTime);
            _targetPiece.Move(_clickedPiece.xIndex, _clickedPiece.yIndex, swapTime);
            Debug.Log("Switching");
            _clickedPiece = null;
            _targetPiece = null;
        }
        
        //reset the two so they can be click again 
       
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
        if (Mathf.Abs(Mathf.Abs(yEnd) - Mathf.Abs(yStart))  == 1 && Mathf.Abs(xStart - xEnd) == 0)
        {
            Debug.Log("Moving up down");
            return true;
        } 
       
        return false;

    }
}
