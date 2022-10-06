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
    }
}
