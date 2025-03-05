using MixedReality.Toolkit.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeManagerAndSelectionProfiler : MonoBehaviour
{
    public enum SelectionOptions
    {
        //gaze, //1
        headAndGaze,//2
        //nod //3, all out commented...
    }
    [SerializeField] private GazeInteractor gazeInteractor;
    public SelectionOptions selectOption;
    [SerializeField] private float TimeToSelect = 1f;
    [SerializeField] private float fixationRadius = 0.01f; // Allowable deviation, not in Bjorns orig code tho

    private Camera cam = null;
    private Ray actGazeRay;
    public Ray actGazeRayLocal;

    private Vector3 lastGazeHitPoint;
    private Vector3 gazeHitPoint_; //used for aligning eye and head
    private float fixationTimer = 0f; //was nothing (timedelta...)

    
    private bool isFixated = false;
    //trying something new with actions that will talk to code on the targets
    public static event Action<GameObject> OnDwellEnter;
    public static event Action<GameObject> OnDwellStay;
    public static event Action<GameObject> OnDwellExit;
    private GameObject lastDwelledObject;
    
    private bool select_ = false;

    public bool Select()
    {
        if (select_)
        {
            // Debug.Log("Should select");
            select_ = false;
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Gaze Manager start() called");
        cam = Camera.main;//not sure where this comes in anymore

    }

    // Update is called once per frame, for eye tracker or for scene...?
    void Update()
    {
        if (cam != null)
        {
            actGazeRay = new Ray(gazeInteractor.rayOriginTransform.position,
                gazeInteractor.rayOriginTransform.forward * 3); //3m away max....?
            actGazeRayLocal = new Ray(gazeInteractor.rayOriginTransform.localPosition, (gazeInteractor.rayOriginTransform.localRotation * Vector3.forward) * 3);
            //Hopefully that is the local and is correct. 
            //Debug.Log("access gaze");

            CheckGazeFixation();
        }
    }

    private void CheckGazeFixation() {
        //precursor to selection.... head alignment to gaze confirms the selection. 
        //Debug.Log("CheckFixation");
        RaycastHit gazeHit;

        if (Physics.Raycast(actGazeRay, out gazeHit, Mathf.Infinity))
        {

            // Check if the object has a DwellableObject component
            DwellableObject hoverable = gazeHit.collider.GetComponent<DwellableObject>();
            if (hoverable == null)
            {
                ClearFixation();
                return; // Ignore objects without HoverableObject
            }

            //TODO: Ray headGaze = new(eyeGaze.origin, Camera.main.transform.forward); 
            GameObject hitObject = gazeHit.collider.gameObject;
            //Debug.Log("Raycast hit: " + hitObject.name);

            if (hitObject != lastDwelledObject)
            {
                if (lastDwelledObject != null)
                    OnDwellExit?.Invoke(lastDwelledObject);

                lastDwelledObject = hitObject;
                ResetFixation();
                OnDwellEnter?.Invoke(hitObject);
            }
            else
            {
                fixationTimer += Time.deltaTime;
                if (fixationTimer >= TimeToSelect)
                {
                    //if dwell longer than the given threshold, we have a stay and can check selection quali
                    //select_ = true; only if align of head
                    OnDwellStay?.Invoke(hitObject);
                    //fixationTime_ = 0.0f; do i need this
                }
            }   
        }
        else {
            ClearFixation();
        }
        
    }
    private void ClearFixation() {
        if (lastDwelledObject != null) {
            OnDwellExit?.Invoke(lastDwelledObject);
            lastDwelledObject = null;
        }
    }

    private void ResetFixation()
    {
        fixationTimer = 0f;
        isFixated = false;
    }

    public bool IsFixated()
    {
        return isFixated;
    }

    /*public GameObject GetFixatedObject()
    {//Don't think we need this anymore with the actions
        return isFixated ? fixatedObject_ : null;
    }*/

    public Vector3 GetHitPoint()
    {
        return isFixated ? lastGazeHitPoint : Vector3.zero;
    }


    public Ray GetCombinedGazeRay()
    {
        //Debug.Log(gazeInteractor.rayOriginTransform.position);
        actGazeRay = new Ray(gazeInteractor.rayOriginTransform.position,
                gazeInteractor.rayOriginTransform.forward);
        return actGazeRay;
    }

    /*ORIGINAL
     * public GameObject GetFixatedObject()
    {
        return fixatedObject_;
    }

    public Vector3 GetHitPoint()
    {
        return gazeHitPoint_;
    }*/
}
