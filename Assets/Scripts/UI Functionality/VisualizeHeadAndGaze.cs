using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit.Input;
using UnityEditor.PackageManager;

public class VisualizeHeadAndGaze : MonoBehaviour
{
    [SerializeField]
    private GazeInteractor gazeInteractor;//too lazy to make this internal

    public GameObject headSpherePrefab;
    private GameObject headSphereInstance;
    public GameObject gazeSpherePrefab;
    private GameObject gazeSphereInstance;
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
                headSphereInstance = Instantiate(headSpherePrefab);
                gazeSphereInstance = Instantiate(gazeSpherePrefab);
            }
        }
        else
        {

            if (headSphereInstance != null) {
                Destroy(headSphereInstance);
                Destroy(gazeSphereInstance);
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
                Vector3 headPosition = Camera.main.transform.position;
                Vector3 headForward = Camera.main.transform.forward;

                var ray = new Ray(gazeInteractor.rayOriginTransform.position,
                gazeInteractor.rayOriginTransform.forward * 3);

                // Offset: 0.3 meters (30 cm) in front of your face
                headSphereInstance.transform.position = headPosition + headForward * 0.3f;
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    gazeSphereInstance.transform.position = hitInfo.point;
                }
                else
                {
                    gazeSphereInstance.transform.position = gazeInteractor.rayOriginTransform.position + gazeInteractor.rayOriginTransform.forward * 0.3f;
                }
            }
        }
    }
}
