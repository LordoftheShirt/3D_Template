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

    private float timer = 1;

    private bool algorithmComplete = false, firstTilePlaced = false;
    protected override void Awake()
    {
        //singleton
        base.Awake();

        /*
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

        // 3D

        allModuleAlternatives.Add(openSpace);
        allModuleAlternatives.Add(flatGrass);
        
        // COAST
        allModuleAlternatives.Add(coastEdgeInnerBend_0);
        allModuleAlternatives.Add(coastEdgeInnerBend_1);
        allModuleAlternatives.Add(coastEdgeInnerBend_2);
        allModuleAlternatives.Add(coastEdgeInnerBend_3);

        allModuleAlternatives.Add(coastEdgeOuterBend_0);
        allModuleAlternatives.Add(coastEdgeOuterBend_1);
        allModuleAlternatives.Add(coastEdgeOuterBend_2);
        allModuleAlternatives.Add(coastEdgeOuterBend_3);

        allModuleAlternatives.Add(coastStraight_0);
        allModuleAlternatives.Add(coastStraight_1);
        /*
        // Everything CIFF
        allModuleAlternatives.Add(cliffStemInnerBend_0);
        allModuleAlternatives.Add(cliffStemInnerBend_1);
        allModuleAlternatives.Add(cliffStemInnerBend_2);
        allModuleAlternatives.Add(cliffStemInnerBend_3);

        allModuleAlternatives.Add(cliffStemOuterBend_0);
        allModuleAlternatives.Add(cliffStemOuterBend_1);
        allModuleAlternatives.Add(cliffStemOuterBend_2);
        allModuleAlternatives.Add(cliffStemOuterBend_3);

        allModuleAlternatives.Add(cliffStemStraight_0);
        allModuleAlternatives.Add(cliffStemStraight_1);
        allModuleAlternatives.Add(cliffStemStraight_2);
        allModuleAlternatives.Add(cliffStemStraight_3);

        allModuleAlternatives.Add(cliffEdgeInnerBend_0);
        allModuleAlternatives.Add(cliffEdgeInnerBend_1);
        allModuleAlternatives.Add(cliffEdgeInnerBend_2);
        allModuleAlternatives.Add(cliffEdgeInnerBend_3);

        allModuleAlternatives.Add(cliffEdgeOuterBend_0);
        allModuleAlternatives.Add(cliffEdgeOuterBend_1);
        allModuleAlternatives.Add(cliffEdgeOuterBend_2);
        allModuleAlternatives.Add(cliffEdgeOuterBend_3);

        allModuleAlternatives.Add(cliffEdgeStraight_0);
        allModuleAlternatives.Add(cliffEdgeStraight_1);
        allModuleAlternatives.Add(cliffEdgeStraight_2);
        allModuleAlternatives.Add(cliffEdgeStraight_3);
        /*
        //include edgeToStem here.

        // These "Only Walls" can be left out upon first trial.
        /*
        allModuleAlternatives.Add(cliffInnerBend_0);
        allModuleAlternatives.Add(cliffInnerBend_1);
        allModuleAlternatives.Add(cliffInnerBend_2);
        allModuleAlternatives.Add(cliffInnerBend_3);

        allModuleAlternatives.Add(cliffOuterBend_0);
        allModuleAlternatives.Add(cliffOuterBend_1);
        allModuleAlternatives.Add(cliffOuterBend_2);
        allModuleAlternatives.Add(cliffOuterBend_3);

        allModuleAlternatives.Add(cliffStemStraight_0);
        allModuleAlternatives.Add(cliffStemStraight_1);
        allModuleAlternatives.Add(cliffStemStraight_2);
        allModuleAlternatives.Add(cliffStemStraight_3);
        */
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
        meshSize = ResourceSystem.Instance.GetWFCModule("moduleSize").prefab.GetComponent<MeshRenderer>().bounds.size;
        //print("meshSize: " + meshSize);

        levelGrid = new WFCSlot[rowMax, columnMax, height3DMax];

        for (int i = 0; rowMax > i; i++)
        {
            for (int j = 0; columnMax > j; j++)
            {
                for (int y = 0; height3DMax > y; y++)
                {
                    levelGrid[i, j, y] = Instantiate(TileTextDisplay, new Vector3(meshSize.x * i, 5 + meshSize.y * y+2, meshSize.z * j), Quaternion.Euler(new Vector3(90, 0, 0)), this.transform).GetComponent<WFCSlot>();
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
        if (!firstTilePlaced)
        {
            CollapseTile(2, 2, 0, flatGrass);
            firstTilePlaced = true;
        }

        if (timer > 0)
            timer = timer - Time.deltaTime;
        else if (timer <= 0)
        {
            if (!algorithmComplete)
                RunAlgorithm();
            else
                ResetAlgorithm();
            timer = 1f;
        } 

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
        CollapseTile(Random.Range(0, rowMax), Random.Range(0, columnMax));
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
                // Also, if on the bottom layer, checks that the downwards socket is -1.
                if (ResourceSystem.Instance.GetWFCModule(type.myType).weightPercentage <= Random.value || (height3D == 0 && !type.negY.Contains("-1")))
                    randomIndex = -1;
            }
            // Random index.
        }

        levelGrid[row, col, height3D].myModule = type;
        levelGrid[row, col, height3D].myPrefab = Instantiate(ResourceSystem.Instance.GetWFCModule(type.myType).prefab, new Vector3(meshSize.x * row, meshSize.y * height3D, meshSize.z * col), Quaternion.Euler(new Vector3(0, 90 * type.rotation, 0)), this.transform);
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


    // creation of all 2D-module Data. All gets assembled into a Hashset up at Awake.
    public WFCModuleType blank = new WFCModuleType("-1", "-1", "-1", "-1", "blank", 0);
    public WFCModuleType cross = new WFCModuleType("1", "1", "1", "1", "cross", 0);

    public WFCModuleType straight_0 = new WFCModuleType("-1", "1", "-1", "1", "straight", 0);
    public WFCModuleType straight_1 = new WFCModuleType("1", "-1", "1", "-1", "straight", 1);

    public WFCModuleType turn90_0 = new WFCModuleType("1", "-1", "-1", "1", "turn90", 0);
    public WFCModuleType turn90_1 = new WFCModuleType("1", "1", "-1", "-1", "turn90", 1);
    public WFCModuleType turn90_2 = new WFCModuleType("-1", "1", "1", "-1", "turn90", 2);
    public WFCModuleType turn90_3 = new WFCModuleType("-1", "-1", "1", "1", "turn90", 3);

    public WFCModuleType pathT_0 = new WFCModuleType("-1", "1", "1", "1", "pathT", 0);
    public WFCModuleType pathT_1 = new WFCModuleType("1", "-1", "1", "1", "pathT", 1);
    public WFCModuleType pathT_2 = new WFCModuleType("1", "1", "-1", "1", "pathT", 2);
    public WFCModuleType pathT_3 = new WFCModuleType("1", "1", "1", "-1", "pathT", 3);

    public WFCModuleType deadEnd_0 = new WFCModuleType("-1", "-1", "-1", "1", "deadEnd", 0);
    public WFCModuleType deadEnd_1 = new WFCModuleType("1", "-1", "-1", "-1", "deadEnd", 1);
    public WFCModuleType deadEnd_2 = new WFCModuleType("-1", "1", "-1", "-1", "deadEnd", 2);
    public WFCModuleType deadEnd_3 = new WFCModuleType("-1", "-1", "1", "-1", "deadEnd", 3);

    // Creation of All 3D-module Data.

    // BASICS
    public WFCModuleType openSpace = new WFCModuleType("-1f", "-1f", "-1f", "-1f", "openSpace", 0, "-1f", "-1f");
    public WFCModuleType flatGrass = new WFCModuleType("1s", "1s", "1s", "1s", "flatGrass", 0, "-1", "-1");

    // COAST
    public WFCModuleType coastStraight_0 = new WFCModuleType("0", "1s", "0f", "-1", "coastStraight", 0, "-1", "-1");
    public WFCModuleType coastStraight_1 = new WFCModuleType("-1", "0", "1s", "0f", "coastStraight", 1, "-1", "-1");
    public WFCModuleType coastStraight_2 = new WFCModuleType("0f", "-1", "0", "1s", "coastStraight", 2, "-1", "-1");
    public WFCModuleType coastStraight_3 = new WFCModuleType("1s", "0f", "-1", "0", "coastStraight", 3, "-1", "-1");

    public WFCModuleType coastEdgeInnerBend_0 = new WFCModuleType("1s", "1s", "0f", "0", "coastInnerBend", 0, "-1", "-1");
    public WFCModuleType coastEdgeInnerBend_1 = new WFCModuleType("0", "1s", "1s", "0f", "coastInnerBend", 1, "-1", "-1");
    public WFCModuleType coastEdgeInnerBend_2 = new WFCModuleType("0f", "0", "1s", "1s", "coastInnerBend", 2, "-1", "-1");
    public WFCModuleType coastEdgeInnerBend_3 = new WFCModuleType("1s", "0f", "0", "1s", "coastInnerBend", 3, "-1", "-1");

    public WFCModuleType coastEdgeOuterBend_0 = new WFCModuleType("0", "0f", "-1", "-1", "coastOuterBend", 0, "-1", "-1");
    public WFCModuleType coastEdgeOuterBend_1 = new WFCModuleType("-1", "0", "0f", "-1", "coastOuterBend", 1, "-1", "-1");
    public WFCModuleType coastEdgeOuterBend_2 = new WFCModuleType("-1", "-1", "0", "0f", "coastOuterBend", 2, "-1", "-1");
    public WFCModuleType coastEdgeOuterBend_3 = new WFCModuleType("0f", "-1", "-1", "0", "coastOuterBend", 3, "-1", "-1");

    // CLIFF STEM
    public WFCModuleType cliffStemStraight_0 = new WFCModuleType("4", "-1", "4f", "-1", "cliffStemStraight", 0, "v0_0", "-1");
    public WFCModuleType cliffStemStraight_1 = new WFCModuleType("-1", "4", "-1", "4f", "cliffStemStraight", 1, "v0_1", "-1");
    public WFCModuleType cliffStemStraight_2 = new WFCModuleType("4f", "-1", "4", "-1", "cliffStemStraight", 2, "v0_2", "-1");
    public WFCModuleType cliffStemStraight_3 = new WFCModuleType("-1", "4f", "-1", "4", "cliffStemStraight", 3, "v0_3", "-1");

    public WFCModuleType cliffStemInnerBend_0 = new WFCModuleType("-1", "-1", "4f", "4", "cliffStemInnerBend", 0, "v1_0", "-1");
    public WFCModuleType cliffStemInnerBend_1 = new WFCModuleType("4", "-1", "-1", "4f", "cliffStemInnerBend", 1, "v1_1", "-1");
    public WFCModuleType cliffStemInnerBend_2 = new WFCModuleType("4f", "4", "-1", "-1", "cliffStemInnerBend", 2, "v1_2", "-1");
    public WFCModuleType cliffStemInnerBend_3 = new WFCModuleType("-1", "4f", "4", "-1", "cliffStemInnerBend", 3, "v1_3", "-1");

    public WFCModuleType cliffStemOuterBend_0 = new WFCModuleType("4", "4f", "1s", "1s", "cliffStemOuterBend", 0, "v2_0", "-1");
    public WFCModuleType cliffStemOuterBend_1 = new WFCModuleType("1s", "4", "4f", "1s", "cliffStemOuterBend", 1, "v2_1", "-1");
    public WFCModuleType cliffStemOuterBend_2 = new WFCModuleType("1s", "1s", "4", "4f", "cliffStemOuterBend", 2, "v2_2", "-1");
    public WFCModuleType cliffStemOuterBend_3 = new WFCModuleType("4f", "1s", "1s", "4", "cliffStemOuterBend", 3, "v2_3", "-1");


    // CLIFF
    public WFCModuleType cliffStraight_0 = new WFCModuleType("2s", "-1", "2s", "-1", "cliffStraight", 0, "v0_0", "v0_0");
    public WFCModuleType cliffStraight_1 = new WFCModuleType("-1", "2s", "-1", "2s", "cliffStraight", 1, "v0_1", "v0_1");
    public WFCModuleType cliffStraight_2 = new WFCModuleType("2s", "-1", "2s", "-1", "cliffStraight", 2, "v0_2", "v0_2");
    public WFCModuleType cliffStraight_3 = new WFCModuleType("-1", "2s", "-1", "2s", "cliffStraight", 3, "v0_3", "v0_3");

    public WFCModuleType cliffInnerBend_0 = new WFCModuleType("-1", "-1", "2s", "2s", "cliffInnerBend", 0, "v1_0", "v1_0");
    public WFCModuleType cliffInnerBend_1 = new WFCModuleType("2s", "-1", "-1", "2s", "cliffInnerBend", 1, "v1_1", "v1_1");
    public WFCModuleType cliffInnerBend_2 = new WFCModuleType("2s", "2s", "-1", "-1", "cliffInnerBend", 2, "v1_2", "v1_2");
    public WFCModuleType cliffInnerBend_3 = new WFCModuleType("-1", "2s", "2s", "-1", "cliffInnerBend", 3, "v1_3", "v1_3");

    public WFCModuleType cliffOuterBend_0 = new WFCModuleType("-1", "2s", "2s", "-1", "cliffOuterBend", 0, "v2_0", "v2_0");
    public WFCModuleType cliffOuterBend_1 = new WFCModuleType("-1", "-1", "2s", "2s", "cliffOuterBend", 1, "v2_1", "v2_1");
    public WFCModuleType cliffOuterBend_2 = new WFCModuleType("2s", "-1", "-1", "2s", "cliffOuterBend", 2, "v2_2", "v2_2");
    public WFCModuleType cliffOuterBend_3 = new WFCModuleType("2s", "2s", "-1", "-1", "cliffOuterBend", 3, "v2_3", "v2_3");

    // CLIFF EDGE
    public WFCModuleType cliffEdgeStraight_0 = new WFCModuleType("3", "1s", "3f", "-1", "cliffEdgeStraight", 0, "-1", "v0_0");
    public WFCModuleType cliffEdgeStraight_1 = new WFCModuleType("-1", "3", "1s", "3f", "cliffEdgeStraight", 1, "-1", "v0_1");
    public WFCModuleType cliffEdgeStraight_2 = new WFCModuleType("3f", "-1", "3", "1s", "cliffEdgeStraight", 2, "-1", "v0_2");
    public WFCModuleType cliffEdgeStraight_3 = new WFCModuleType("1s", "3f", "-1", "3", "cliffEdgeStraight", 3, "-1", "v0_3");

    public WFCModuleType cliffEdgeInnerBend_0 = new WFCModuleType("1s", "1s", "3f", "3", "cliffEdgeInnerBend", 0, "-1", "v1_0");
    public WFCModuleType cliffEdgeInnerBend_1 = new WFCModuleType("3", "1s", "1s", "3f", "cliffEdgeInnerBend", 1, "-1", "v1_1");
    public WFCModuleType cliffEdgeInnerBend_2 = new WFCModuleType("3f", "3", "1s", "1s", "cliffEdgeInnerBend", 2, "-1", "v1_2");
    public WFCModuleType cliffEdgeInnerBend_3 = new WFCModuleType("1s", "3f", "3", "1s", "cliffEdgeInnerBend", 3, "-1", "v1_3");

    public WFCModuleType cliffEdgeOuterBend_0 = new WFCModuleType("3", "3f", "-1", "-1", "cliffEdgeOuterBend", 0, "-1", "v2_0");
    public WFCModuleType cliffEdgeOuterBend_1 = new WFCModuleType("-1", "3", "3f", "-1", "cliffEdgeOuterBend", 1, "-1", "v2_1");
    public WFCModuleType cliffEdgeOuterBend_2 = new WFCModuleType("-1", "-1", "3", "3f", "cliffEdgeOuterBend", 2, "-1", "v2_2");
    public WFCModuleType cliffEdgeOuterBend_3 = new WFCModuleType("3f", "-1", "-1", "3", "cliffEdgeOuterBend", 3, "-1", "v2_3");

    // Must make this later.
    public WFCModuleType cliffEdgeToStem = new WFCModuleType("f", "f", "f", "f", "cliffEdgeToStem", 0, "f", "´f");



    /* 3D island SOCKETS.
     * 
     * -1 is null space.
     * 0 is COAST bend: -\         while: /- is 0f
     * 1s is straight horizontal line: ---
     * 2s is straight vertical line: |
     * 
     * 3 is CLIFFEDGE_SIDE: -\    while /- is 3f
     * 
     * 4 is CLIFFSTEM_SIDE: |\_     while: _/| is 4f 
     * 
     * v0 is straight vertical.
     * v1 is inner bend vertical.
     * v2 is outer bend vertical. 
     */
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

    public string myType;
    public int rotation;

    public WFCModuleType(string posZ, string posX, string negZ, string negX, string type, int rotation, string posY = "", string negY = "")
    {
        this.posZ = posZ;
        this.posX = posX;
        this.negZ = negZ;
        this.negX = negX;
        this.myType = type;
        this.rotation = rotation;

        this.posY = posY;
        this.negY = negY;
    }
}
