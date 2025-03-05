using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwellableObject : MonoBehaviour
{
    void OnEnable()
    {
        GazeManagerAndSelectionProfiler.OnDwellEnter += HandleDwellEnter;
        GazeManagerAndSelectionProfiler.OnDwellStay += HandleDwellStay;
        GazeManagerAndSelectionProfiler.OnDwellExit += HandleDwellExit;
    }
    void OnDisable()
    {
        GazeManagerAndSelectionProfiler.OnDwellEnter -= HandleDwellEnter;
        GazeManagerAndSelectionProfiler.OnDwellStay -= HandleDwellStay;
        GazeManagerAndSelectionProfiler.OnDwellExit -= HandleDwellExit;
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
            //do the selection paradigm
        }
    }
    private void HandleDwellExit(GameObject obj) 
    {
        if (obj == gameObject)
        {
            Debug.Log("Mouse exited: " + obj.name);
        }
    }

}
