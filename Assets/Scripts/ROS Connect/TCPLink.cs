using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//**using EyeTrackingDemo;

using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine.Windows;

#if !UNITY_EDITOR
    using System.Threading;
    using System.Threading.Tasks;

    using Windows.Networking;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;
#endif

public class TCPLink : MonoBehaviour
{
    //**EyeTracker eyetracker;
    private DataStreamer dataStreamer; 
    public String _input = "Waiting";
    //private String ErrorLog = "E";
    //public GameObject debugLogText;


#if !UNITY_EDITOR
        StreamSocket socket;

        StreamSocketListener listener;
        String port;
        String message;
#endif

    void Start()
    {
        //Debug.Log(Math.Max(0, 4));
        //**eyetracker = GetComponent<EyeTracker>();
        // Initialize DataStreamer to handle incoming data
        dataStreamer = DataStreamer.Instance; //todo figure out how we just recieve the packaged data struct...
#if !UNITY_EDITOR
        listener = new StreamSocketListener();
        port = "9090";
        listener.ConnectionReceived += Listener_ConnectionReceived;
        listener.ConnectionReceived += Listener_Sender;
        listener.Control.KeepAlive = false;

        Listener_Start();

#endif  
    }

#if !UNITY_EDITOR
    private async void Listener_Start()
    {
        Debug.Log("Listener started");
        try
        {
            await listener.BindServiceNameAsync(port);
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }

        Debug.Log("Listening");
    }

    private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
    {
        try
        {
            using (var dr = new DataReader(args.Socket.InputStream))
            {
                dr.InputStreamOptions = InputStreamOptions.Partial;

                while (true) {
                    await dr.LoadAsync(6000);

                    var input = dr.ReadString(dr.UnconsumedBufferLength);
                    _input = input.Substring(Math.Max(0, input.Length - 2));


                }

            }
            
        }
        catch (Exception e)
        {
            Debug.Log("disconnected!!!!!!!! " + e);
            //ErrorLog = e.Message;
        }
    }

    
    private async void Listener_Sender(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
    {
        try
        {
            while (true) {
                using (var dw = new DataWriter(args.Socket.OutputStream))
                {
                
                    dw.WriteString(eyetracker.info);    
                    await dw.StoreAsync(); 
                    dw.DetachStream();
                
                } 
            }
            
        }
        catch (Exception e)
        {
            Debug.Log("disconnected!!!!!!!! " + e);
            //ErrorLog = e.Message;
        }
    }

#endif
    /*
    void Update()
    {
        LOG(_input + ErrorLog);
    }

    void LOG(string msg)
    {
        debugLogText.GetComponent<TextMesh>().text = "\n " + msg;
    }
    */
}
