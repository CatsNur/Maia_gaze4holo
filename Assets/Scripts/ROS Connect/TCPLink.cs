using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
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
    private static TCPLink _instance;
    public static TCPLink Instance => _instance;

#if !UNITY_EDITOR
    private Windows.Storage.Streams.DataWriter writer;
#endif

    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);
    }

    public bool IsConnected
    {
#if !UNITY_EDITOR
        get => writer != null;
#else
        get => false;
#endif
    }

    public void SendMessage(string message)
    {
#if !UNITY_EDITOR
        if (writer != null)
        {
            _ = SendAsync(message); // fire and forget
        }
#endif
    }

#if !UNITY_EDITOR
    private async Task SendAsync(string message)
    {
        try
        {
            writer.WriteString(message);
            await writer.StoreAsync();
            writer.DetachStream(); // optional
        }
        catch (Exception e)
        {
            Debug.Log("[TCPLink] Send error: " + e.Message);
            writer = null;
        }
    }

    private async void Listener_Sender(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
    {
        try
        {
            writer = new Windows.Storage.Streams.DataWriter(args.Socket.OutputStream);
            Debug.Log("[TCPLink] TCP connection ready.");
        }
        catch (Exception e)
        {
            Debug.Log("[TCPLink] Connection failed: " + e.Message);
            writer = null;
        }
    }
#endif
}


/*FROM Gianni
 * using System;
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
    
    //void Update()
    //{
    //    LOG(_input + ErrorLog);
    //}

    //void LOG(string msg)
    //{
    //    debugLogText.GetComponent<TextMesh>().text = "\n " + msg;
    //}
    
}*/
