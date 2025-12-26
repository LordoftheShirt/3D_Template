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
    public WFCSlot[,] levelGrid;
    [SerializeField] public int rowMax = 9, columnMax = 9;
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
         * Should be able to modify percentage weights somehow.
         * I want the second away neighbour to be able to react. Currently untestable.
         * The algorithm should be resettable.
         * 
         * Objects should be made both addable and removable via player click?
         * Expand the algorithm into 3D to allow the possibility of 3D.
         * 
         * Mutli-use sockets? How does one do that? They're currently binary.
         * 
         * Eliminate last index bias.
         * Optimise?
         */
    }

    void Start()
    {
        meshSize = ResourceSystem.Instance.GetWFCModule(WFCModuleEnum.Straight).prefab.GetComponent<MeshRenderer>().bounds.size;
        //print("meshSize: " + meshSize);

        levelGrid = new WFCSlot[rowMax, columnMax];

        for (int i = 0; rowMax > i; i++)
        {
            for (int j = 0; columnMax > j; j++)
            {
                levelGrid[i, j] = Instantiate(TileTextDisplay, new Vector3(meshSize.x * i, 5, meshSize.z * j), Quaternion.Euler(new Vector3 (90, 0, 0)), this.transform).GetComponent<WFCSlot>();
                levelGrid[i, j].myRow = i;
                levelGrid[i, j].myCol = j;
                // Remember: x = row, z = column.
                levelGrid[i, j].UpdateTextDisplay();

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
                Destroy(levelGrid[i, j].myPrefab);
                levelGrid[i, j].myModule = null;
                levelGrid[i, j].myModuleAlternatives.UnionWith(allModuleAlternatives);
                levelGrid[i, j].UpdateTextDisplay();
                algorithmComplete = false;
                firstTilePlaced = false;
                  
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


        int entropy, row, column;

            entropy = -1;
            row = -1;
            column = -1;


            for (int i = 0; i < rowMax; i++)
            {
                for (int j = 0; j < columnMax; j++)
                {
                    // first assignment
                    if (entropy == -1 && levelGrid[i, j].GetEntropy() != 0)
                    {
                        row = i;
                        column = j;
                        entropy = levelGrid[i, j].GetEntropy();
                        //Debug.Log("First assignment: " + i + ", " + j + "Current entropy: " + entropy);
                        continue;
                    }

                    // Ensures  the first assigment has already occurred. Then finds others with lesser entropy. If two have equal entropy, do a coin flip who gets assignment.
                    if ((row != -1 && column != -1) && levelGrid[i, j].GetEntropy() != 0)
                    {
                        if (entropy > levelGrid[i, j].GetEntropy())
                        {
                            row = i;
                            column = j;
                            entropy = levelGrid[i, j].GetEntropy();
                            //Debug.Log("Found lesser entropy: " + entropy);
                        }
                        /* This randomizer is biased toawrds all later indexes. The more I think about the natural build order bias, 
                         * the less I think it is much of an issue. Feels like wasted computing to try and randomly pick between identical entropies.
                        else if (entropy == levelGrid[i, j].GetEntropy() && Random.Range(0, 2) == 1)
                        {
                            Debug.Log("Coin flip!");
                            row = i;
                            column = j;
                            entropy = levelGrid[i, j].GetEntropy();
                        } */
                    } 
                }
            }

            // if no first assigment ever occured. Else collapse the found lowest Entropy tile. Currently it certain bias to numbers the closer they are to the bottom left of the grid. Can be offset with an which ran
            if (row == -1 && column == -1  && entropy == -1)
            {
                algorithmComplete = true;
                Debug.Log("Algorithm complete!");
            }
            else
            {
                CollapseTile(row, column);
            }
        
    }

    // hardcodes the tiles' weights.
    private WFCModuleType DetermineTileWeights(int row, int col)
    {
        
        WFCModuleType chosen = null;

        /*
        int randomNumber = 0;
        while (chosen == null)
        {
            randomNumber = Random.Range(1, 101);

            if(randomNumber > 50)
            {
                chosen = blank;
            }



            if (!levelGrid[row, col].myModuleAlternatives.Contains(chosen))
                chosen = null;
        }
        */
        return chosen;
        
    }

    public void RandomizeFirstTile()
    {
        CollapseTile(Random.Range(0, rowMax), Random.Range(0, columnMax));
    }

    // type should typically be null. The type parameter exists only in case one wants to force it. For instance, when placing the first Tile.
    public void CollapseTile(int row, int col, WFCModuleType type = null)
    {
        if (type == null)
        {
            int randomIndex = Random.Range(0, levelGrid[row, col].myModuleAlternatives.Count);
            
            // Random index.
            type = levelGrid[row, col].myModuleAlternatives.ElementAt(randomIndex);

            //type = DetermineTileWeights(row, col);
        }

        levelGrid[row, col].myModule = type;
        levelGrid[row, col].myPrefab = Instantiate(ResourceSystem.Instance.GetWFCModule(type.myEnum).prefab, new Vector3(meshSize.x * row, 1, meshSize.z * col), Quaternion.Euler(new Vector3(0, 90 * type.rotation, 0)), this.transform);
        levelGrid[row, col].myModuleAlternatives.Clear();
        levelGrid[row, col].UpdateTextDisplay();

        UpdateSurroundingNeighbours(row, col);
        // Ensures 

        // Add a way of collapsing tiles already... Then updating the
        // Then WFCSlot.UpdateLocalNeighbours in all my neigbours.
        // Then WFCSlot.CalculateEntropy in all my neighbours.

    }

    public void UpdateSurroundingNeighbours(int row, int col)
    {
        if (this.rowMax > row + 1)
        {
            //Debug.Log("rightN N-if accessed");
            levelGrid[row + 1, col].UpdateMyNeighbourCanisters();
        }

        if (0 <= row - 1)
            levelGrid[row - 1, col].UpdateMyNeighbourCanisters();

        if (0 <= col - 1)
        {
            //Debug.Log("BottomN N-if accessed");
            levelGrid[row, col - 1].UpdateMyNeighbourCanisters();
        }

        if (this.columnMax > col + 1)
        {
            //Debug.Log("UPN above N-if accessed");
            levelGrid[row, col + 1].UpdateMyNeighbourCanisters();
        }
    }


    // creation of all module Data. All gets assembled into a Hashset up at Awake.
    public WFCModuleType blank = new WFCModuleType("-1", "-1", "-1", "-1", WFCModuleEnum.Blank, 0);
    public WFCModuleType cross = new WFCModuleType("1", "1", "1", "1", WFCModuleEnum.Cross, 0);

    public WFCModuleType straight_0 = new WFCModuleType("-1", "1", "-1", "1", WFCModuleEnum.Straight, 0);
    public WFCModuleType straight_1 = new WFCModuleType("1", "-1", "1", "-1", WFCModuleEnum.Straight, 1);

    public WFCModuleType turn90_0 = new WFCModuleType("1", "-1", "-1", "1", WFCModuleEnum.Turn90, 0);
    public WFCModuleType turn90_1 = new WFCModuleType("1", "1", "-1", "-1", WFCModuleEnum.Turn90, 1);
    public WFCModuleType turn90_2 = new WFCModuleType("-1", "1", "1", "-1", WFCModuleEnum.Turn90, 2);
    public WFCModuleType turn90_3 = new WFCModuleType("-1", "-1", "1", "1", WFCModuleEnum.Turn90, 3);

    public WFCModuleType pathT_0 = new WFCModuleType("-1", "1", "1", "1", WFCModuleEnum.PathT, 0);
    public WFCModuleType pathT_1 = new WFCModuleType("1", "-1", "1", "1", WFCModuleEnum.PathT, 1);
    public WFCModuleType pathT_2 = new WFCModuleType("1", "1", "-1", "1", WFCModuleEnum.PathT, 2);
    public WFCModuleType pathT_3 = new WFCModuleType("1", "1", "1", "-1", WFCModuleEnum.PathT, 3);

    public WFCModuleType deadEnd_0 = new WFCModuleType("-1", "-1", "-1", "1", WFCModuleEnum.DeadEnd, 0);
    public WFCModuleType deadEnd_1 = new WFCModuleType("1", "-1", "-1", "-1", WFCModuleEnum.DeadEnd, 1);
    public WFCModuleType deadEnd_2 = new WFCModuleType("-1", "1", "-1", "-1", WFCModuleEnum.DeadEnd, 2);
    public WFCModuleType deadEnd_3 = new WFCModuleType("-1", "-1", "1", "-1", WFCModuleEnum.DeadEnd, 3);
}
    

    // This is the data container. That will fill each grid slot.
    public class WFCModuleType
    {
        // Sockets. Ordered in the clock's direction.
        public string posZ;
        public string posX;
        public string negZ;
        public string negX;

        public WFCModuleEnum myEnum;
        public int rotation;

        public WFCModuleType(string posZ, string posX, string negZ, string negX, WFCModuleEnum myType, int rotation)
        {
            this.posZ = posZ;
            this.posX = posX;
            this.negZ = negZ;
            this.negX = negX;
            this.myEnum = myType;
            this.rotation = rotation;
        }
    }

public enum WFCModuleEnum
{
    Blank = 0,
    Straight = 1,
    Turn90 = 2,

    Cross = 3,
    PathT = 4,
    DeadEnd = 5,
}
