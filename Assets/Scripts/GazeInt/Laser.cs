using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public bool useVR = true;
    public bool useAimBot = true;

    private float aimBotTH;
    [SerializeField] private GameObject muzzlePoint;
    [SerializeField] private Material materialA;
    [SerializeField] private Material materialB;
    [SerializeField] private ParticleSystem particlesA;
    [SerializeField] private ParticleSystem particlesB;
    [SerializeField] private ParticleSystem explosion;

    //i find it strange the error detection stuff is called on a script attached to a prefab
    private ErrorDetection errorDetection;
    private List<float> gazeAngles = new List<float>();
    private Vector3 lastGV = Vector3.zero;

    private LineRenderer beam;
    private GazeIntManager gazeIntManager;
    private TargetManagerScript targetManager; //this will be where all the timers run 

    private float laserMaxTimer;
    //private float laserTimeToDestroy = 0.25f;// now running in the target manager
    private float graceTime;

    private Vector3 origin;
    private Vector3 endPoint;

    private bool beamAactive = false;
    private bool beamBactive = false;
    private bool destroyedObject = false;
    private ISelection select;

    private Camera cam;

    private SelectionOptions usedSelectionOption;
    private bool selectionOptionSet = false;
    private bool drawingLaser = false; //was commented out

    private GameObject headGazePoint;
    private bool showHeadGaze;

    public bool DestroyedObject
    {
        get { return destroyedObject; }
        set { destroyedObject = value; }
    }

    public GameObject FixatedObject
    {
        get 
        {
            if(selectionOptionSet) return select.GetFixatedObject();
            else return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        beam = this.gameObject.AddComponent<LineRenderer>();
        beam.startWidth = 0.02f;
        beam.endWidth = 0.02f;
        beam.enabled = false;
        particlesA.Stop();
        particlesB.Stop();

        cam = this.gameObject.GetComponent<Camera>();

        gazeIntManager = GameObject.Find("ETManager").GetComponent<GazeIntManager>();
        errorDetection = GameObject.Find("ErrorDetectionModel").GetComponent<ErrorDetection>();
        targetManager = GameObject.Find("TargetManager").GetComponent<TargetManagerScript>();
        aimBotTH = gazeIntManager.aimBotTH;

        laserMaxTimer = targetManager.laserTimeToDestroy;

        graceTime = 0;
        /*showHeadGaze = gazeIntManager.showHeadGazePoint;// we're not showing the head gaze so no need
        if(gazeIntManager.ShowHeadGazePoint){
            headGazePoint = Instantiate(gazeIntManager.headGazePoint);
            headGazePoint.transform.position = new(0f,-3,0f);
        }*/
        UpdateSelection();
    }
    private void UpdateSelection() 
    {
        select = new HeadAndGazeSelection();
        //usedSelectionOption = gazeIntManager.SelectionOption
        selectionOptionSet = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        Ray headRay = new Ray(Camera.main.transform.position,Camera.main.transform.forward);
        RaycastHit headGazeHit;
        if (Physics.Raycast(headRay, out headGazeHit, Mathf.Infinity)) {
            Debug.Log("Hit from head gaze. needs to do something, creating a head gaze hit point");
            //headGazePoint.transform.position = headGazeHit.point; //todo: turn back on Friday?
            //something needs to be happening here, for the update in the selection script to do something....
            //Not sure if it's debuggin problem 
        }
        /*
        if(showGeadGaze != manager.showHeadGazePoint){
            headGazePoint.transform.position = new(0f,-3f,0f);
            showHeadGaze = manager.showHeadGazePoint;
        }  
         */
        //Debug.Log("Exiting physics ray cast loop in laser");
        UpdateAngleList();
        //Debug.Log("Safely exit update angle list?");
        Ray eyeGazeRay = gazeIntManager.GetCombinedGazeRay();
        
        select.UpdateGazePath(eyeGazeRay);

        if (select.Select() & !beam.enabled) 
        {
            Debug.Log("select Select() not null and beam not enabled"); //never enter here as of friday
            GameObject go = select.GetFixatedObject();
            TargetScript tar = go.GetComponent<TargetScript>(); 
            StartCoroutine(drawAutoLaser(tar.IsMainTarget)); 
        }
    }
    private void UpdateAngleList() {
        //Get the consecutive angle sequence
        Vector3 newGV = gazeIntManager.actGazeRayLocal.direction; //should be correct
        float angle;
        if (gazeAngles.Count == 0) angle = 0f;
        else angle = Vector3.Angle(lastGV, newGV);

        lastGV = newGV;

        gazeAngles.Add(angle);
        if (gazeAngles.Count > errorDetection.seqLength) {
            gazeAngles.RemoveAt(0);
        }
    }
    IEnumerator drawAutoLaser(bool correctTarget) {
        origin = muzzlePoint.transform.position;
        endPoint = select.GetHitPoint();
        beam.material = materialA;
        beam.SetPosition(0, origin);
        beam.SetPosition(1, endPoint);
        GameObject target = select.GetFixatedObject();
        float laserTimer = 0f;

        particlesA.transform.position = endPoint;
        particlesA.transform.forward = -(endPoint - origin);
        particlesA.Play();
        beam.enabled = true;
        //soundManager.PlayLaserSound(origin);
        while (laserTimer < (targetManager.laserTimeToDestroy + 0.05f)) { 
        //TODO: double check the timer in target manager is doing something
            laserTimer += Time.deltaTime;
            if (laserTimer > (errorDetection.decisionTime / 1000)) {
                if (errorDetection.CheckError(gazeAngles))
                {
                    //the autoencoder steps in
                    Debug.Log("Detect a False Selection");
                    break;
                }
                else {
                    //all good
                    Debug.Log("Detect True Selection");
                }
            }
            if (target == null) {
                break;
            }
            /*if (target.GetComponent<Target>() != null) {
                target.GetComponent<Target>().PercentHitPointsUpdate(Time.deltaTime / laserMaxTimer);
                //^affects the change of the outline color,i.e., goes more to red with low health (in target script)
            }*/
            yield return null;
        }//end while
        if (target != null) {
            //so target not already destroyed
            Debug.Log("Target contains target, the points manager would technically update");
        }
        beam.enabled = false;
        //soundManager.StopLaserSound();
        particlesA.Stop();
        yield return null;

    }

    private void drawLaser(Material material)
    {
        beam.material = material;
        origin = muzzlePoint.transform.position;

        Ray ray;
        ray = gazeIntManager.GetCombinedGazeRay();
        beam.SetPosition(0,origin);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) { 
            endPoint = hit.point;
            GameObject target = hit.collider.gameObject;
            //originally the code checks for target a or b and whether that's correct, but we don't care about that anymore
            //something something grace time, and percentHitPointUpdate called
        }
        beam.SetPosition(1, endPoint);
        if (beamAactive) { 
            particlesA.transform.position = endPoint;
            particlesA.transform.forward = -(endPoint - origin);
            if (!particlesA.isPlaying) { particlesA.Play(); }
        }
        if (beamBactive) { 
            particlesB.transform.position = endPoint;
            particlesB.transform.forward = -(endPoint - origin);
            if (!particlesB.isPlaying) {particlesB.Play(); }
        }
        //some if statement for soundmanager
        beam.enabled = true;
    }

}
