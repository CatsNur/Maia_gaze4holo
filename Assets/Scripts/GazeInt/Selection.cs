using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public interface ISelection
{
    public bool Select();
    public void UpdateGazePath(Ray eyeGaze);
    public GameObject GetFixatedObject();
    public Vector3 GetHitPoint();
}

/*public class NodSelection: ISelection
{
    private float fixationTime_ = 0.0f;
    private List<Quaternion> headRots = new List<Quaternion>();
    private GameObject fixatedObject_ = null;
    private Vector3 gazeHitPoint_;
    private bool select_ = false;

    //private Manager manager;
    private GazeIntManager gazeIntManager;
    private Camera cam;
    private LineRenderer eyeGazeBeam;




    public NodSelection()
    {
        gazeIntManager = GameObject.Find("ETManager").GetComponent<GazeIntManager>(); //hopefully correct
        cam = Camera.main;
        

        eyeGazeBeam = new GameObject("Nod Beam").AddComponent<LineRenderer>();
        eyeGazeBeam.startWidth = 0.01f;
        eyeGazeBeam.endWidth = 0.01f;
    }

    public bool Select()
    {
        if (select_)
        {
            select_ = false;
            return true;
        }
        return false;
    }
    public void UpdateGazePath(Ray eyeGaze)
    {
        if (gazeIntManager.useAimbot)
        {
            eyeGaze = GazeSelection.AimBot(eyeGaze, gazeIntManager.aimBotTH);
        }

        RaycastHit gazeHit;

        if (Physics.Raycast(eyeGaze, out gazeHit, Mathf.Infinity))
        {
            gazeHitPoint_ = gazeHit.point;
            if (gazeIntManager.drawGaze)
            {
                if (!eyeGazeBeam.enabled) eyeGazeBeam.enabled = true;
                eyeGazeBeam.SetPosition(0, eyeGaze.origin);
                eyeGazeBeam.SetPosition(1, eyeGaze.origin + eyeGaze.direction.normalized * 1.5f);
            }
            else if (eyeGazeBeam.enabled) eyeGazeBeam.enabled = false;

            if (fixatedObject_ != gazeHit.transform.gameObject)
            {
                fixationTime_ = 0.0f;
                headRots.Clear();
                if (fixatedObject_ != null)
                {
                    if (fixatedObject_.GetComponent<TargetScript>() != null)
                    {
                        fixatedObject_.GetComponent<TargetScript>().DisableHighlight();
                    }
                }
                fixatedObject_ = gazeHit.transform.gameObject;

                if (fixatedObject_.GetComponent<TargetScript>() != null)
                {
                    if (!fixatedObject_.GetComponent<TargetScript>().IsHighlighted())
                    {
                        fixatedObject_.GetComponent<TargetScript>().Highlight();
                    }
                }
            }

            if (fixatedObject_.transform.IsChildOf(GameObject.Find("Room").transform)) return; //TODO: don't need...


            if (fixationTime_ < gazeIntManager.timeToSelect)
            {
                fixationTime_ += Time.deltaTime;
            }
            else
            {
                headRots.RemoveAt(0);
            }
            headRots.Add(cam.transform.rotation);

            if (CheckNod())
            {
                select_ = true;
                fixationTime_ = 0.0f;
            }
        }
        else
        {
            if (fixatedObject_ != null)
            {
                if (fixatedObject_.GetComponent<TargetScript>() != null)
                {
                    if (fixatedObject_.GetComponent<TargetScript>().IsHighlighted())
                    {
                        fixatedObject_.GetComponent<TargetScript>().DisableHighlight();
                    }
                }
            }
        }
    }

    private bool CheckNod()
    {
        Vector3 upStart = headRots[0] * Vector3.up;
        Vector3 forwardAct;
        float angle;
        bool moveFound = false;
        foreach (Quaternion rotAct in headRots)
        {
            forwardAct = rotAct * Vector3.forward;
            angle = Mathf.Asin(Vector3.Dot(forwardAct, upStart)) * 180 / Mathf.PI;
            if (!moveFound & (Mathf.Abs(angle) > gazeIntManager.nodTHs[1]))
            {
                // Debug.Log("[DEBUG] Move Detected!");
                moveFound = true;
            }
            if (moveFound && (Mathf.Abs(angle) < gazeIntManager.nodTHs[0]))
            {
                return true;
            }
            // else if (moveFound) { Debug.Log("[DEBUG] Angle " + angle.ToString()); }
            // if (moveFound) Debug.Log("[DEBUG] no move back!");
        }
        return false;
    }


    public GameObject GetFixatedObject()
    {
        return fixatedObject_;
    }
    public Vector3 GetHitPoint()
    {
        return gazeHitPoint_;
    }

}*/

public class GazeSelection: ISelection
{
    //all for GazeDwell, should not be used!! EXCEPT AIMBOT.... ugh

