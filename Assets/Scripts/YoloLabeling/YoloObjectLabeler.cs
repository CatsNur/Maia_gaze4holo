using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealityCollective.ServiceFramework.Services;
using UnityEngine;
using maia.Services;
using maia.Utilities;
using System.Runtime.CompilerServices;

namespace maia.YoloLabeling
{
    public class YoloObjectLabeler : MonoBehaviour
    {
        [SerializeField]
        private GameObject labelObject;

        [SerializeField]
        private int cameraFPS = 4;

        [SerializeField]
        private Vector2Int requestedCameraSize = new(896,504);//originally from local joost new(896, 504);

        private Vector2Int actualCameraSize;

        [SerializeField]
        private Vector2Int yoloImageSize = new(320, 256);

        [SerializeField]
        private float virtualProjectionPlaneWidth = 1.356f;

        [SerializeField]
        private float minIdenticalLabelDistance = 0.3f;

        [SerializeField]
        private float labelNotSeenTimeOut = 5f;

        [SerializeField]
        private Renderer debugRenderer;

        private WebCamTexture webCamTexture;

        private IYoloProcessor yoloProcessor;

        private readonly List<YoloGameObject> yoloGameObjects = new();


        private void Start()
        {
            //Debug.Log("Start() yoloObjectLabeler script");
            yoloProcessor = ServiceManager.Instance.GetService<IYoloProcessor>();
            webCamTexture = new WebCamTexture(requestedCameraSize.x, requestedCameraSize.y, cameraFPS);
            webCamTexture.Play();
            StartRecognizingAsync();
            //_ = StartRecognizingAsync();
        }

        private async Task StartRecognizingAsync()
        {
            //Debug.Log("Calling <StartRecognizingAsync> from yoloObjectLabeller");
            await Task.Delay(1000);

            actualCameraSize = new Vector2Int(webCamTexture.width, webCamTexture.height); //(848,480)
            var renderTexture = new RenderTexture(yoloImageSize.x, yoloImageSize.y, 24); 
            if (debugRenderer != null && debugRenderer.gameObject.activeInHierarchy)
            {
                debugRenderer.material.mainTexture = renderTexture;
            }

            while (true)
            {
                var cameraTransform = Camera.main.CopyCameraTransForm();
                Graphics.Blit(webCamTexture, renderTexture);
                await Task.Delay(32);

                var texture = renderTexture.ToTexture2D();
                await Task.Delay(32);

                var foundObjects = await yoloProcessor.RecognizeObjects(texture);
                //Debug.Log($"Yolo Object Labeler script, func start recog async Found {foundObjects.Count} objects");

                ShowRecognitions(foundObjects, cameraTransform);
                Destroy(texture);
                Destroy(cameraTransform.gameObject);
            }
        }


        private void ShowRecognitions(List<YoloItem> recognitions, Transform cameraTransform)
        {
            //Debug.Log($"YoloObjectLabeler Script, function ShowRecognitions called and found {recognitions.Count}");
            foreach (var recognition in recognitions)
            {
                // Issue landed here... 
                var newObj = new YoloGameObject(recognition, cameraTransform,
                    actualCameraSize, yoloImageSize, virtualProjectionPlaneWidth);
                //shitHack for now
                if (newObj.PositionInSpace == null) {
                    newObj.PositionInSpace = Camera.main.transform.position + Camera.main.transform.forward * 2.0f;
                    newObj.Name = "Dumb";
                }
                if (newObj.PositionInSpace != null && !HasBeenSeenBefore(newObj))
                {
                    //currently not hitting the if loop, position in space is null
                    //Debug.Log("Show Recognitions has hit the if loop"); 
                    yoloGameObjects.Add(newObj);
                    newObj.DisplayObject = Instantiate(labelObject,
                        newObj.PositionInSpace.Value, Quaternion.identity);
                    newObj.DisplayObject.transform.parent = transform;
                    var labelController = newObj.DisplayObject.GetComponent<ObjectLabelController>();
                    labelController.SetText(newObj.Name);
                }
            }

            for (var i = yoloGameObjects.Count - 1; i >= 0; i--)
            {
                if (Time.time - yoloGameObjects[i].TimeLastSeen > labelNotSeenTimeOut)
                {
                    Destroy(yoloGameObjects[i].DisplayObject);
                    yoloGameObjects.RemoveAt(i);
                }
            }
        }

        private bool HasBeenSeenBefore(YoloGameObject obj)
        {
            var seenBefore = yoloGameObjects.FirstOrDefault(
                ylo => ylo.Name == obj.Name &&
                Vector3.Distance(obj.PositionInSpace.Value,
                    ylo.PositionInSpace.Value) < minIdenticalLabelDistance);
            if (seenBefore != null)
            {
                seenBefore.TimeLastSeen = Time.time;
            }
            return seenBefore != null;
        }
    }
}
