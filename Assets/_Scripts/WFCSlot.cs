using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
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
        // Checks for neighbours. If that direction is out of bounds, Ensure it gets interpreted as a -1 socket.
        if (WFCAlgorithm.Instance.rowMax > myRow + 1)
            rightN = WFCAlgorithm.Instance.levelGrid[myRow + 1, myCol, myHeight3D].myModule;
        else
            myModuleAlternatives.RemoveWhere(r => !r.posX.Contains("-1"));

        if (0 <= myRow - 1)
            leftN = WFCAlgorithm.Instance.levelGrid[myRow - 1, myCol, myHeight3D].myModule;
        else
            myModuleAlternatives.RemoveWhere(r => !r.negX.Contains("-1"));

        if (0 <= myCol - 1)
            bottomN = WFCAlgorithm.Instance.levelGrid[myRow, myCol - 1, myHeight3D].myModule;
        else
            myModuleAlternatives.RemoveWhere(r => !r.negZ.Contains("-1"));

        if (WFCAlgorithm.Instance.rowMax > myCol + 1)
            topN = WFCAlgorithm.Instance.levelGrid[myRow, myCol + 1, myHeight3D].myModule;
        else
            myModuleAlternatives.RemoveWhere(r => !r.posZ.Contains("-1"));


        if (WFCAlgorithm.Instance.height3DMax > 1)
        {
            if (WFCAlgorithm.Instance.height3DMax > myHeight3D + 1)
                rightN = WFCAlgorithm.Instance.levelGrid[myRow, myCol, myHeight3D + 1].myModule;

            if (0 <= myHeight3D - 1)
                leftN = WFCAlgorithm.Instance.levelGrid[myRow, myCol, myHeight3D - 1].myModule;
        }


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
        {
            // first if ensures the neighbour isn't opewnSpace. The next if in case it's a symmetrical socket. The next if in case it's a flipped socket.
            if (topN.negZ.Contains("-1"))
                myModuleAlternatives.RemoveWhere(r => !r.posZ.Equals("-1f"));
            else if (topN.negZ.Contains('s'))
                myModuleAlternatives.RemoveWhere(r => !r.posZ.Equals(topN.negZ));
            else if (topN.negZ.Contains('f'))
                myModuleAlternatives.RemoveWhere(r => !r.posZ.Equals(topN.negZ.Trim('f')));

        }

        //myModuleAlternatives.RemoveWhere(r => r.posZ.Equals(topN.negZ));


        if (bottomN != null)
        {
            if (bottomN.posZ.Contains("-1"))
                myModuleAlternatives.RemoveWhere(r => !r.negZ.Equals("-1f"));
            else if (bottomN.posZ.Contains('s'))
                myModuleAlternatives.RemoveWhere(r => !r.negZ.Equals(bottomN.posZ));
            else if (bottomN.posZ.Contains('f'))
                myModuleAlternatives.RemoveWhere(r => !r.negZ.Equals(bottomN.posZ.Trim('f')));
        }
                //myModuleAlternatives.RemoveWhere(r => !r.negZ.Equals(bottomN.posZ));


        if (leftN != null)
            if (leftN.posX.Contains("-1"))
                myModuleAlternatives.RemoveWhere(r => !r.negX.Equals("-1f"));
            else if (leftN.posX.Contains('s'))
                myModuleAlternatives.RemoveWhere(r => !r.negX.Equals(leftN.posX));
            else if (leftN.posX.Contains('f'))
                myModuleAlternatives.RemoveWhere(r => !r.negX.Equals(leftN.posX.Trim('f')));

        //myModuleAlternatives.RemoveWhere(r => !r.negX.Equals(leftN.posX));

        if (rightN != null)
            if (rightN.negX.Contains("-1"))
                myModuleAlternatives.RemoveWhere(r => !r.posX.Equals("-1f"));
            else if (rightN.negX.Contains('s'))
                myModuleAlternatives.RemoveWhere(r => !r.posX.Equals(rightN.negX));
            else if (rightN.negX.Contains('f'))
                myModuleAlternatives.RemoveWhere(r => !r.posX.Equals(rightN.negX.Trim('f')));

        //myModuleAlternatives.RemoveWhere(r => !r.posX.Equals(rightN.negX));

        if (WFCAlgorithm.Instance.height3DMax > 1)
        {
            if (above3DN != null)
                    myModuleAlternatives.RemoveWhere(r => !r.posY.Equals(above3DN.negY));

            if (below3DN != null)
                    myModuleAlternatives.RemoveWhere(r => !r.negY.Equals(below3DN.posY));
        }

        if (myModuleAlternatives.Count == 0)
        {
            Debug.Log("No alternatives LEFT ERROR:" + myRow + ", " + myCol + ", " + myHeight3D);
            WFCAlgorithm.Instance.CollapseTile(myRow, myCol, myHeight3D, WFCAlgorithm.Instance.flatGrass);
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
        myText.text = myRow + "," + myCol + "," + myHeight3D + "\nE: " + GetEntropy();
    }
}