    // private float timeToSelect_;
    private float fixationTime_;
    private GameObject fixatedObject_ = null;
    private Vector3 gazeHitPoint_;
    private bool select_ = false;

    private GazeIntManager gazeIntManager;
    private LineRenderer eyeGazeBeam;

    public GazeSelection()
    {
        gazeIntManager = GameObject.Find("ETManager").GetComponent<GazeIntManager>();
        // timeToSelect_ = timeToSelect;

        eyeGazeBeam = new GameObject("GazeDraw").AddComponent<LineRenderer>();
        eyeGazeBeam.startWidth = 0.01f;
        eyeGazeBeam.endWidth = 0.01f;
    }

    static public Ray AimBot(Ray gaze, float aimBotTH) // is used!!!
    {
        //Debug.Log("In Aimbot func");
        //is gaze null here,no
        GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetObject"); //finds the targets sucessfully in the scene
        float minAngle = aimBotTH;
        float angleBetween;
        Ray newGaze = new Ray(gaze.origin, gaze.direction);
        foreach (GameObject target in targets)
        {
            //Debug.Log("for loop in aimbot entered");
            TargetScript targetScript = target.GetComponent<TargetScript>();
            //is target the target update() getting called here?
            Vector3 targetDir = target.transform.position - gaze.origin;
            angleBetween = Vector3.Angle(targetDir, gaze.direction);
            if (angleBetween < minAngle)
            {
                //Debug.Log("new gaze being created");
                newGaze = new Ray(gaze.origin, targetDir);
                minAngle = angleBetween;
            }
        }
        return newGaze;
    }

    public bool Select()//not implemented yet
    {
        if (select_)
        {
            select_ = false;
            return true;
        }
        return false;
    }
    public void UpdateGazePath(Ray eyeGaze)//not implemented yet
    {
        if (gazeIntManager.useAimbot)
        {
            eyeGaze = AimBot(eyeGaze, gazeIntManager.aimBotTH);
        }

        RaycastHit gazeHit;

        if (Physics.Raycast(eyeGaze, out gazeHit, Mathf.Infinity))
        {
            gazeHitPoint_ = gazeHit.point;
            if (gazeIntManager.drawGaze)
            {
                if (!eyeGazeBeam.enabled) eyeGazeBeam.enabled = true;
                eyeGazeBeam.SetPosition(0, eyeGaze.origin);
                eyeGazeBeam.SetPosition(1, eyeGaze.origin + eyeGaze.direction.normalized * 1.5f);
            }
            else if (eyeGazeBeam.enabled) eyeGazeBeam.enabled = false;

            if (fixatedObject_ != gazeHit.transform.gameObject)
            {
                fixationTime_ = 0.0f;
                if (fixatedObject_ != null)
                {
                    if (fixatedObject_.GetComponent<TargetScript>() != null)
                    {
                        fixatedObject_.GetComponent<TargetScript>().DisableHighlight();
                    }
                }
                fixatedObject_ = gazeHit.transform.gameObject;


                if (fixatedObject_.GetComponent<TargetScript>() != null)
                {
                    if (!fixatedObject_.GetComponent<TargetScript>().IsHighlighted())
                    {
                        fixatedObject_.GetComponent<TargetScript>().Highlight();
                        //follow target code snippits 
                    }

                }
            }
            Debug.Log("Fixated Object " + fixatedObject_.name);

            //*if (!fixatedObject_.transform.IsChildOf(GameObject.Find("Room").transform)) fixationTime_ += Time.deltaTime;
            fixationTime_ += Time.deltaTime;

            if (fixationTime_ > gazeIntManager.timeToSelect)
            {
                select_ = true;
                fixationTime_ = 0.0f;
            }
        } else
        {
            if (fixatedObject_ != null)
            {
                if (fixatedObject_.GetComponent<TargetScript>() != null)
                {
                    if (fixatedObject_.GetComponent<TargetScript>().IsHighlighted())
                    {
                        fixatedObject_.GetComponent<TargetScript>().DisableHighlight();
                    }
                }
            }
        }
    }
    public GameObject GetFixatedObject()//not implemented yet
    {
        return fixatedObject_;
    }
    public Vector3 GetHitPoint()//not implemented yet
    {
        return gazeHitPoint_;
    }
}

public class HeadAndGazeSelection: ISelection
{
    /// <summary>
    /// This is the default...
    /// </summary>

    private float fixationTime_;
    private GameObject fixatedObject_ = null;
    private Vector3 gazeHitPoint_;
    private bool select_ = false;

    private GazeIntManager gazeIntManager;

