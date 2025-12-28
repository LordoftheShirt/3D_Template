using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;

// This is where all the loops are happening.
public class WFCAlgorithm : Singleton<WFCAlgorithm>
{
    public WFCSlot[,,] levelGrid;
    [SerializeField] public int rowMax = 9, columnMax = 9, height3DMax = 1;
    [SerializeField] public GameObject TileTextDisplay;

    public HashSet<WFCModuleType> allModuleAlternatives = new HashSet<WFCModuleType>();
    private Vector3 meshSize;

    private bool algorithmComplete = false, firstTilePlaced = false;
    protected override void Awake()
    {
        //singleton
        base.Awake();

        allModuleAlternatives.Add(blank);
        allModuleAlternatives.Add(cross);

        allModuleAlternatives.Add(straight_0);

        allModuleAlternatives.Add(straight_1);


        allModuleAlternatives.Add(turn90_0);
        allModuleAlternatives.Add(turn90_1);
        allModuleAlternatives.Add(turn90_2);
        allModuleAlternatives.Add(turn90_3);

        allModuleAlternatives.Add(pathT_0);
        allModuleAlternatives.Add(pathT_1);
        allModuleAlternatives.Add(pathT_2);
        allModuleAlternatives.Add(pathT_3);

        /*
        allModuleAlternatives.Add(deadEnd_0);
        allModuleAlternatives.Add(deadEnd_1);
        allModuleAlternatives.Add(deadEnd_2);
        allModuleAlternatives.Add(deadEnd_3); */

        // TODO:
        /*
         * I want the second away neighbour to be able to react. DONE?! (Currently untestable.)
         * 
         * Objects should be made both addable and removable via player click?
         * 
         * THESE TWO GO HAND IN HAND:
         * Expand the algorithm into 3D to allow the possibility of 3D.
         * Mutli-use sockets? How does one do that? They're currently binary.
         * 
         * Optimise?
         */
    }

    void Start()
    {
        meshSize = ResourceSystem.Instance.GetWFCModule(WFCModuleEnum2D.Straight).prefab.GetComponent<MeshRenderer>().bounds.size;
        //print("meshSize: " + meshSize);

        levelGrid = new WFCSlot[rowMax, columnMax, height3DMax];

        for (int i = 0; rowMax > i; i++)
        {
            for (int j = 0; columnMax > j; j++)
            {
                for (int y = 0; height3DMax > y; y++)
                {
                    levelGrid[i, j, y] = Instantiate(TileTextDisplay, new Vector3(meshSize.x * i, 5, meshSize.z * j), Quaternion.Euler(new Vector3(90, 0, 0)), this.transform).GetComponent<WFCSlot>();
                    levelGrid[i, j, y].myRow = i;
                    levelGrid[i, j, y].myCol = j;
                    // Remember: x = row, z = column.
                    levelGrid[i, j, y].UpdateTextDisplay();
                }
            }
        }

        /*
        CollapseTile(5, 5, turn90_1);

        CollapseTile(5, 6);
        CollapseTile(5, 4);

        CollapseTile(6, 5);
        CollapseTile(4, 5); */

        //CollapseTile(7, 7);

    }

    void Update()
    {
        /*
        if (timer > 0)
            timer = timer - Time.deltaTime;
        else if (!algorithmComplete && timer <= 0)
        {
            RunAlgorithm();
            timer = 0.05f;
        } */

        if (!algorithmComplete)
            RunAlgorithm();
        else
            ResetAlgorithm();

    }

