using UnityEngine;

public struct StreamData
{
    public string timestamp;
    public Vector3? gazePosition;
    public Vector3? gazeDirection;
    public bool? falseSelect;
    public string selectedObject;
    public Vector3? objectPosition;

    public StreamData(string timestamp, Vector3? gazePosition, Vector3? gazeDirection, bool falseSelect = false, string selectedObject = "", Vector3? objectPosition = null)
    {
        this.timestamp = timestamp;
        this.gazePosition = gazePosition; //the direction of the local gaze ray
        this.gazeDirection = gazeDirection;  //the direction of the world gaze ray
        this.falseSelect = falseSelect;
        this.selectedObject = selectedObject; // the name of the object that was selected by gaze
        //need coordinates of object here
        this.objectPosition = objectPosition;
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
               $"{(string.IsNullOrEmpty(selectedObject) ? "None" : selectedObject)}" + //;
               $"{objectPosition?.x.ToString("F2") ?? "None"}," +
               $"{objectPosition?.y.ToString("F2") ?? "None"}," +
               $"{objectPosition?.z.ToString("F2") ?? "None"}";
    }
}