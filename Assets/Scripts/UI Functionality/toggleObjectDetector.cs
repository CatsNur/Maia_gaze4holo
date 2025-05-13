using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggleObjectDetector : MonoBehaviour
{
    public GameObject TargetCollection;      // Assign your empty GameObject that holds the children
    private bool targetsActivated = true; // Track the toggle state

    public void ToggleTracking(bool isOn)
    {
        targetsActivated = isOn;

        if (targetsActivated)
        {
            // Activate all children
            foreach (Transform child in TargetCollection.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            // Deactivate all children
            foreach (Transform child in TargetCollection.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

}
