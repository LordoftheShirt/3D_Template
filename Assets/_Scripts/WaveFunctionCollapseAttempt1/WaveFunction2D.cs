using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
/*
public class WaveFunction2D : MonoBehaviour
{
    
    [SerializeField] int columns = 9;
    [SerializeField] int rows = 9;
    public TileData[,] levelGrid;
    [SerializeField] GameObject pin;

    /*
    private List<TileRotation> upAlternatives;
    private List<TileRotation> downAlternatives;
    private List<TileRotation> rightAlternatives;
    private List<TileRotation> leftAlternatives;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelGrid = new TileData[rows, columns];
        PopulateEmptyGrid();

        ProbeTile(0, 1);
        collapseTile(0, 0, 1, 1);
        ProbeTile(0, 1);
        collapseTile(0, 1);
        collapseTile(1, 0);
        // acts completely seperately. Exists just for visualizations sake.
        PlacePins();
    }

    void ProbeTile(int row, int column)
    {
        foreach(TileRotation var in levelGrid[row, column].allAlternatives)
        {
            //print("Tile at coordinates: " + row + " " + column + "\n");
            //print("permitted tileType: "  + var.tileType + " rotation: " + var.rotation + "\n");
        }
        //print("BIRDUP---------------------------------\n");
    }

    void collapseTile(int row = -1, int column = -1, int tileIndex = -1, int rotation = -1)
    {
        //simply fail safe
        if (levelGrid[row, column].allAlternatives.Count == 0)
        {
            print("allAlternatives tile count was 0 for r, c: " + row + ", " + column);
            return;
        }

        // This whole portion simply randomizes the parameters in case none are inputed.
        if (row == -1)
            row = Random.Range(0, rows);
        if (column == -1)
            column = Random.Range(0, columns);
        if (tileIndex == -1)
            tileIndex = Random.Range(0, levelGrid[row, column].allAlternatives.Count);

        print("\ntileIndex: " + tileIndex);
        WFC_2D_Scriptable gatheredTile = ResourceSystem.Instance.GetWaveFunctionPart(levelGrid[row, column].allAlternatives[tileIndex].tileType);

        if (rotation == -1)
            rotation = Random.Range(0, gatheredTile.rotation90.Length);

        rotation = levelGrid[row, column].allAlternatives[tileIndex].rotation;
        //-------------------------------


        // collapses the tile;
        //levelGrid[row, column].allAlternatives.Clear();

        // adds an object in that spot;
        Instantiate(gatheredTile.gameObject, new Vector3(column * 10, 0, row * 10), Quaternion.Euler(0, 90 * rotation, 0) , this.transform);

        //print("row: " + row + " column: " + column + " tiletype: " + (WFCType)tileType + " = " + tileType + " rotation: " + (rotation * 90));

        if (column - 1 >= 0)
        RemovePossibilities(row, column -1, "RightSocket", gatheredTile.rotation90[rotation].right);

        if (column + 1 < columns)
        RemovePossibilities(row, column + 1, "LeftSocket", gatheredTile.rotation90[rotation].left);

        if (row - 1 >= 0)
            RemovePossibilities(row - 1, column, "DownSocket", gatheredTile.rotation90[rotation].down);

        if (row + 1 < rows)
            RemovePossibilities(row + 1, column, "UpSocket", gatheredTile.rotation90[rotation].up);
    }

    void RemovePossibilities(int row, int column, string direction, int socketType)
    {
        // repeated code. It's a bit yuck. It's "LeftSocket" refers to the socket of the neighbour it's comparing with.

            List<TileRotation> removeables = new List<TileRotation>();

            if(direction.Equals("LeftSocket"))
            {
                foreach(TileRotation var in levelGrid[row, column].allAlternatives)
                {
                    if (ResourceSystem.Instance.GetWaveFunctionPart(var.tileType).rotation90[var.rotation].right != socketType)
                    {
                        removeables.Add(var);
                    }
                }
            }

        if (direction.Equals("RightSocket"))
        {
            foreach (TileRotation var in levelGrid[row, column].allAlternatives)
            {
                if (ResourceSystem.Instance.GetWaveFunctionPart(var.tileType).rotation90[var.rotation].left != socketType)
                {
                    removeables.Add(var);
                    //Debug.Log("direction: " + direction + " socketType: " + socketType + " neighbour socket: " + ResourceSystem.Instance.GetWaveFunctionPart(var.tileType).rotation90[var.rotation].left);
                }
            }
            //Debug.Log("row: " + row + " column: " + column + " direction: " + direction + " socketType: " + socketType);
        }

        if (direction.Equals("UpSocket"))
        {
            foreach (TileRotation var in levelGrid[row, column].allAlternatives)
            {
                if (ResourceSystem.Instance.GetWaveFunctionPart(var.tileType).rotation90[var.rotation].down != socketType)
                {
                    removeables.Add(var);
                    Debug.Log("\ndirection: " + direction + " socketType: " + socketType + " neighbour socket: " + ResourceSystem.Instance.GetWaveFunctionPart(var.tileType).rotation90[var.rotation].up);
                }
            }
            Debug.Log("row: " + row + " column: " + column + " direction: " + direction + " socketType: " + socketType);
        }

        if (direction.Equals("DownSocket"))
        {
            foreach (TileRotation var in levelGrid[row, column].allAlternatives)
            {
                if (ResourceSystem.Instance.GetWaveFunctionPart(var.tileType).rotation90[var.rotation].up != socketType)
                {
                    removeables.Add(var);
                }
            }
        }

        levelGrid[row, column].allAlternatives.RemoveAll(tile => removeables.Contains(tile));
        
    }

    void PopulateEmptyGrid()
    {
        TileData defaultTileData = new TileData();
        defaultTileData.allAlternatives = new List<TileRotation>();

        TileRotation addAlternative = new TileRotation();
        WFC_2D_Scriptable tile;

        // Creates the storage of options that every tile should have by default.
        for(int i = 0; ResourceSystem.Instance.GetWaveFunctionsPartCount() > i; i++) 
        {
            tile = ResourceSystem.Instance.GetWaveFunctionPart((WFCType)i);
            //Debug.Log("tile index: "+ i +" tile achieved rotations: " + tile.rotation90.Length);
            foreach(WFC2Drotations var in tile.rotation90)
            {
                addAlternative.rotation = var.rotation;
                addAlternative.tileType = (WFCType)i;
                
                //print(addAlternative.tileType + " WHAT " + addAlternative.rotation);

                defaultTileData.allAlternatives.Add(addAlternative);
            }
        }

        foreach(var var in defaultTileData.allAlternatives)
        {
            //print("defaultTileData: " + var.tileType);
        }

        // Iterates the grid and assigns the created data out onto every tile.
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                levelGrid[i, j] = defaultTileData;
                //print(j);
            }
        }

    }

    void PlacePins()
    {

        int tilePlacementX = 0;
        int tilePlacementZ = 0;
        for (int i = 0; i < rows; i++)
        {

            for (int j = 0; j < columns; j++)
            {
                Instantiate(pin, new Vector3(tilePlacementX, 0, tilePlacementZ), Quaternion.identity, this.transform);
                tilePlacementX += 10;
            }
            tilePlacementX = 0;
            tilePlacementZ += 10;
        }

    }
}

*/
/*
public struct TileData
{
    public List<TileRotation> allAlternatives { get; set; }
}

public struct TileRotation 
{
    public int rotation { get; set; }
    public WFCType tileType { get; set; }
}

*/