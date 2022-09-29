using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;

/********************************************************************************************
    AUTHOR: DANG CONG THANH
    DATE: 27/09/2022
    Object(s) holding this script: Tile
    Sumarry: Initialise a tile and assign it an xIndex, yIndex and the board it is on 
********************************************************************************************/

public class Tile : MonoBehaviour
{
    public int xIndex; //the x location of the tile 
    public int yindex; //the y location of the tile 

    private PieceManager pieceManager;

    private Board boardScript;
    // Start is called before the first frame update
    void Start()
    {
        //get the reference to the PieceManager class
        pieceManager = GameObject.Find("PieceManager").GetComponent<PieceManager>();
    }
    //Sets the xIndex, yIndex and boardScript variables to the ones passed in
    //We are passing in a boardScript in case later we want more than one boardScript in our game 
    //Called by Board.SetupTiles when the level start
    public void Init(int x, int y, Board board)
    {
        xIndex = x; //store the x-location of the tile as the x variable passed in by Board.SetupTiles()
        yindex = y; //store the y-location of the tile as the y variable passed in by Board.SetupTiles()
        boardScript = board; //store the Board script of the tile as the boardScript variable passed in by Board.SetupTiles()
    }

}
