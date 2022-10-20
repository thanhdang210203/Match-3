using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting; //used so we can combine lists into a single list using the Union() method

/********************************************************************************************
    AUTHOR: DANG CONG THANH
    DATE: 27/09/2022
    Object(s) holding this script: Match manager
    Sumarry: 
    Find matches using FinsMatchesAt()
    Highlight matches using HighlightMatches() 
    Turn off highlight using HighlightTilesOff()       
    THIS IS WHERE YOU WILL DO ALL THE THINGS THIS CLASS IS RESPONSIBLE FOR AS YOU WRITE THEM 
********************************************************************************************/
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
        
        HightlightMatches();
        
        Debug.Log("Reading IsWithinBound() as: " + _pieceManager.IsWithinBounds(0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Looks for matches and then stores matching pieces in a list which it then returns to the function calling this
    //minLength has a default value which makes it an optional parameter. If I dont pass a fourth argument in, it will use 3
    //called by FindVerticalMatches() to search for vertical matches from the start piece 
    //called by FindHorizontalMatches() to search for horizontal matches from the start piece 
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
            //we clamp it as a safety check to make sure it never goes outside of -1 and 1
            //and still searches one piece at a time in either direction 
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            //if the nextX or nextY is outside the board, our search is finished so break out of the loop
            if (_pieceManager.IsWithinBounds(nextX, nextY) == false)
            {
                break;
            }

            //if the next piece is within the bounds of the board create a 
            //variable to store the next piece
            GamePiece nextPiece = _pieceManager.allGamePieces[nextX, nextY];
            
            //check to see if our game piece is null 
            if (nextPiece == null)
            {
                break;
            }
            else
            {
                //if the nextPiece matches the startPiece and the matches list 
                //doesn't already have the next piece 
                if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece))
                {
                    //We already have a match!
                    //Add the next game piece to our list of matches 
                    matches.Add(nextPiece);
                }
                else //the next piece isnt a match 
                {
                    //breakout of the loop
                    break;
                }
            }
        }

        //if the number of matches meets or exceeds the required amount
        if (matches.Count >= minLength)
        {
            //return our list of matches 
            return matches;
        }
        else
        {
            //else return a null list (all paths must return something)
            return null;
        }

    }

    //Calls FindMatches() and passes in a direction to search upward and then downwards
    //we pass in a min length of 2 (not 3) because it is possible to match 3
    //by making one above the start piece and one below (we will combine these two Lists later)
    List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

        //we cannot use the System.Linq.Union() method to combine two Lists if any 
        //of them are full, so if they are, we need to set them to empty list 
        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        //use the System.Linq.Union() method to combine the two Lists 
        //var will default to the first data type that is put in it (a List)
        //Union() returns an IEnumerable, so we use ToList() to make combined matches a List
        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();
        
        //check whether the combined matches List is long enough for a winning match (by default 3)
        if (combinedMatches.Count >= minLength)
        {
            return combinedMatches;
        }
        else //we dont have enough matches so return a null list 
        {
            //all paths must return something 
            return null;
        }
    }
    
    //Calls FindMatches() and searches in a left and right direction 
    //Returns a list of all horizontal matches
    //Called by
    List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        //calls FindMatches() and passes in a direction to search right and then left
        //we pass in a min Length of 2 (not 3) because it is possible to match 3
        //by matching to the left of the start piece and one to the right 
        //(we will combine these two lists later)
        List<GamePiece> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<GamePiece> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);
        
        //we cannot use System.Linq.Union() method to combine two Lists if any of them are full
        //so if they are, we need to set them to empty list
        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }
        
        //use the System.Linq.Union() method to combine the two Lists 
        //var will default to the first data type that is put in it (a List)
        //Union() returns an IEnumerable, so we use ToList() to make combined matches a List
        var combinedMatches = rightMatches.Union(leftMatches).ToList();

        if (combinedMatches.Count >= minLength)
        {
            return combinedMatches;
        }
        else
        {
            return null;
        }
    }

    
    //Highlight the tiles on matched game pieces 
    //Called by MatchManager.Start()
    void HightlightMatches()
    {
        //loop through the width of the board
        for (int row = 0; row < _board.width; row++)
        {
            //loop through the height of the board
            for (int col = 0; col < _board.height; col++)
            {
                //only runs the loop is row and col are inside the board
                if (_pieceManager.IsWithinBounds(row, col) == false)
                {
                    break;
                }

                HighlightMatchesAt(row, col);
            }
        }
    }

    //checks for matches at the coordinate passed in as arguments, and
    //if it finds them, calls HighlightTilesOn() to highlight the match 
    //Called by HighlightMatches() when checking the whole board for matches
    public void HighlightMatchesAt(int row, int col)
    {
        //turn of highlights on the whole board so we are ready to highlight the current matches 
        HighlightTilesOff(row, col);

        //return a list of all matches 
        List<GamePiece> allMatches = FindMatchesAt(row, col);

        foreach (GamePiece matchPiece in allMatches)
        {
            HighlightTilesOn(matchPiece.xIndex, matchPiece.yIndex);
        }
    }

    //Finds horizontal and vertical matches and combines them into 
    //an allMatches List that is then returned 
    //Call by HightLightMatches to highlight matching pieces
    public List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3)
    {
        //safety check to make sure that we definitely have PieceManager and Board
        _pieceManager = GameObject.Find("PieceManager").GetComponent<PieceManager>();
        _board = GameObject.Find("Board").GetComponent<Board>();
        
        List<GamePiece> horizMatches = FindHorizontalMatches(x, y, minLength);
        List<GamePiece> vertMatches = FindVerticalMatches(x, y, minLength);

        //defensive programming
        if (horizMatches == null)
        {
            horizMatches = new List<GamePiece>();
        }

        //defensive programming
        if (vertMatches == null)
        {
            vertMatches = new List<GamePiece>();
        }

        //combine horizMatches and vertMatches using Union
        List<GamePiece> allMatches = horizMatches.Union(vertMatches).ToList();
        return allMatches;
    }

    //Resets the color and transparency of the tile at the coordinates passed
    //in so that the highlighting disappears and it returns to normal
    //called by HighlightMatchesAt()
    private void HighlightTilesOff(int x, int y)
    {
        //Change the color of the tiles back to white 
        _board.AllTiles[x, y].gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        
        //minus 0 in red, 0 in blue, 0 in green and 0.375 in alpha to return the tile to an alpha of 160
        _board.AllTiles[x, y].gameObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.375f);
    }
    
    //called by HighlightMatchesAt()
    private void HighlightTilesOn(int x, int y)
    {
        //Change the color of the tiles back to white 
        _board.AllTiles[x, y].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        //call FindMatches() to populate 2 Lists of matches 
        List<GamePiece> leftMatches = FindMatches(x, y, new Vector2(-1, 0), minLength);
        List<GamePiece> downwardMatches = FindMatches(x, y, new Vector2(0, -1), minLength);

        //defendsive programing
        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }
        //defendsive programing
        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        //check whether leftMatches OR downwardMatches have anything in them
        if (leftMatches.Count > 0 || downwardMatches.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
