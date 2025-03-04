using UnityEngine;

namespace maia.Utilities
{
    public static class CameraExtensions
    {
        public static Transform CopyCameraTransForm(this Camera camera)
        {
            var g = new GameObject("Camera Transform Kopie")
            {
                transform =
                {
                    position = camera.transform.position,
                    rotation = camera.transform.rotation,
                    localScale = camera.transform.localScale
                }
            };
            return g.transform;
        }
    }
}
