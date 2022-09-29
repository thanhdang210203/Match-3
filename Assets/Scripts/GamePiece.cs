using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/********************************************************************************************
    AUTHOR: DANG CONG THANH
    DATE: 27/09/2022
    Object(s) holding this script: Game Piece 
    Sumarry: Initialise a tile and assign it an xIndex, yIndex and the board it is on 
********************************************************************************************/
public class GamePiece : MonoBehaviour
{
    public int xIndex; //the current x-coordinate of the game piece 
    public int yindex; //the current y-coordinate of the game piece 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //sets the x and the y index to the arguments passed in 
    //Called by ***
    public void SetCoord(int x, int y)
    {
        xIndex = x; //set xIndex to the x value passed in by the function call
        yindex = y; //set yIndex to the y value passed in by the function call
    }
}
