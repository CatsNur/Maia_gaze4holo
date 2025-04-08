using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StreamData
{
    public string timestamp;
    public Vector3? localGaze;
    public Vector3? worldGaze;
    public bool? isCorrect;
    public string objectName;

    public StreamData(string timestamp, Vector3? localGaze, Vector3? worldGaze, bool? isCorrect, GameObject lookedAt)
    {
        this.timestamp = timestamp;
        this.localGaze = localGaze;
        this.worldGaze = worldGaze;
        this.isCorrect = isCorrect;
        this.objectName = lookedAt ? lookedAt.name : "None";
    }

    public override string ToString()
    {
        return $"{timestamp}," +
               $"{localGaze?.x.ToString("F2") ?? "None"}," +
               $"{localGaze?.y.ToString("F2") ?? "None"}," +
               $"{localGaze?.z.ToString("F2") ?? "None"}," +
               $"{worldGaze?.x.ToString("F2") ?? "None"}," +
               $"{worldGaze?.y.ToString("F2") ?? "None"}," +
               $"{worldGaze?.z.ToString("F2") ?? "None"}," +
               $"{(isCorrect.HasValue ? isCorrect.ToString() : "None")}," +
               $"{objectName}";
    }
}

public class DataStreamer : MonoBehaviour
{
    private static DataStreamer _instance;
    public static DataStreamer Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("DataStreamer");
                _instance = go.AddComponent<DataStreamer>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void Stream(StreamData data)
    {
        Debug.Log("[DataStreamer] " + data.ToString());
        // Later:
        // - Send to ROS publisher
        // - Write to CSV
        // - Emit UnityEvent or notify subscribers
    }
}