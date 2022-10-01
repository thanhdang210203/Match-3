using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/********************************************************************************************
    AUTHOR: DANG CONG THANH
    DATE: 27/09/2022
    Object(s) holding this script: Board
    Sumarry: Creates a grid of tiles names them and tiles them to the Tiles GameObject
    THIS IS WHERE YOU WILL DO ALL THE THINGS THIS CLASS IS RESPONSIBLE FOR AS YOU WRITE THEM 
********************************************************************************************/

public class Board : MonoBehaviour
{
    public int width; //board width 
    public int height; //board height
    public GameObject tileNormalPrefab; //a standard tile with the sprite attached  
    private Tile[,] AllTiles; //a 2 dimensional array holding all the board tile scripts

    // Start is called before the first frame update
    void Start()
    {
        AllTiles = new Tile[width, height]; //construct a new array of size
        SetupTiles(); //calls the method below to construct the empty board
    }
    //Instantiate a grid of tiles, rename the tiles, parent them to the Baord>Tiles objects 
    //In the Hierarchy and adds their Tile scripts to the allTiles array 
    //Called in Start()
    void SetupTiles()
    {
        for(int row = 0; row < width; row++)
        {
            for(int col = 0; col < height; col++)
            {
                //instantiate the tile prefab at coordinates row and col
                //Instantiate() constrcucts an Object, so this 'cast' it instead as a GameObject
                //A tile is 512x512 and 512 pixel per unit, and so is exactly 1 unit square 
                GameObject tile = Instantiate(tileNormalPrefab, new Vector3(row, col, 0), Quaternion.identity) as GameObject;

                //Set the tile name to it's coordinate
                tile.name = "Tile (" + row + "," + col + ")";

                //Store the tilePrefabs Tile scrpit at the appropriate position in the array 
                AllTiles[row, col] = tile.GetComponent<Tile>();

                //To keep things tidy, parent tiles to the Pieces object in the Hierarchy
                tile.transform.parent = GameObject.Find("Tiles").transform;
                
                //call the Init method on the tile and pass it row and col (which becoem Tile.xIndex and 
                //Tile.yIndex and pass it a reference to the board which becomes Tile.boardScript;
                AllTiles[row, col].Init(row, col, this);
                
            }
        }
    }
}
