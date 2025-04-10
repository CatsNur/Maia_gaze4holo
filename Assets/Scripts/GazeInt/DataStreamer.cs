using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStreamer : MonoBehaviour
{
    private static DataStreamer _instance;
    private TextUpdater textUpdater;
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
        textUpdater = FindObjectOfType<TextUpdater>();
        if (textUpdater != null)
        {
            textUpdater.GetLiveText("[DataStreamer] " + data.ToString());
        }
        // Later:
        // - Send to ROS publisher
        // - Write to CSV
        // - Emit UnityEvent or notify subscribers
    }
}