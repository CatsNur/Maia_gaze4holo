using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeHeadAndGaze : MonoBehaviour
{
    public GameObject headSpherePrefab;
    private GameObject headSphereInstance;
    private bool trackingEnabled = false;

    // Called from Toggle Button
    public void ToggleTracking(bool isOn)
    {
        trackingEnabled = isOn;
        Debug.Log("Vis Tracking is " + (trackingEnabled ? "enabled" : "disabled"));

        if (trackingEnabled)
        {
            // Check if the headSphereInstance is null before instantiating
            if (headSphereInstance == null)
            {
                Debug.Log("Instantiating headSphereInstance");
                headSphereInstance = Instantiate(headSpherePrefab);
            }
            Debug.Log("Showing head");
        }
        else
        {

            if (headSphereInstance != null) {
                Debug.Log("Not Showing Head");
                Destroy(headSphereInstance);
            }
               
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (trackingEnabled && headSphereInstance != null)
        {
            if (trackingEnabled && headSphereInstance != null && Camera.main != null)
            {
                headSphereInstance.transform.position = Camera.main.transform.position;
            }
        }
    }
}
