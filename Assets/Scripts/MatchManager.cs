using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    private PieceManager _pieceManager; //a reference to the PieceManager class 

    private Board _board; //a reference to the Board class 
    // Start is called before the first frame update
    void Start()
    {
        //get access to the PieceManager class
        _pieceManager = GameObject.Find("PieceManager").GetComponent<PieceManager>();
        
        //get access to the Board class
        _board = GameObject.Find("Board").GetComponent<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Looks for matches and then stores matching pieces in a list which it then returns to the function calling this
    //minLength has a default value which makes it an optional parameter. If I dont pass a fourth argument in, it will use 3
    //called by ***
    List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        //create a list that will be returned if an appropriate match is found 
        List<GamePiece> matches = new List<GamePiece>();
        
        //if we are using a starting x and y position that is outside the bounds of our board
        if (_pieceManager.IsWithinBounds(startX, startY) == false)
        {
            //return a null list and break out of the function
            return null;
        }
        
        //get a reference to the game piece we will be starting our check for matches from
        GamePiece startPiece = _pieceManager.allGamePieces[startX, startY];
        
        //defensive programming to check that the startPiece is a valid piece 
        if (startPiece != null)
        {
            //add the startPiece as the first element of the matches list 
            matches.Add(startPiece);
        }
        else
        {
            //return a null list and break out of the function
            return null;
        }
        
        //create two variables that will hold the next position we will search in 
        int nextX; //the x-coordinates of the next tile to search 
        int nextY; //the y-coordinates of the next tile to search 
        int maxSearches; //the max num of searches that we will do 
        
        //set the value of the maximum number of searches we will do either 
        //the width or height of the board (which ever is the greater)
        if (_board.width > _board.height)
        {
            maxSearches = _board.width;
        }
        else
        {
            maxSearches = _board.height;
        }

        //create a loop to start the search for matches 
        //we don't need to start at zero because the startPiece is zero
        //we also dont need to go all the way to maxSearches because the startPiece is one of them
        for (int i = 1; i < maxSearches; i++)
        {
            //search direction is a Vector2, (1,0), (-1, 0), (0, 1) or (0, -1)
            //so if searchDirection.x = 1 or -1, and searchDirection.y = 0 multiplying by i
            //will search each piece in the same row as the startPiecce.
            //if searchDirection.x = 0, and searchDirection.y = 1 or -1, multiplying it by i 
            //will search up and down and not change the x
            nextX = startX + (int) Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int) Mathf.Clamp(searchDirection.y, -1, 1) * i;
            
            //if the nextX or nextY is outside the board, our search is finished so break out of the loop
            if (_pieceManager.IsWithinBounds(nextX, nextY) == false)
            {
                break;
            }
            
            //if the next piece is within the bounds of the board create a 
            //variable to store the next piece
            GamePiece nextPiece = _pieceManager.allGamePieces[nextX, nextY];
            
            //if the nextPiece matches the startPiece 
            if (nextPiece.matchValue == startPiece.matchValue)
            {
                
            }
        }
        return null;
    }
}
