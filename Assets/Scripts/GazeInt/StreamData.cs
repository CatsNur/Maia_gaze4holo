using UnityEngine;

public struct StreamData
{
    public string timestamp;
    public Vector3? localGaze;
    public Vector3? worldGaze;
    public bool? falseSelect;
    public string selectedObject;

    public StreamData(string timestamp, Vector3? localGaze, Vector3? worldGaze, bool falseSelect = false, string selectedObject = "")
    {
        this.timestamp = timestamp;
        this.localGaze = localGaze;
        this.worldGaze = worldGaze;
        this.falseSelect = falseSelect;
        this.selectedObject = selectedObject;
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
               $"{(falseSelect.HasValue ? falseSelect.ToString() : "None")}," +
               $"{(string.IsNullOrEmpty(selectedObject) ? "None" : selectedObject)}";
    }
}