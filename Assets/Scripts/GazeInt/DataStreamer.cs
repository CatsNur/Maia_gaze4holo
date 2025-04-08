using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStreamer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // initalize something, like a file or ros package, or both?
        /*
         * string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
         filePath = Path.Combine(Application.persistentDataPath, $"gazeData_{timeStamp}.csv");
         // Optional: Write header line
         File.WriteAllText(filePath, "Time,X,Y,Z\n");
        */
    }

    void Update()
    {
        // Collect some data per frame
        Vector3 data = transform.position;
        string time = DateTime.Now.ToString("HH:mm:ss.fff"); //currently not safe if other parts of the code buffer or lag

        // Send data to self
        ReceiveData(data, time);
    }

    void ReceiveData(Vector3 position, string timestamp)
    {
        string line = $"{timestamp},{position.x},{position.y},{position.z}";

        // Log to console (optional)
        Debug.Log("Writing: " + line);

        // do something else? like Append to file or send as ros package
        //File.AppendAllText(filePath, line + "\n");
    }
}
