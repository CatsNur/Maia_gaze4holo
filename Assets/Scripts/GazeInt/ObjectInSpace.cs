using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ObjectInSpace : MonoBehaviour
{
    //Similar in idea to the flying objects script, giving the potential for the objects to  move around,
    //this randomly determines who is the target, but all are target 
    [SerializeField]
    private GameObject targets;
    [SerializeField]
    private float travelTime = 1f; //time objects need to read destination if they are moving around

    private float eps = 0.01f;

    private Vector3 pos;
    private Camera cam;
    private bool mainTarget;  // plot twist all of them
    private GameObject target;
    private bool moving;
    private bool targetAlive;

    //private FlyManagerScript flyManager;
    //private Manager manager;

    private Vector3 oldPos;
    private float actTravelTime = 0f;

    public GameObject Target { get { return target; } }

    public bool TargetAlive
    {
        get { return targetAlive; }
        set { targetAlive = value; }
    }

    public bool MainTarget
    {
        get { return mainTarget; }
        set { mainTarget = value; }
    }

    public bool Moving
    {
        get { return moving; }
        set { moving = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        pos = transform.position;
        oldPos = pos;
        moving = false;
        targetAlive = false;
        //flyManager = GameObject.Find("FlyManager").GetComponent<FlyManagerScript>();
        //manager = GameObject.Find("Manager").GetComponent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.forward = -(cam.transform.position - gameObject.transform.position).normalized;

        if (moving)
        {
            // float step = speed * Time.deltaTime; 
            // transform.position = Vector3.MoveTowards(transform.position, pos, step);

            actTravelTime += Time.deltaTime;
            transform.position = oldPos + ((actTravelTime / travelTime) * (pos - oldPos));

            if ((Vector3.Distance(transform.position, pos) < eps) | actTravelTime >= travelTime)
            {
                moving = false;
                transform.position = pos;
                actTravelTime = 0f;
            }
        }

        if (Vector3.Distance(transform.position, pos) < eps & (target == null))
        {
            spawnTarget();
            mainTarget = false;
        }
    }
    public void moveTo(Vector3 newPos)
    {
        actTravelTime = 0f;
        oldPos = transform.position;
        pos = newPos;
        moving = true;
    }
    public void spawnTarget()
    {
        /* TODO: think of it as the pop up over the object
        int rand = Random.Range(0, targets.Length);
        Vector3 targetPos = transform.Find("Target").transform.position;
        target = Instantiate(targets[rand], gameObject.transform) as GameObject;
        target.transform.position = targetPos; // target.transform.position + (target.transform.forward * 0.2f);
        target.transform.forward *= -1;
        target.transform.localScale = Vector3.one * 0.1f;

        if (currentTarget)
        {
            target.GetComponent<TargetScript>().SetMainTarget(true);
            //flyManager.MainTargetDestroyed = false;
        }
        else
        {
            target.GetComponent<TargetScript>().SetMainTarget(false);
        }*/
        Debug.Log("Target spawning");
        Vector3 targetPos = transform.Find("Target").transform.position;
        target = Instantiate(targets,gameObject.transform) as GameObject;
        target.transform.position = targetPos; // target.transform.position + (target.transform.forward * 0.2f);
        target.transform.forward *= -1;
        target.transform.localScale = Vector3.one * 5f; //was 0.1f

        if (mainTarget)
        {
            target.GetComponent<TargetScript>().SetMainTarget(true);
            //flyManager.MainTargetDestroyed = false;
        }
        else
        {
            target.GetComponent<TargetScript>().SetMainTarget(false);
        }

        targetAlive = true;
    }

    public void DestroyTarget()
    {
        if (target != null)
        {
            targetAlive = false;
            target.GetComponent<TargetScript>().UseExplosion = false;
            Destroy(target); //do we though?
        }
    }
}