    public void ResetAlgorithm()
    {
        for (int i = 0; rowMax > i; i++)
        {
            for (int j = 0; columnMax > j; j++)
            {
                for (int y = 0; y < height3DMax; y++)
                {
                    Destroy(levelGrid[i, j, y].myPrefab);
                    levelGrid[i, j, y].myModule = null;
                    levelGrid[i, j, y].myModuleAlternatives.UnionWith(allModuleAlternatives);
                    levelGrid[i, j, y].UpdateTextDisplay();
                    algorithmComplete = false;
                    firstTilePlaced = false;
                }
            }
        }
    }
    public void RunAlgorithm()
    {
        if (!firstTilePlaced)
        {
            RandomizeFirstTile();
            firstTilePlaced = true;
        }


        int entropy, row, column, height3D;

        entropy = -1;
        row = -1;
        column = -1;
        height3D = -1;


        for (int i = 0; i < rowMax; i++)
        {
            for (int j = 0; j < columnMax; j++)
            {
                for (int y = 0; y < height3DMax; y++)
                {
                    // first assignment
                    if (entropy == -1 && levelGrid[i, j, y].GetEntropy() != 0)
                    {
                        row = i;
                        column = j;
                        height3D = y;
                        entropy = levelGrid[i, j, y].GetEntropy();
                        //Debug.Log("First assignment: " + i + ", " + j + "Current entropy: " + entropy);
                        continue;
                    }

                    // Ensures  the first assigment has already occurred. Then finds others with lesser entropy. If two have equal entropy, do a coin flip who gets assignment.
                    if ((row != -1 && column != -1) && levelGrid[i, j, y].GetEntropy() != 0)
                    {
                        if (entropy > levelGrid[i, j, y].GetEntropy())
                        {
                            row = i;
                            column = j;
                            height3D = y;
                            entropy = levelGrid[i, j, y].GetEntropy();
                            //Debug.Log("Found lesser entropy: " + entropy);
                        }
                    }
                }
            }
        }

        // if no first assigment ever occured. Else collapse the found lowest Entropy tile. Currently it certain bias to numbers the closer they are to the bottom left of the grid. Can be offset with an which ran
        if (row == -1 || column == -1 || height3D == -1 || entropy == -1)
        {
            algorithmComplete = true;
            Debug.Log("Algorithm complete!");
        }
        else
        {
            CollapseTile(row, column, height3D);
        }

    }

    public void RandomizeFirstTile()
    {
        CollapseTile(Random.Range(0, rowMax), Random.Range(0, columnMax), Random.Range(0, height3DMax));
    }

    // type should typically be null. The type parameter exists only in case one wants to force it. For instance, when placing the first Tile.
    public void CollapseTile(int row, int col, int height3D = 0, WFCModuleType type = null)
    {
        if (type == null)
        {
            int randomIndex = -1;
            while (randomIndex == -1)
            {
                randomIndex = Random.Range(0, levelGrid[row, col, height3D].myModuleAlternatives.Count);
                type = levelGrid[row, col, height3D].myModuleAlternatives.ElementAt(randomIndex);

                // If the weight does not fulfill the size of the random value, do a do-over. Example: weight is 10% and random.value rolls 20%. Now, random.value is bigger than the weight. Try again. 
                if (ResourceSystem.Instance.GetWFCModule(type.myEnum).weightPercentage <= Random.value)
                    randomIndex = -1;
            }
            // Random index.
        }

        levelGrid[row, col, height3D].myModule = type;
        levelGrid[row, col, height3D].myPrefab = Instantiate(ResourceSystem.Instance.GetWFCModule(type.myEnum).prefab, new Vector3(meshSize.x * row, meshSize.y * height3D, meshSize.z * col), Quaternion.Euler(new Vector3(0, 90 * type.rotation, 0)), this.transform);
        levelGrid[row, col, height3D].myModuleAlternatives.Clear();
        levelGrid[row, col, height3D].UpdateTextDisplay();

        UpdateSurroundingNeighbours(row, col, height3D);
    }

    public void UpdateSurroundingNeighbours(int row, int col, int height3D = 0)
    {
        if (this.rowMax > row + 1)
            levelGrid[row + 1, col, height3D].UpdateMyNeighbourCanisters();

        if (0 <= row - 1)
            levelGrid[row - 1, col, height3D].UpdateMyNeighbourCanisters();

        if (0 <= col - 1)
            levelGrid[row, col - 1, height3D].UpdateMyNeighbourCanisters();

        if (this.columnMax > col + 1)
            levelGrid[row, col + 1, height3D].UpdateMyNeighbourCanisters();


        if (this.height3DMax > 1)
        {
            if (this.height3DMax > height3D + 1)
                levelGrid[row, col, height3D + 1].UpdateMyNeighbourCanisters();

            if (0 <= height3D - 1)
                levelGrid[row, col, height3D - 1].UpdateMyNeighbourCanisters();
        }
    }


