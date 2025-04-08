using UnityEngine;

public struct StreamData
{
    public string timestamp;
    public Vector3? localGaze;
    public Vector3? worldGaze;
    public bool? isSelected;
    public string objectName;

    public StreamData(string timestamp, Vector3? localGaze, Vector3? worldGaze, bool? isSelected, GameObject lookedAt)
    {
        this.timestamp = timestamp;
        this.localGaze = localGaze;
        this.worldGaze = worldGaze;
        this.isSelected = isSelected;
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
               $"{(isSelected.HasValue ? isSelected.ToString() : "None")}," +
               $"{objectName}";
    }
}