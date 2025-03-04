using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

namespace maia.Services
{
    public class YoloItem
    {
        public Vector2 Center { get; }

        public Vector2 Size { get; }

        public Vector2 TopLeft { get; }

        public Vector2 BottomRight { get; }

        public float Confidence { get; }

        public string MostLikelyObject { get; }

        public YoloItem(Tensor tensorData, int boxIndex, IYoloClassTranslator translator)
        {
            Center = new Vector2(tensorData[0, 0, 0, boxIndex], tensorData[0, 0, 1, boxIndex]);
            Size = new Vector2(tensorData[0, 0, 2, boxIndex], tensorData[0, 0, 3, boxIndex]);
            TopLeft = Center - Size / 2;
            BottomRight = Center + Size / 2;
            Confidence = tensorData[0, 0, 4, boxIndex];

            var classProbabilities = new List<float>();
            for (var i = 5; i < tensorData.width; i++)
            {
                classProbabilities.Add(tensorData[0, 0, i, boxIndex]);
            }
            var maxIndex = classProbabilities.IndexOf(classProbabilities.Max());
            MostLikelyObject = translator.GetName(maxIndex);
        }
        //Dummy YoloItem
        public YoloItem(string mostLikelyObject,Vector2 topLeft, Vector2 bottomRight, float confidence) {
            MostLikelyObject = mostLikelyObject;
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Confidence = confidence;

        }
    }
}
