using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

/********************************************************************************************
    AUTHOR: DANG CONG THANH
    DATE: 27/09/2022
    Object(s) holding this script: Piece Manager
    Sumarry: Randomly spawn pieces in the tiles position
    THIS IS WHERE YOU WILL DO ALL THE THINGS THIS CLASS IS RESPONSIBLE FOR AS YOU WRITE THEM 
********************************************************************************************/



public class PieceManager : MonoBehaviour
{
    public GameObject[] gamePiecePrefab; //an array of all the game pieces in the game as GameObject
    private GamePiece[,] allGamePieces; //a 2-dimensional array holding all the game piece's GamePiece scripts
    private Board board; //reference to the Board class 
    
   
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
    //called by ****
    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        //safety check to make sure the gamepiece passed in has a value 
        if (gamePiece == null)
        {
            Debug.LogWarning("PieceManager: Invalid gamePiece!");
            return; //break out of the method so the line dont run
        }
        
        //move the game piece passed into the bracket by the function call to the x and y passed in 
        gamePiece.transform.position = new Vector3(x, y, 0);
        
        //set the rotation back to zero if accidentally rotate it 
        gamePiece.transform.rotation = Quaternion.identity;
        
        //call the SetCoord() method to populate GamePiece.xIndex and GamePiece.yIndex variable 
        gamePiece.SetCoord(x, y);
        
    }

    void FillRandom()
    {
        
        for (int row = 0; row < board.width; row++)
        {
            for (int col = 0; col < board.height; col++)
            {
                //instantiate the gamePiecesPrefab at coordinates row and col
                //Instantiate() constrcucts an Object, so this 'cast' it instead as a GameObject
                GameObject pieces = Instantiate(GetRandomGamePiece(), new Vector3(row, col, 0), Quaternion.identity) as GameObject;
               
                //Set the tile name to it's coordinate
                pieces.name = "Piece (" + row + "," + col + ")";
                
                //Store the gamePiecesPrefab GamePieces scrpit at the appropriate position in the array 
                allGamePieces[row, col] = pieces.GetComponent<GamePiece>();
                
                //parent gamePiece to the pieces object in the Hierarchy
                pieces.transform.parent = GameObject.Find("Pieces").transform;
                
                //call the SetCoord method on the tile and pass it row and col (which become SetCoord.xIndex and 
                //SetCoord.yIndex
                allGamePieces[row, col].SetCoord(row, col);

                if (pieces == null)
                {
                    Debug.LogWarning("Piece error!");
                    return;
                }
                
                
            }
        }
    }
}
