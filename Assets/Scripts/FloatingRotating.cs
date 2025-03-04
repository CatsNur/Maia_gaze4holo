using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingRotating : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float floatSpeed = 1.0f;
    public float floatAmplitude = 0.2f;

    Vector3 startPositon;
    // Start is called before the first frame update
    void Start()
    {
        startPositon = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        float newY = startPositon.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x,newY,transform.position.z);
        
    }
}
