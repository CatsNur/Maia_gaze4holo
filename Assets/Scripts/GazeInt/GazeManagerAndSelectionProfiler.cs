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

    private Camera cam = null;
    private Ray actGazeRay;
    public Ray actGazeRayLocal;

    private Vector3 gazeHitPoint_;
    private float fixationTime_;

    private GameObject fixatedObject_ = null;
    //public GetFixatedObject();

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
        cam = Camera.main;

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
            //Debug.Log("something");
        }
    }
    public Ray GetCombinedGazeRay()
    {
        //Debug.Log(gazeInteractor.rayOriginTransform.position);
        actGazeRay = new Ray(gazeInteractor.rayOriginTransform.position,
                gazeInteractor.rayOriginTransform.forward);
        return actGazeRay;
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
