using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;
    public int maxLines = 10;
    private Queue<string> textLines = new Queue<string>();
    // Start is called before the first frame update
    /*void Start()
    {
        
    }*/

    // Replace this with whatever you're tracking
    public void GetLiveText(string incomingStream)
    {
        /*if (targetText != null)
        {
            targetText.text = incomingStream;
        }*/
        // Example: show camera position and time
        //Vector3 camPos = Camera.main.transform.position;
        //return $"Camera Position:\nX: {camPos.x:F2} Y: {camPos.y:F2} Z: {camPos.z:F2}\nTime: {Time.time:F2}s";
        if (targetText != null)
        {
            // Add the new line
            textLines.Enqueue(incomingStream);

            // If too many lines, remove the oldest
            if (textLines.Count > maxLines)
            {
                textLines.Dequeue();
            }

            // Join lines into a single string
            targetText.text = string.Join("\n", textLines);
        }
    }
}
