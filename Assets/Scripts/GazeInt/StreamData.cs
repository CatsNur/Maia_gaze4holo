using UnityEngine;

public struct StreamData
{
    public string timestamp;
    public Vector3? gazePosition;
    public Vector3? gazeDirection;
    public bool? falseSelect;
    public string selectedObject;

    public StreamData(string timestamp, Vector3? gazePosition, Vector3? gazeDirection, bool falseSelect = false, string selectedObject = "")
    {
        this.timestamp = timestamp;
        this.gazePosition = gazePosition; //the direction of the local gaze ray
        this.gazeDirection = gazeDirection;  //the direction of the world gaze ray
        this.falseSelect = falseSelect;
        this.selectedObject = selectedObject;
    }

    public override string ToString()
    {
        return $"{timestamp}," +
               $"{gazePosition?.x.ToString("F2") ?? "None"}," +
               $"{gazePosition?.y.ToString("F2") ?? "None"}," +
               $"{gazePosition?.z.ToString("F2") ?? "None"}," +
               $"{gazeDirection?.x.ToString("F2") ?? "None"}," +
               $"{gazeDirection?.y.ToString("F2") ?? "None"}," +
               $"{gazeDirection?.z.ToString("F2") ?? "None"}," +
               $"{(falseSelect.HasValue ? falseSelect.ToString() : "None")}," +
               $"{(string.IsNullOrEmpty(selectedObject) ? "None" : selectedObject)}";
    }
}