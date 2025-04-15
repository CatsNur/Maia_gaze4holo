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
    private GameObject hitObject = null;
    private GameObject selectedObject = null;

    private bool select_ = false;
    private bool falseSelectionDetected = false; //TODO: check logic of this hold up

    
    //for the error detection on the selection and related vectors
    private ErrorDetection errorDetection;
    private List<float> gazeAngles = new List<float>();
    private Vector3 lastGV = Vector3.zero; //could this be covered by "lastGazeHitPoint"?
    private float timeToDestroy = 0.25f; //time to destroy the target, which we're not currently doing

    public bool Select()
    {
        //if (!select_ || hitObject == null || selectionInProgress) return false;
        //selectionInProgress = true;
        //StartCoroutine(ResetSelectionFlag());

        if (!select_ || hitObject == null) return false;

        OnSelect?.Invoke(hitObject);
        selectedObject = hitObject;
        //TODO: handle selecting an object and looking away
        //(like you would casually loko around or talk to someone,
        //not necessarily the autoencoder stepping in)

        falseSelectionDetected = false;
        StartCoroutine(RunSelectionError());

        select_ = false;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Gaze Manager and profiler start() called");
        cam = Camera.main;
        errorDetection = GameObject.Find("ErrorDetectionModel").GetComponent<ErrorDetection>();
        if (errorDetection == null)
            Debug.LogError("ErrorDetection component not found.");
            //TODO: code proceeds without model...

    }

    // Update is called once per frame, for eye tracker or for scene...?
    void Update()
    {
        if (cam == null) return;
        
        actGazeRay = new Ray(gazeInteractor.rayOriginTransform.position,
            gazeInteractor.rayOriginTransform.forward * 3); //3m away max....?
        actGazeRayLocal = new Ray(gazeInteractor.rayOriginTransform.localPosition, (gazeInteractor.rayOriginTransform.localRotation * Vector3.forward) * 3);

        UpdateAngleList(); 

        CheckGazeFixation();

        //here if you want to stream data every frame
        /*var datastream = new StreamData(
            timestamp: DateTime.Now.ToString("HH:mm:ss.fff"),
            localGaze: gazeInteractor.rayOriginTransform.localPosition,
            worldGaze: gazeInteractor.rayOriginTransform.position,  
            falseSelect: falseSelectionDetected,
            selectedObject: selectedObject != null ? selectedObject.name : ""
        );
        DataStreamer.Instance.Stream(datastream);*/

        
        if (Select())
        {
            Debug.Log("Selection occurred");
            var datastream = new StreamData(
                timestamp: DateTime.Now.ToString("HH:mm:ss.fff"),
                gazePosition: actGazeRay.origin,
                gazeDirection: actGazeRay.direction,
                falseSelect: falseSelectionDetected,
                selectedObject: selectedObject != null ? selectedObject.name : ""
            );
            DataStreamer.Instance.Stream(datastream);
        }

    }

    private void CheckGazeFixation() {
        //precursor to selection.... head alignment to gaze confirms the selection. 
        RaycastHit gazeHit; //todo: provide visualzation option

        if (Physics.Raycast(actGazeRay, out gazeHit, Mathf.Infinity))
        {

            // Check if the object has a DwellableObject component
            DwellableObject hoverable = gazeHit.collider.GetComponent<DwellableObject>();
            if (hoverable == null)
            {
                ClearFixation();
                return; 
            }

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
                            //Debug.Log("Gaze and head aligned, selecting object: " + hitObject.name);
                            /* THIS CHUNK Moves to Select()
                            falseSelectionDetected = false;
                            OnSelect?.Invoke(hitObject);
                            Select();
                            if (errorDetection == null)
                            {
                                Debug.LogError("ErrorDetection is not assigned in the Manager script!");
                                return; // Don't start the coroutine if errorDetection is null, pretend nothing happened
                            }
                            StartCoroutine(RunSelectionError());*/

                        }
                    }
                }
            }   
        }
        else {
            hitObject = null; 
            ClearFixation();
        }
        
    }
    private void ClearFixation() {
        //moreso handleing dwells
        if (lastDwelledObject != null) {
            OnDwellExit?.Invoke(lastDwelledObject);
            lastDwelledObject = null;
            //falseSelectionDetected = false;//premature potentially
            //select_ = false;
            //Select(); //not sure if necessary here
        }
    }

    private void ResetFixation()
    {
        //when you swap target objects
        fixationTimer = 0f;
        isFixated = false;
        //falseSelectionDetected = false;
        //select_ = false;
        //Select();
    }
    private bool HeadAligned(RaycastHit gH,RaycastHit hH) {
        // takes hit colliders
        return gH.transform.gameObject == hH.transform.gameObject;

    }


    //code from bjorn's laser script going here, gets called when selection happens.. 
    private void UpdateAngleList()
    {
        //Debug.Log("Angle update getting called"); //originally is called everyframe
        Vector3 newGV = actGazeRay.direction; //should this be actGazeRay? local is always 0,0,0
        float angle;

        if (gazeAngles.Count == 0) angle = 0f;
        else angle = Vector3.Angle(lastGV, newGV);
        //Debug.Log("Angle: " + angle.ToString());

        lastGV = newGV;

        gazeAngles.Add(angle);
       
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
                if (errorDetection.CheckError(gazeAngles) || (Input.GetKeyDown(KeyCode.Backspace))) //TODO: For testing trigger this with a keypress
                {
                    Debug.Log($"[ErrorDetection] False selection detected on {hitObject?.name}");
                    falseSelectionDetected = true;
                    SelectionError?.Invoke(hitObject); //happening wayy to fast...

                    var datastream = new StreamData(
                        //does this need to go before the ?.Invoke()?
                        timestamp: DateTime.Now.ToString("HH:mm:ss.fff"),
                        gazePosition: actGazeRay.origin,
                        gazeDirection: actGazeRay.direction,
                        falseSelect: falseSelectionDetected,
                        selectedObject: selectedObject != null ? selectedObject.name : ""
                    );
                    DataStreamer.Instance.Stream(datastream);
                }
                else
                {
                    Debug.Log($"[ErrorDetection] Correct selection on {hitObject?.name}");
                    falseSelectionDetected = false;
                }

                break; // Exit coroutine early after decision

                /*ORIG if (errorDetection.CheckError(gazeAngles))
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
                } END ORIG*/
            }
            yield return null;
        }
        
     }
    /*
    private IEnumerator ResetSelectionFlag()
    {
        yield return new WaitForSeconds(0.25f);
        selectionInProgress = false;
    }
     */
}
