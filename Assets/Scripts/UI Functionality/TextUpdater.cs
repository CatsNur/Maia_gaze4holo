using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;
    // Start is called before the first frame update
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        if (targetText != null)
        {
            targetText.text = GetLiveText();
        }
    }

    // Replace this with whatever you're tracking
    private string GetLiveText()
    {
        // Example: show camera position and time
        Vector3 camPos = Camera.main.transform.position;
        return $"Camera Position:\nX: {camPos.x:F2} Y: {camPos.y:F2} Z: {camPos.z:F2}\nTime: {Time.time:F2}s";
    }
}
