using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelected : MonoBehaviour
{
    /*
     * This script attaches to whichever object you want to interact with
     * potential issue, 2 targets could be "selected at the same time"
     */
    private bool targetSelected;


    void Awake()
    { 
        targetSelected = false;
    }

        // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //could change material colors untill selection is complete or show text that says selected
        
    }
}
