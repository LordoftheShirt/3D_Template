using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// This is what the Grid will consist of. Acts as the adress holder, for the data container. The object itself is currently tied to a TextMeshPro object.
public class WFCSlot : MonoBehaviour
{
    //neighbours
    public WFCModuleType topN, rightN, bottomN, leftN;
    public WFCModuleType myModule;
    public GameObject myPrefab;

    private TextMeshPro myText;

    public HashSet<WFCModuleType> myModuleAlternatives;

    public int myRow;
    public int myCol;

    private void Awake()
    {
        myText = GetComponent<TextMeshPro>();
        myText.color = Color.red;

        myModuleAlternatives = new HashSet<WFCModuleType>(WFCAlgorithm.Instance.allModuleAlternatives);
    }


    // temporary
    public int GetEntropy()
    {
        return myModuleAlternatives.Count;
    }


    public void UpdateMyNeighbourCanisters()
    {
        //
        if (WFCAlgorithm.Instance.rowMax > myRow + 1)
            rightN = WFCAlgorithm.Instance.levelGrid[myRow + 1, myCol].myModule;

        if (0 <= myRow - 1)
            leftN = WFCAlgorithm.Instance.levelGrid[myRow - 1, myCol].myModule;

        if (0 <= myCol - 1)
        {
            bottomN = WFCAlgorithm.Instance.levelGrid[myRow, myCol - 1].myModule;
            //Debug.Log("UpdateNeighbour BottomN if assigned: " + bottomN);
        }

        if (WFCAlgorithm.Instance.rowMax > myCol + 1)
            topN = WFCAlgorithm.Instance.levelGrid[myRow, myCol + 1].myModule;


        CalculateLocalEntropy();
    }

    private void CalculateLocalEntropy()
    {
        //Debug.Log("MyEntropy before filter: " + GetEntropy());
        // In which case, it's already been collapsed.
        if (myPrefab != null)
        return;

        int entropyCount = myModuleAlternatives.Count;

        if (topN != null)
            myModuleAlternatives.RemoveWhere(r => r.posZ != topN.negZ);
        /* else if (WFCAlgorithm.Instance.columnMax > myCol +1 && WFCAlgorithm.Instance.levelGrid[myRow, myCol + 1].myModuleAlternatives.FirstOrDefault(r => r.negZ.Equals("-1")) == null)
            myModuleAlternatives.RemoveWhere(r => r.posZ.Equals("-1"));
        else if (WFCAlgorithm.Instance.columnMax > myCol + 1 && WFCAlgorithm.Instance.levelGrid[myRow, myCol + 1].myModuleAlternatives.FirstOrDefault(r => r.negZ.Equals("1")) == null)
            myModuleAlternatives.RemoveWhere(r => r.posZ.Equals("-1")); */

        // This "else if" above checks if it can find any alternative for the topN to have -1. If it can, remove


        if (bottomN != null)
        {
            myModuleAlternatives.RemoveWhere(r => !r.negZ.Equals(bottomN.posZ));
            //Debug.Log("Entropy lowered!" + GetEntropy());
        }

        
        if (leftN != null)   
            myModuleAlternatives.RemoveWhere(r => r.negX != leftN.posX);
        
        if (rightN != null)       
            myModuleAlternatives.RemoveWhere(r => r.posX != rightN.negX);

        if (myModuleAlternatives.Count == 0)
        {
            //Debug.Log("No alternatives LEFT ERROR:" + myRow + ", " + myCol);
            WFCAlgorithm.Instance.CollapseTile(myRow, myCol, WFCAlgorithm.Instance.blank);

        }
        
        if (entropyCount != myModuleAlternatives.Count)
        {
            //Debug.Log("Entropy changed from " +  entropyCount + " to " + myModuleAlternatives.Count);
            UpdateTextDisplay();

            // only those with a collapsed neigbour are allowed to update the entropy of all their neigbours.
            if (topN != null || rightN != null || bottomN != null || leftN != null)
                WFCAlgorithm.Instance.UpdateSurroundingNeighbours(myRow, myCol);;
        }
        
    }

    public void UpdateTextDisplay()
    {
        myText.text = myRow + ", " + myCol + "\nE: " + GetEntropy();
    }
}