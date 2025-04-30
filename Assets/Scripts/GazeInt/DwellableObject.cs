using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwellableObject : MonoBehaviour
{
    private Renderer objRenderer;
    private Material originalMaterial;
    [SerializeField] public Material selectedMaterial; // Assign this in the Inspector
    [SerializeField] public Material falseSelectionMaterial;

    private Coroutine selectionCoroutine;

    public bool hasSelected = false;

    void Awake()
    {
        //objRenderer = GetComponent<Renderer>(); //this assumes its on obj, assed chatgpt for sophis
        //originalMaterial = objRenderer.material; // Store the original material
        //get object name
        Debug.Log("DwellableObject is attached to: " + gameObject.name);

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
            //Debug.Log("orig material stored");
            Debug.Log("Material name: " + objRenderer.material.name);
        }
    }

    void OnEnable()
    {
        GazeManagerAndSelectionProfiler.OnDwellEnter += HandleDwellEnter;
        GazeManagerAndSelectionProfiler.OnDwellStay += HandleDwellStay;
        GazeManagerAndSelectionProfiler.OnDwellExit += HandleDwellExit;
        GazeManagerAndSelectionProfiler.OnSelect += HandleSelection;
        GazeManagerAndSelectionProfiler.SelectionError += SelectionError;
    }
    void OnDisable()
    {
        GazeManagerAndSelectionProfiler.OnDwellEnter -= HandleDwellEnter;
        GazeManagerAndSelectionProfiler.OnDwellStay -= HandleDwellStay;
        GazeManagerAndSelectionProfiler.OnDwellExit -= HandleDwellExit;
        GazeManagerAndSelectionProfiler.OnSelect -= HandleSelection;
        GazeManagerAndSelectionProfiler.SelectionError -= SelectionError;
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
            //hasSelected = false;
            if (!hasSelected)
            {
                objRenderer.material = originalMaterial;
            }
        }
    }
    private void HandleSelection(GameObject obj) {
        if (obj == gameObject) {
            Debug.Log($"HandleSelection called, hasSelected: {hasSelected}");
            if (!hasSelected)
            {
                Debug.Log("Selected");
                objRenderer.material = selectedMaterial;
                hasSelected = true;

                if (selectionCoroutine != null) StopCoroutine(selectionCoroutine);
                selectionCoroutine = StartCoroutine(SelectionHoldDuration(2.0f));
            }
        }
    }
    private IEnumerator SelectionHoldDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        ResetSelection(); // reset visual + selection state
    }
    private void SelectionError(GameObject obj)
    {
        if (obj == gameObject && hasSelected) 
            // if selection error occur, while selected, trigger false selected
            //error detection model only starts once selection triggered in gazemanager
        {
            Debug.Log("False Selected");
            //objRenderer.material = falseSelectionMaterial;
            //hasSelected = false;

            // short wait a moment? considering it jumps strait to hovering...
            StopAllCoroutines(); 
            StartCoroutine(HandleFalseSelection());
        }
    }

    private IEnumerator HandleFalseSelection()
    {
        objRenderer.material = falseSelectionMaterial;
        //hasSelected = false;
        yield return new WaitForSeconds(0.5f); // adjust time as needed, now 500ms
        //objRenderer.material = originalMaterial;
        ResetSelection();
    }

    public void ResetSelection()
    {
        //could call this in the gaze manager
        hasSelected = false;
        objRenderer.material = originalMaterial;
    }

}
