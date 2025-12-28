using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// This is what the Grid will consist of. Acts as the adress holder, for the data container. The object itself is currently tied to a TextMeshPro object.
public class WFCSlot : MonoBehaviour
{
    //neighbours
    public WFCModuleType topN, rightN, bottomN, leftN, above3DN, below3DN;
    public WFCModuleType myModule;
    public GameObject myPrefab;

    private TextMeshPro myText;

    public HashSet<WFCModuleType> myModuleAlternatives;

    public int myRow;
    public int myCol;
    public int myHeight3D = 0;

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
            rightN = WFCAlgorithm.Instance.levelGrid[myRow + 1, myCol, myHeight3D].myModule;

        if (0 <= myRow - 1)
            leftN = WFCAlgorithm.Instance.levelGrid[myRow - 1, myCol, myHeight3D].myModule;

        if (0 <= myCol - 1)
            bottomN = WFCAlgorithm.Instance.levelGrid[myRow, myCol - 1, myHeight3D].myModule;


        if (WFCAlgorithm.Instance.rowMax > myCol + 1)
            topN = WFCAlgorithm.Instance.levelGrid[myRow, myCol + 1, myHeight3D].myModule;

        if (WFCAlgorithm.Instance.height3DMax > 1)
        {
            if (WFCAlgorithm.Instance.height3DMax > myHeight3D + 1)
                rightN = WFCAlgorithm.Instance.levelGrid[myRow, myCol, myHeight3D + 1].myModule;

            if (0 <= myHeight3D - 1)
                leftN = WFCAlgorithm.Instance.levelGrid[myRow, myCol, myHeight3D - 1].myModule;
        }


        CalculateLocalEntropy();
    }

    private bool CheckIfFlipped(string neighbourSocket)
    {
        // this is to first find any flipped.
        foreach (WFCModuleType module in myModuleAlternatives)
        {
            if (neighbourSocket.Equals(module.posZ + "f"))
            {
                myModuleAlternatives.RemoveWhere(r => !r.Equals(module.posZ));
                return true;
            }

            if (module.Equals(neighbourSocket + "f"))
            {
                myModuleAlternatives.RemoveWhere(r => !r.Equals(neighbourSocket + "f"));
                return true;
            }
        }
        return false;
    }

    private void CalculateLocalEntropy()
    {
        //Debug.Log("MyEntropy before filter: " + GetEntropy());
        // In which case, it's already been collapsed.
        if (myPrefab != null)
            return;

        int entropyCount = myModuleAlternatives.Count;

        if (topN != null)
            if (!CheckIfFlipped(topN.negZ))
                myModuleAlternatives.RemoveWhere(r => r.posZ.Equals(topN.negZ));
        /* else if (WFCAlgorithm.Instance.columnMax > myCol +1 && WFCAlgorithm.Instance.levelGrid[myRow, myCol + 1].myModuleAlternatives.FirstOrDefault(r => r.negZ.Equals("-1")) == null)
            myModuleAlternatives.RemoveWhere(r => r.posZ.Equals("-1"));
        else if (WFCAlgorithm.Instance.columnMax > myCol + 1 && WFCAlgorithm.Instance.levelGrid[myRow, myCol + 1].myModuleAlternatives.FirstOrDefault(r => r.negZ.Equals("1")) == null)
            myModuleAlternatives.RemoveWhere(r => r.posZ.Equals("-1")); */

        // This "else if" above checks if it can find any alternative for the topN to have -1. If it can, remove


        if (bottomN != null)
            if (!CheckIfFlipped(bottomN.posZ))
                myModuleAlternatives.RemoveWhere(r => !r.negZ.Equals(bottomN.posZ));


        if (leftN != null)
            if (!CheckIfFlipped(leftN.posX))
                myModuleAlternatives.RemoveWhere(r => !r.negX.Equals(leftN.posX));

        if (rightN != null)
            if (!CheckIfFlipped(rightN.negX))
                myModuleAlternatives.RemoveWhere(r => !r.posX.Equals(rightN.negX));

        if (WFCAlgorithm.Instance.height3DMax > 1)
        {
            if (above3DN != null)
                if (!CheckIfFlipped(above3DN.negY))
                    myModuleAlternatives.RemoveWhere(r => !r.posY.Equals(above3DN.negY));

            if (below3DN != null)
                if (!CheckIfFlipped(below3DN.posY))
                    myModuleAlternatives.RemoveWhere(r => !r.negY.Equals(below3DN.posY));
        }

        if (myModuleAlternatives.Count == 0)
        {
            Debug.Log("No alternatives LEFT ERROR:" + myRow + ", " + myCol);
            WFCAlgorithm.Instance.CollapseTile(myRow, myCol, 0, WFCAlgorithm.Instance.blank);
        }


        if (entropyCount != myModuleAlternatives.Count)
        {
            //Debug.Log("Entropy changed from " +  entropyCount + " to " + myModuleAlternatives.Count);
            UpdateTextDisplay();

            // only those with a collapsed neigbour are allowed to update the entropy of all their neigbours.
            if (topN != null || rightN != null || bottomN != null || leftN != null || above3DN != null || below3DN != null)
                WFCAlgorithm.Instance.UpdateSurroundingNeighbours(myRow, myCol, myHeight3D); ;
        }

    }

    public void UpdateTextDisplay()
    {
        myText.text = myRow + ", " + myCol + ", " + myHeight3D + "\nE: " + GetEntropy();
    }
}