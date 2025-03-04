using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManagerScript : MonoBehaviour
{
    //the timer is here now, at least for the laser beam to activate
    public float laserTimeToDestroy; //0.25f
    
    private GameObject[] givenTargets; //similar to "FlyingObjects" from B's code
    //private GameObject[] flyPoints; //not updating target positions, but may want that at some point
    //private GameObject[] usedFlyPoints //^same (not a physical cobect in gametho)

    private bool a_TargetDestroyed;

    [SerializeField] private GameObject laser;

    public bool aTargetDestroyed 
    {
        get { return a_TargetDestroyed; }
        set { a_TargetDestroyed = value; }
    }

    void Awake()
    {
        givenTargets = GameObject.FindGameObjectsWithTag("TargetObject"); // finds all floaty objects in a scene, cuurently 1....
        //flyPoints = GameObject.FindGameObjectsWithTag("FlyPoint");
        //usedFlyPoints = new GameObject[givenTargets.Length];
        a_TargetDestroyed = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Target Manager Script Called Start ()");
        //for B's game, this sets up the flying objects to move to the flypoint positions, then randomly determines one as target

        //nora's decision. create the gaze lazer object here
        GameObject laserObj = Instantiate(laser);
        laserObj.name = "Laser";
        laserObj.transform.parent = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //do something if a target is destroyed
        //--respawn?
        // and for each gameobject that is a target, get the targetobjectscript attached to the target.
    }
}
