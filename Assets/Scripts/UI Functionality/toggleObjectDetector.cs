using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggleObjectDetector : MonoBehaviour
{
    public GameObject TargetCollection;      // Assign your empty GameObject that holds the children
    public GameObject hologramCollection;
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
            //deactivate yolo objects
            foreach (Transform child in hologramCollection.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            // Deactivate all children
            foreach (Transform child in TargetCollection.transform)
            {
                child.gameObject.SetActive(false);
            }
            //activate yolo objects
            foreach (Transform child in hologramCollection.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

}