    private LineRenderer eyeGazeBeam;
    private LineRenderer headGazeBeam;
    public HeadAndGazeSelection()
    {
        Debug.Log("HEAD And GAZE from selection script called"); //not getting called currently 
        gazeIntManager = GameObject.Find("ETManager").GetComponent<GazeIntManager>();


        headGazeBeam = new GameObject("Head Beam").AddComponent<LineRenderer>();
        headGazeBeam.startWidth = 0.01f;
        headGazeBeam.endWidth = 0.01f;

        eyeGazeBeam = new GameObject("H Gaze Beam").AddComponent<LineRenderer>();
        eyeGazeBeam.startWidth = 0.01f;
        eyeGazeBeam.endWidth = 0.01f;
    }

    private Ray CorrectHead(Ray headGaze, Vector3 gazeHitPoint) //not using? is necessary?
    {
        Vector3 targetDir = gazeHitPoint - headGaze.origin;
        if (Vector3.Angle(targetDir, headGaze.direction) < 10f)
        {
            return new Ray(headGaze.origin, targetDir);
        }
        return headGaze;
    }

    public void UpdateGazePath(Ray eyeGaze)
    {
        //Debug.Log("GAZE and HEAD: Select updateGazePath"); //This is throwing the null, when absolutely no mouse movement
        if (gazeIntManager.useAimbot)
        {
            eyeGaze = GazeSelection.AimBot(eyeGaze, gazeIntManager.aimBotTH);
        }
        RaycastHit gazeHit; //or is this the null. no

        if (Physics.Raycast(eyeGaze, out gazeHit, Mathf.Infinity))
        {
            Ray headGaze = new(eyeGaze.origin, Camera.main.transform.forward);
            // headGaze = CorrectHead(headGaze, gazeHit.transform.gameObject);
            /*if (!gazeHit.transform.IsChildOf(GameObject.Find("Room").transform))
            {
                //TODO: we have no room so always falls in here....
                headGaze = CorrectHead(headGaze, gazeHit.point);
            }*/
            
            if (gazeIntManager.drawGaze)//false right now, what is this even doing?
            {
                if (!eyeGazeBeam.enabled) eyeGazeBeam.enabled = true;
                eyeGazeBeam.SetPosition(0, eyeGaze.origin);
                eyeGazeBeam.SetPosition(1, eyeGaze.origin + eyeGaze.direction.normalized * 1.5f);

                if (!headGazeBeam.enabled) headGazeBeam.enabled = true;
                headGazeBeam.SetPosition(0, headGaze.origin);
                headGazeBeam.SetPosition(1, headGaze.origin + headGaze.direction.normalized * 1.5f);
            } else
            {
                //enters here, not above...
                if (eyeGazeBeam.enabled) eyeGazeBeam.enabled = false;
                if (headGazeBeam.enabled) headGazeBeam.enabled = false;
            }
            
            if (fixatedObject_ != gazeHit.transform.gameObject)
            {
                Debug.Log("Doing another thing");
                fixationTime_ = 0.0f;
                if (fixatedObject_ != null) //issue here...
                {
                    Debug.Log("fixated object exists");
                    if (fixatedObject_.GetComponent<TargetScript>() != null)
                    {
                        Debug.Log("Selection Script, HG wants to disable Highlight");
                        fixatedObject_.GetComponent<TargetScript>().DisableHighlight();
                    }
                }
                fixatedObject_ = gazeHit.transform.gameObject;
            }

            if (fixatedObject_.GetComponent<TargetScript>() != null)
            {
                Debug.Log("fixated object exists and has target attached to it");
                if (!fixatedObject_.GetComponent<TargetScript>().IsHighlighted())
                {
                    Debug.Log("Selection Script HG wants to highlight something Magenta");
                    fixatedObject_.GetComponent<TargetScript>().Highlight(false, Color.magenta);
                }
            }

            RaycastHit headHit;
            if (Physics.Raycast(headGaze, out headHit, Mathf.Infinity))
            {
                gazeHitPoint_ = gazeHit.point;
                if (gazeHit.transform.gameObject == headHit.transform.gameObject)
                {
                    
                    if (fixatedObject_.GetComponent <TargetScript>() != null)
                    {
                        if (fixatedObject_.GetComponent<TargetScript>().IsHighlighted())
                        {
                            fixatedObject_.GetComponent<TargetScript>().StartTimer = true;
                        }
                    }

                    if (!fixatedObject_.transform.IsChildOf(GameObject.Find("Room").transform)) fixationTime_ += Time.deltaTime;//TODO:fix line
                    // Debug.Log(fixationTime_ + " " + timeToSelect_);
                    if (fixationTime_ > gazeIntManager.timeToSelect)
                    {
                        // Debug.Log("Select true");
                        select_ = true;
                        fixationTime_ = 0.0f;
                    }
                }
               
            }
        }
        
        
    }

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

    public GameObject GetFixatedObject()
    {
        return fixatedObject_;
    }

    public Vector3 GetHitPoint()
    {
        return gazeHitPoint_;
    }

}
