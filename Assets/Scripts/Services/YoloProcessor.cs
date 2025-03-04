using RealityCollective.ServiceFramework.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Barracuda;
using UnityEngine;

namespace maia.Services
{
    [System.Runtime.InteropServices.Guid("11f556bc-1f7b-4892-b6db-c80e29c5cde9")]
    public class YoloProcessor : BaseServiceWithConstructor, IYoloProcessor
    {
        private readonly YoloProcessorProfile profile;
        private IWorker worker;

        public YoloProcessor(string name, uint priority, YoloProcessorProfile profile)
            : base(name, priority)
        {
            this.profile = profile;
        }

        public override void Initialize()
        {
            //Debug.Log("YOLO processor initalize() from yoloProcessor.cs");
            // Load the YOLOv7 model from the provided NNModel asset
            var model = ModelLoader.Load(profile.Model);

            // Create a Barracuda worker to run the model on the GPU
            worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
        }

        public async Task<List<YoloItem>> RecognizeObjects(Texture2D texture)
        {
            //Debug.Log("RecognizeObjects function called in YoloProcessor.cs");//called constanstly 
            var inputTensor = new Tensor(texture, channels: profile.Channels);
            await Task.Delay(32);
            // Run the model on the input tensor
            var outputTensor = await ForwardAsync(worker, inputTensor);
            inputTensor.Dispose();

            var yoloItems = outputTensor.GetYoloData(profile.ClassTranslator,
                profile.MinimumProbability, profile.OverlapThreshold);
            //Debug.Log($"Yolo Items Got {yoloItems.Capacity}");

            outputTensor.Dispose(); //not being disposed properly? cause of leak?
            return yoloItems;
        }

        // Nicked from https://github.com/Unity-Technologies/barracuda-release/issues/236#issue-1049168663
        public async Task<Tensor> ForwardAsync(IWorker modelWorker, Tensor inputs)
        {
            //Debug.Log("YoloProcessor script, func forwardAsync called");
            var executor = worker.StartManualSchedule(inputs);
            var it = 0;
            bool hasMoreWork;
            do
            {
                hasMoreWork = executor.MoveNext();
                if (++it % 20 == 0)
                {
                    worker.FlushSchedule();
                    await Task.Delay(32);
                }
            } while (hasMoreWork);

            return modelWorker.PeekOutput();
        }

        
        public override void Destroy()
        {
            // Dispose of the Barracuda worker when it is no longer needed
            worker?.Dispose();
        }
    }
}
