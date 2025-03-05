using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwellableObject : MonoBehaviour
{
    private Renderer objRenderer;
    private Material originalMaterial;
    [SerializeField] public Material selectedMaterial; // Assign this in the Inspector

    void Awake()
    {
        //objRenderer = GetComponent<Renderer>(); //this assumes its on obj, assed chatgpt for sophis
        //originalMaterial = objRenderer.material; // Store the original material

        // First, try to get the Renderer from the parent object
        objRenderer = GetComponent<Renderer>();
        // If no Renderer is found on the parent, try finding it in the children
        if (objRenderer == null)
        {
            objRenderer = GetComponentInChildren<Renderer>();
        }
        // If no Renderer is found in the object or its children, log an error
        if (objRenderer == null)
        {
            Debug.LogError("No Renderer found in the object or its children for " + gameObject.name);
        }
        else
        {
            originalMaterial = objRenderer.material; // Store the original material
        }
    }

    void OnEnable()
    {
        GazeManagerAndSelectionProfiler.OnDwellEnter += HandleDwellEnter;
        GazeManagerAndSelectionProfiler.OnDwellStay += HandleDwellStay;
        GazeManagerAndSelectionProfiler.OnDwellExit += HandleDwellExit;
        GazeManagerAndSelectionProfiler.OnSelect += HandleSelection;
    }
    void OnDisable()
    {
        GazeManagerAndSelectionProfiler.OnDwellEnter -= HandleDwellEnter;
        GazeManagerAndSelectionProfiler.OnDwellStay -= HandleDwellStay;
        GazeManagerAndSelectionProfiler.OnDwellExit -= HandleDwellExit;
        GazeManagerAndSelectionProfiler.OnSelect -= HandleSelection;
    }
    private void HandleDwellEnter(GameObject obj) 
    {
        if (obj == gameObject) {
            Debug.Log("Dwell Enter " + obj.name);
            //can do somethins
        }
            
    }
    private void HandleDwellStay(GameObject obj)
    {
        if (obj == gameObject) {
            Debug.Log("Dwell is hovering over: " + obj.name);
            //do the selection paradigm, or maybe invoke?
        }
    }
    private void HandleDwellExit(GameObject obj) 
    {
        if (obj == gameObject)
        {
            Debug.Log("Dwell exited: " + obj.name);
        }
    }
    private void HandleSelection(GameObject obj) {
        if (obj == gameObject) {
            Debug.Log("Selected");
            //lets change Material to orange
            objRenderer.material = selectedMaterial;
        }
    }

}
