using MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectionOptions { 
gaze, //1
headAndGaze,//2
nod //3, all out commented...
}

public class GazeIntManager : MonoBehaviour
{
    [SerializeField] private GazeInteractor gazeInteractor;
    //[SerializeField]TODO
    //public SelectionOptions selectOption;

    private Camera cam = null;
    private Ray actGazeRay;
    public Ray actGazeRayLocal;

    public bool useAimbot = true;
    public float aimBotTH = 5f; //why 5?
    public float timeToSelect = 1f;
    public float[] nodTHs = {5,10 };//new float[2];

    public bool drawGaze = true; //where else is this showing up...nowhere.... testing what happens when trues

    //**private Manager manager;//add back in unless we do the manager here
    private TargetManagerScript targetManager;
    //private GameObject laser;

    /*nora
   
    private long actTimestamp;

    public long ActTimestamp
    {
        get { return actTimestamp; }
    }
     nora*/


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Gaze Interaction Manager start() called");
        cam = Camera.main;
        targetManager = GameObject.Find("TargetManager").GetComponent<TargetManagerScript>();
        //call the laser gameobject here, pull the prefab in completely, or maybe call it in the target manager...

    }

    // Update is called once per frame, replacing bjoern's newsample function (his update function is empty...)
    void Update()
    {
        if (cam != null)
        {
            actGazeRay = new Ray(gazeInteractor.rayOriginTransform.position,
                gazeInteractor.rayOriginTransform.forward * 3); //3m away max....?
           actGazeRayLocal = new Ray(gazeInteractor.rayOriginTransform.localPosition, (gazeInteractor.rayOriginTransform.localRotation * Vector3.forward)*3); 
            //Hopefully that is the local and is correct. 
        }
    }

    /*norapublic void NewSample(SampleData s) 
    { 
        if (cam != null)
            {
                if (s.isValid)
                {
                
                
                    switch (manager.eye)
                    {
                        case Eye.Both:
                            actGazeRay = new Ray(s.worldGazeOrigin, s.worldGazeDirection);
                            actGazeRayLocal = new Ray(s.localGazeOrigin, s.localGazeDirection);
                            break;
                        case Eye.Left:
                            actGazeRay = new Ray(s.worldGazeOrigin_L, s.worldGazeDirection_L);
                            actGazeRayLocal = new Ray(s.localGazeOrigin_L, s.localGazeDirection_L);
                            break;
                        case Eye.Right:
                            actGazeRay = new Ray(s.worldGazeOrigin_R, s.worldGazeDirection_R);
                            actGazeRayLocal = new Ray(s.localGazeOrigin_R, s.localGazeDirection_R);
                            break;
                    }
                
                    // actGazeRayLocal = new Ray(s.localGazeOrigin, s.localGazeDirection);
                    // actGazeRay = new Ray(s.worldGazeOrigin, s.worldGazeDirection);
                }
                actTimestamp = s.timeStamp;
            }
    }nora*/

    public Ray GetCombinedGazeRay()
    {
        //Debug.Log(gazeInteractor.rayOriginTransform.position);
        actGazeRay = new Ray(gazeInteractor.rayOriginTransform.position,
                gazeInteractor.rayOriginTransform.forward);
        return actGazeRay;
    }
}
