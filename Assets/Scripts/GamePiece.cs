using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/********************************************************************************************
    AUTHOR: DANG CONG THANH
    DATE: 27/09/2022
    Object(s) holding this script: Game Piece 
    Summary: Initialise a game piece and assign it an xIndex and yIndex
    Move the game pieces using Vector3.Lerp
********************************************************************************************/
public class GamePiece : MonoBehaviour
{
    public int xIndex; //the current x-coordinate of the game piece 
    public int yIndex; //the current y-coordinate of the game piece 
    private bool _isMoving = false; //Check if whether the piece are moving right now 

    private PieceManager pieceMana; //a reference to the pieceManger class 

    //the color of the piece, defined in the Inspector using the enum below 
    //the type of the variable must be the same as the name of the enum to link them
    public MatchValue matchValue;

    //Assign a constant to each color of piece 
    //This is used to determine whether pieces match, even if they have different sprites
    public enum MatchValue
    {
        blue,
        green, 
        red,
        orange,
        purple, 
        yellow
    };
    // Start is called before the first frame update
    void Start()
    {
        pieceMana = GameObject.Find("PieceManager").GetComponent<PieceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move((int)this.transform.position.x - 1, (int)this.transform.position.y, 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move((int)this.transform.position.x + 1, (int)this.transform.position.y, 0.5f); 
        }
        
    }
    
    //Initialise the GamePiece to give it access to the PieceManager class 
    //Called by ****
    public void Init(PieceManager pm)
    {
        //set the pieceManager variable to the one passed in to the function 
        pieceMana = pm;
        
    }
    
    //sets the x and the y index to the arguments passed in 
    //Called by PieceManager.PlaceGamePiece() 
    //Called by MoveRoutine() when a game piece is moved 
    public void SetCoord(int x, int y)
    {
        xIndex = x; //set xIndex to the x value passed in by the function call
        yIndex = y; //set yIndex to the y value passed in by the function call
    }
    //Called by ***
    public void Move(int destX, int destY, float timeToMove)
    {
        if (_isMoving == false) //game pieces are currently moving to a new destination 
        {
            //start the MoveRoutine and pass destX, destY values to Vector3
            StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), timeToMove));    
        }
        
        
    }
    //Lerp the game pieces to the destination passed in 
    //over the amount of time passes in timeToMove 
    //called by Move function when the pieces move 
    IEnumerator MoveRoutine(Vector3 destination, float timeToMove)
    {
        //store the start position of the game pieces as variables 
        Vector3 startPosition = this.transform.position;
        
        //bool flag used to determine whether we have arrived at the destination passed in 
        bool reachedDestination = false;
        
        //set IsMoving to true cuz moving has started 
        _isMoving = true;
        //how many secs have passed since moving 
        float elaspedTime = 0f;
        
        //while the gamepiece has not reached it's destination 
        while (reachedDestination == false)
        {
            //determine if we have reached our destination yet by checking 
            //the distance between our current position and the destination
            if (Vector3.Distance(this.transform.position, destination) < 0.01f)
            {
                reachedDestination = true; //break out of the loop 
                if (pieceMana != null)
                {
                    //call the PlaceGamePiece() function to set the pieces final position, to
                    //set it's xIndex and yIndex and to add it to the allGamePieces array
                    //PlaceGamePieces receives 2 ints, so recast the destination x and y as ints 
                    pieceMana.PlaceGamePiece(this, (int)destination.x, (int)destination.y);
                }
                //break out of the while loop immediately to save running all the remaining code
                break;
            }
            
            //increment the amount of time since 
            //the last frame so we can track the total movement time 
            elaspedTime += Time.deltaTime;
            
            //give us a number clamped between 0 and 1 representing how far along 
            //the journey we are (0 is the start, 0.5 is halfway, 1 is the journey complete)
            float t = elaspedTime / timeToMove;
            
            //move the gamepiece to it's new destination based on the current value of t
            this.transform.position = Vector3.Lerp(startPosition, destination, t);
            //wait until next framem 
            yield return null;
            
        }

        _isMoving = false; //game piece is no longer moving 
    }
}
