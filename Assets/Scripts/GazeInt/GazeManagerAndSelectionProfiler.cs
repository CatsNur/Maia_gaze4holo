using MixedReality.Toolkit.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Vector3 gazeHitPoint_;
    private float fixationTimer = 0f; //was nothing (timedelta...)

    
    private bool isFixated = false;
    // actions that will talk to code on the targets
    public static event Action<GameObject> OnDwellEnter;
    public static event Action<GameObject> OnDwellStay;
    public static event Action<GameObject> OnDwellExit;
    public static event Action<GameObject> OnSelect;
    public static event Action<GameObject> SelectionError;
    private GameObject lastDwelledObject;
    private GameObject hitObject = null;  // making this available here, but see if last dwellable object is doing/ how often it's updating

    private bool select_ = false;
    private bool falseSelectionDetected = false; //TODO: check logic of this hold up

    
    //for the error detection on the selection and related vectors
    private ErrorDetection errorDetection;
    private List<float> gazeAngles = new List<float>();
    private Vector3 lastGV = Vector3.zero; //could this be covered by "lastGazeHitPoint"?
    private float timeToDestroy = 0.25f; //time to destroy the target, which we're not currently doing

    /*public bool Select() //not being used currently...
    {
        if (select_)
        {
            Debug.Log("Should select");
            select_ = false;
            return true;
        }
        return false;
    }*/

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Gaze Manager and profiler start() called");
        cam = Camera.main;
        errorDetection = GameObject.Find("ErrorDetectionModel").GetComponent<ErrorDetection>();

    }

    // Update is called once per frame, for eye tracker or for scene...?
    void Update()
    {
        if (cam != null)
        {
            actGazeRay = new Ray(gazeInteractor.rayOriginTransform.position,
                gazeInteractor.rayOriginTransform.forward * 3); //3m away max....?
            actGazeRayLocal = new Ray(gazeInteractor.rayOriginTransform.localPosition, (gazeInteractor.rayOriginTransform.localRotation * Vector3.forward) * 3);

            //from chatgpt, not working?
            /*actGazeRayLocal = new Ray(
                gazeInteractor.rayOriginTransform.localPosition, // Local position (relative to parent)
                (gazeInteractor.rayOriginTransform.localRotation * Vector3.forward) * 3 // Local forward direction, 3 meters away
            );*/

            UpdateAngleList(); //TODO: confirm this is where this should be called

            CheckGazeFixation();

            //test if our streamer goes here safely, it does except for the false selection
            var datastream = new StreamData(
                timestamp: DateTime.Now.ToString("HH:mm:ss.fff"),
                localGaze: gazeInteractor.rayOriginTransform.localPosition,
                worldGaze: gazeInteractor.rayOriginTransform.position,  
                falseSelect: falseSelectionDetected,
                selectedObject: hitObject != null ? hitObject.name : ""
            );
            DataStreamer.Instance.Stream(datastream);
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
                return; 
            }


            //GameObject hitObject = gazeHit.collider.gameObject; //originally defined first here, but harder to integrate with the data streamer
            hitObject = gazeHit.collider.gameObject; 
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
                
                Ray headGaze = new(actGazeRay.origin, Camera.main.transform.forward);
                fixationTimer += Time.deltaTime;
                if (fixationTimer >= TimeToSelect)
                {
                    //if dwell longer than the given threshold, we have a stay and can check selection quali
                    OnDwellStay?.Invoke(hitObject);
                    RaycastHit headHit;
                    if (Physics.Raycast(headGaze, out headHit, Mathf.Infinity))//checking selection quali
                    {
                        //if a head collider happens
                        gazeHitPoint_ = gazeHit.point; //do i need this point, maybe for visualization?
                        if (HeadAligned(gazeHit,headHit))
                        {
                            select_ = true;
                            OnSelect?.Invoke(hitObject);
                            if (errorDetection == null)
                            {
                                Debug.LogError("ErrorDetection is not assigned in the Manager script!");
                                return; // Don't start the coroutine if errorDetection is null, pretend nothing happened
                            }
                            StartCoroutine(RunSelectionError());
                            //if selection false
                            if (falseSelectionDetected)
                            {
                                SelectionError?.Invoke(hitObject);
                                select_ = false;
                            }
                        }
                    }

                    //fixationTime_ = 0.0f; //do i need this? currently no, maybe if we have a dwel paradigm
                }
            }   
        }
        else {
            hitObject = null; 
            ClearFixation();
        }
        
    }
    private void ClearFixation() {
        if (lastDwelledObject != null) {
            OnDwellExit?.Invoke(lastDwelledObject);
            lastDwelledObject = null;
            falseSelectionDetected = false;//not sure if necessary here
            select_ = false;
        }
    }

    private void ResetFixation()
    {
        fixationTimer = 0f;
        isFixated = false;
        falseSelectionDetected = false;
    }
    private bool HeadAligned(RaycastHit gH,RaycastHit hH) {
        // takes hit colliders
        return gH.transform.gameObject == hH.transform.gameObject;

    }

    /*public bool IsFixated() //not using currently
    {
        return isFixated;
    }*/

    /*public GameObject GetFixatedObject()
    {//Don't think we need this anymore with the actions
        return isFixated ? fixatedObject_ : null;
    }*/

    /*public Vector3 GetHitPoint() //not using currently
    {
        return isFixated ? lastGazeHitPoint : Vector3.zero;
    }*/


    /*public Ray GetCombinedGazeRay() //not using currently
    {
        //Debug.Log(gazeInteractor.rayOriginTransform.position);
        actGazeRay = new Ray(gazeInteractor.rayOriginTransform.position,
                gazeInteractor.rayOriginTransform.forward);
        return actGazeRay;
    }*/


    //code from bjorn's laser script going here, gets called when selection happens.. 
    private void UpdateAngleList()
    {
        //Debug.Log("Angle update getting called"); //originally is called everyframe
        Vector3 newGV = actGazeRayLocal.direction;//for htc vive pro eye it's actGazeRayLocal.direction; 
        //Debug.Log("New GV: " + newGV.ToString());
        //Debug.Log("Last GV: " + lastGV.ToString());
        float angle;

        if (gazeAngles.Count == 0) angle = 0f;
        else angle = Vector3.Angle(lastGV, newGV);
        //Debug.Log("Angle: " + angle.ToString());

        lastGV = newGV;

        gazeAngles.Add(angle);
        

        //Debug.Log("GazeAngles: " + gazeAngles.Count.ToString());
       
        if (gazeAngles.Count > errorDetection.seqLength)//todo fix this so not hardcoded
        {
            gazeAngles.RemoveAt(0);
        }
    }
    IEnumerator RunSelectionError() 
    {
        //Attach an animated robot arm that starts a movement process once this is called
        //FYI: Laser time to destroy is 0.25f
        float selectionTimer = 0.0f;
        while (selectionTimer < (timeToDestroy + 0.05f))
        {
            selectionTimer += Time.deltaTime;
            if (selectionTimer > (errorDetection.decisionTime / 1000)) //convert to ms //also not called properly?
            {
                if (errorDetection.CheckError(gazeAngles))
                {
                    Debug.Log("Detected false selection.");
                    falseSelectionDetected = true;
                    //errorDetectionRecorder.AddLine(correctTarget, errorDetection.GetLastMSE(), errorDetection.Threshold);
                    break;
                }
                else 
                {
                    Debug.Log("Detected correct selection.");
                    falseSelectionDetected = false;
                    //errorDetectionRecorder.AddLine(correctTarget, errorDetection.GetLastMSE(), errorDetection.Threshold);
                }
            }
            yield return null;
        }
        
     }
}
