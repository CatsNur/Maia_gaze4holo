using TMPro;
using UnityEngine;

namespace maia.YoloLabeling
{
    public class ObjectLabelController : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMesh;

        [SerializeField] private GameObject contentParent;

        public void SetText(string text)
        {
            textMesh.text = text;
        }

        private void Start()
        {
            //not landing here yet
            //Debug.Log("start in Objectlabelcontroller called");
            var lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, contentParent.transform.position);
            //Debug.Log($"line renderer position {lineRenderer.GetPosition(0)}");
            lineRenderer.SetPosition(1, transform.position);
        }
    }
}