    // creation of all module Data. All gets assembled into a Hashset up at Awake.
    public WFCModuleType blank = new WFCModuleType("-1", "-1", "-1", "-1", WFCModuleEnum2D.Blank, 0);
    public WFCModuleType cross = new WFCModuleType("1", "1", "1", "1", WFCModuleEnum2D.Cross, 0);

    public WFCModuleType straight_0 = new WFCModuleType("-1", "1", "-1", "1", WFCModuleEnum2D.Straight, 0);
    public WFCModuleType straight_1 = new WFCModuleType("1", "-1", "1", "-1", WFCModuleEnum2D.Straight, 1);

    public WFCModuleType turn90_0 = new WFCModuleType("1", "-1", "-1", "1", WFCModuleEnum2D.Turn90, 0);
    public WFCModuleType turn90_1 = new WFCModuleType("1", "1", "-1", "-1", WFCModuleEnum2D.Turn90, 1);
    public WFCModuleType turn90_2 = new WFCModuleType("-1", "1", "1", "-1", WFCModuleEnum2D.Turn90, 2);
    public WFCModuleType turn90_3 = new WFCModuleType("-1", "-1", "1", "1", WFCModuleEnum2D.Turn90, 3);

    public WFCModuleType pathT_0 = new WFCModuleType("-1", "1", "1", "1", WFCModuleEnum2D.PathT, 0);
    public WFCModuleType pathT_1 = new WFCModuleType("1", "-1", "1", "1", WFCModuleEnum2D.PathT, 1);
    public WFCModuleType pathT_2 = new WFCModuleType("1", "1", "-1", "1", WFCModuleEnum2D.PathT, 2);
    public WFCModuleType pathT_3 = new WFCModuleType("1", "1", "1", "-1", WFCModuleEnum2D.PathT, 3);

    public WFCModuleType deadEnd_0 = new WFCModuleType("-1", "-1", "-1", "1", WFCModuleEnum2D.DeadEnd, 0);
    public WFCModuleType deadEnd_1 = new WFCModuleType("1", "-1", "-1", "-1", WFCModuleEnum2D.DeadEnd, 1);
    public WFCModuleType deadEnd_2 = new WFCModuleType("-1", "1", "-1", "-1", WFCModuleEnum2D.DeadEnd, 2);
    public WFCModuleType deadEnd_3 = new WFCModuleType("-1", "-1", "1", "-1", WFCModuleEnum2D.DeadEnd, 3);
}


// This is the data container. That will fill each grid slot.
public class WFCModuleType
{
    // Sockets. Ordered in the clock's direction.
    public string posZ;
    public string posX;
    public string negZ;
    public string negX;

    // These stay null unless the object is 3D.
    public string posY;
    public string negY;

    public WFCModuleEnum2D myEnum;
    public int rotation;

    public WFCModuleType(string posZ, string posX, string negZ, string negX, WFCModuleEnum2D myEnum, int rotation, string posY = null, string negY = null)
    {
        this.posZ = posZ;
        this.posX = posX;
        this.negZ = negZ;
        this.negX = negX;
        this.myEnum = myEnum;
        this.rotation = rotation;

        this.posY = posY;
        this.negY = negY;
    }
}

public enum WFCModuleEnum2D
{
    Blank = 0,
    Straight = 1,
    Turn90 = 2,

    Cross = 3,
    PathT = 4,
    DeadEnd = 5,
}

public enum WFCModuleEnum3D
{
    coastStraight = 0,
    coast_OuterBend = 1,
    coast_InnerBend = 2,

    FlatGrass = 3,
    openSpace = 4,

    cliffEdgeStem_Straight = 5,
    CliffEdgeStem_OuterBend = 6,
    CliffEdgeStem_InnerBend = 7,

    Cliff_Straight = 8,
    Cliff_OuterBend = 9,
    Cliff_InnerBend = 10,

    CliffEdge_Straight = 11,
    CliffEdge_OuterBend = 12,
    CliffEdge_InnerBend = 13,


}
