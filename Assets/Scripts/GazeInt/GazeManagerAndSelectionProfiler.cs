using MixedReality.Toolkit.Input;
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
    [SerializeField] private float fixationRadius = 0.01f; // Allowable deviation

    private Camera cam = null;
    private Ray actGazeRay;
    public Ray actGazeRayLocal;

    private Vector3 lastGazeHitPoint;
    private Vector3 gazeHitPoint_; //used for aligning eye and head
    private float fixationTimer = 0f; //was nothing (timedelta...)

    private GameObject fixatedObject_ = null;
    private bool isFixated = false;

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

            //TODO: Ray headGaze = new(eyeGaze.origin, Camera.main.transform.forward); 
            GameObject hitObject = gazeHit.collider.gameObject;
            //Debug.Log("Raycast hit: " + hitObject.name);

            // Only process objects with the "TargetObject" tag
            if (!hitObject.CompareTag("TargetObject"))
            {
                ResetFixation();
                return; // Ignore objects without the correct tag
            }
            //Debug.Log("Raycast hit: " + hitObject.name);
            if (fixatedObject_ == hitObject)
            {
                if (Vector3.Distance(lastGazeHitPoint, gazeHit.point) < fixationRadius) //currently jumping to the else that resets the timer. and also could fuck with bjorns code
                {
                    fixationTimer += Time.deltaTime;
                    //Debug.Log($"Fixation timer: {fixationTimer}/{TimeToSelect}");
                    if (fixationTimer >= TimeToSelect)
                    {
                        isFixated = true;
                        Debug.Log("current Fixation"); //never hits this....
                    }
                }
                else //if (fixatedObject_ != gazeHit.transform.gameObject) is from bjorn. also check it's not null i guess
                {
                    Debug.Log("resetting");
                    ResetFixation();
                }
            }
            else
            {
                // New object detected, reset fixation tracking
                fixatedObject_ = hitObject;
                lastGazeHitPoint = gazeHit.point;
                fixationTimer = 0f;
                isFixated = false;
                Debug.Log("NEW fixaton");
            }
        }
        else
        {
            ResetFixation();
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

    public GameObject GetFixatedObject()
    {
        return isFixated ? fixatedObject_ : null;
    }

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
