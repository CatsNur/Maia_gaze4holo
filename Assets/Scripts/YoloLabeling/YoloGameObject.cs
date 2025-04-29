using UnityEngine;
using maia.Services;

namespace maia.YoloLabeling
{
    public class YoloGameObject 
    {
        public Vector2 ImagePosition { get; }
        public string Name { get; set; } //TODO: remove the set after debugging
        public GameObject DisplayObject { get; set; }
        public Vector3? PositionInSpace { get; set; }
        public float TimeLastSeen { get; set; }

        private const int MaxLabelDistance = 10;//originally 10 from localJoost, try smaller?
        private const float SphereCastSize = 0.15f;
        private const string SpatialMeshLayerName  = "Spatial Mesh";

        public YoloGameObject(
            YoloItem yoloItem, Transform cameraTransform, 
            Vector2Int cameraSize, Vector2Int yoloImageSize,
            float virtualProjectionPlaneWidth)
        {
            //debug Image positions... -0.5,-0.5... needs to be within -0.5,-0.5 and 0.5,0.5, why does the camerasize somehow change to (960,540)
            ImagePosition = new Vector2(
                (yoloItem.Center.x / yoloImageSize.x * cameraSize.x - cameraSize.x / 2) / cameraSize.x,
                //(yoloItem.Center.x / yoloImageSize.x * 896 - 896 / 2) / 896,
                (yoloItem.Center.y / yoloImageSize.y * cameraSize.y - cameraSize.y / 2) / cameraSize.y);
                //(yoloItem.Center.y / yoloImageSize.y * 504 - 504 / 2) / 504);
            Name = yoloItem.MostLikelyObject;

            var virtualProjectionPlaneHeight = virtualProjectionPlaneWidth * cameraSize.y / cameraSize.x;
            //var virtualProjectionPlaneHeight = virtualProjectionPlaneWidth * 504 / 896;
            FindPositionInSpace(cameraTransform, virtualProjectionPlaneWidth, virtualProjectionPlaneHeight);
            TimeLastSeen = Time.time;
        }

        private void FindPositionInSpace(Transform transform,
            float width, float height)
        {
            //Debug.Log("Finding Position in space");
            var positionOnPlane = transform.position + transform.forward + 
                transform.right * (ImagePosition.x * width) - 
                transform.up * (ImagePosition.y * height);
            PositionInSpace = CastOnSpatialMap(positionOnPlane, transform);
        }

        private Vector3? CastOnSpatialMap(Vector3 positionOnPlane, Transform transform)
        {
            //Debug.Log("YoloGameObject script, func CastOnSpatialMap");
            //Debug.Log($"SpatialMeshLayerName: {SpatialMeshLayerName}"); // Should print 30 
            //Debug.Log($"Layer index for 'Spatial Mesh': {LayerMask.NameToLayer(SpatialMeshLayerName)}"); 
            //Debug.Log($"LayerMask value: {LayerMask.GetMask(SpatialMeshLayerName)}");

            int spatialMeshLayer = LayerMask.NameToLayer(SpatialMeshLayerName);
            /*if (spatialMeshLayer != -1) {
                Debug.Log("Manual bitflip");
                int layerMask = 1 << spatialMeshLayer;
            }*/
            int layerMask = LayerMask.GetMask(SpatialMeshLayerName);
            layerMask = 1 << spatialMeshLayer;

            //TODO: need to find a better position to create this object, since it's getting created everyframe
            /*Vector3 direction = (positionOnPlane - transform.position);
            Vector3 expectedHitPoint = transform.position + (direction * 2);
            GameObject expectedMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            expectedMarker.transform.position = expectedHitPoint;
            expectedMarker.transform.localScale = Vector3.one * 0.5f;
            expectedMarker.AddComponent<BoxCollider>();
            expectedMarker.layer = LayerMask.NameToLayer(SpatialMeshLayerName);
            expectedMarker.GetComponent<Renderer>().material.color = Color.clear;*/

            if (Physics.SphereCast(transform.position, SphereCastSize,
                    (positionOnPlane - transform.position),
                    out var hitInfo, MaxLabelDistance, LayerMask.GetMask(SpatialMeshLayerName))) //not hitting anything
            {
                Debug.Log("HIT");
                return hitInfo.point;
            }
            return null;
        }

    }
}
