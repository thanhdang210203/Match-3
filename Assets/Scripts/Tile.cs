using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;

/********************************************************************************************
    AUTHOR: DANG CONG THANH
    DATE: 27/09/2022
    Object(s) holding this script: Initialise a tile and assign it an xIndex, yIndex and 
    the board it is on / Handles mouse input events and send them to the PieceManager class
    Summary: Initialise a tile and assign it an xIndex, yIndex and the board it is on 
********************************************************************************************/

public class Tile : MonoBehaviour
{
    public int xIndex; //the x location of the tile 
    public int yIndex; //the y location of the tile 
    
    private PieceManager PieceManager;
    
    private Board boardScript;
    // Start is called before the first frame update
    void Start()
    {
        //get the reference to the PieceManager class
        PieceManager = GameObject.Find("PieceManager").GetComponent<PieceManager>();
    }
    //Sets the xIndex, yIndex and boardScript variables to the ones passed in
    //We are passing in a boardScript in case later we want more than one boardScript in our game 
    //Called by Board.SetupTiles when the level start
    public void Init(int x, int y, Board board)
    {
        xIndex = x; //store the x-location of the tile as the x variable passed in by Board.SetupTiles()
        yIndex = y; //store the y-location of the tile as the y variable passed in by Board.SetupTiles()
        boardScript = board; //store the Board script of the tile as the boardScript variable passed in by Board.SetupTiles()
    }
    
    //a tile has been clicked on 
    private void OnMouseDown() //Tile location not appearing in Debug mode
    {
        if (PieceManager == null)
        {
            //call ClickTile() and pass in the tile that has been clicked 
            PieceManager.ClickTile(this);
        }
    }
    
    //A tile has been entered by the mouse 
    private void OnMouseOver() //Tile location not appearing in Debug mode
    {
        if (PieceManager == null)
        {
            //call DragToTile() and pass in the tile that has been rolled over 
            PieceManager.DragToTile(this);
        } 
    }
    
    //the mouse has been released over a tile
    private void OnMouseUp() //Tile location not appearing in Debug mode
    {
        if (PieceManager == null)
        {
            //call ReleaseTIle() to switch between the clickedTile and the targetTile 
            PieceManager.ReleaseTile();
        }   
    }
}
