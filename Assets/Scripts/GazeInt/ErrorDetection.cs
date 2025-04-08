using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
//using Emgu.CV.CvEnum;

public class ErrorDetection : MonoBehaviour
{
    // Start is called before the first frame update



    [SerializeField] private NNModel modelSourceGaze;
    [SerializeField] private float thGaze;
    [SerializeField] private NNModel modelSourceHeadAndGaze;
    [SerializeField] private float thHeadAndGaze;
    [SerializeField] private NNModel modelSourceNod;
    [SerializeField] private float thNod;
    public int seqLength;
    public float decisionTime;
    public bool test = false;


    //private Manager manager;
    //private SelectionOptions selectOption = SelectionOptions.headAndGaze;
    private float th;
    private Model model;
    private IWorker worker;
    private float mse;

    public float Threshold { get { return th; } }
    
    void Start()
    {
        Debug.Log("Error detection started");
        PrepareWorker();
        //model = ModelLoader.Load(modelSourceGaze);
        //th = thGaze;
        //worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
    }

    private void PrepareWorker()
    {
        /*switch (selectOption)
        {
            case SelectionOptions.gaze:
                model = ModelLoader.Load(modelSourceGaze);
                th = thGaze;
                Debug.Log("Load gaze model.");
                break;
            case SelectionOptions.headAndGaze:
                model = ModelLoader.Load(modelSourceHeadAndGaze);
                th = thHeadAndGaze;
                Debug.Log("Load head and gaze model.");
                break;
            case SelectionOptions.nod:
                model = ModelLoader.Load(modelSourceGaze);
                th = thNod;
                Debug.Log("Load nod model.");
                break;
            default:
                break;
        }*/
        model = ModelLoader.Load(modelSourceHeadAndGaze);
        th = thHeadAndGaze;
        if (model == null)
        {
            Debug.LogError("Model is NULL! Check if it's assigned properly.");
        }

        Debug.Log("Load head and gaze model.");
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
        if (worker == null)
        {
            Debug.LogError("Worker creation failed.");
            return;  // Don't proceed if worker creation failed
        }

        Debug.Log("Worker created successfully.");

    }


    public bool CheckError(List<float> angles)//not even getting here currently
    {
        if (worker == null)
        {
            Debug.LogError("Worker is not initialized! Can't check error. Returning false.");
            return false;  // Return early if worker is not initialized
        }
        Debug.Log("Checking error");

        float[] anglesArray = angles.ToArray();
        int[] shape = new int[] { 1, 1, anglesArray.Length };
        Tensor input = new Tensor(shape, anglesArray);
        //Debug.Log("Input: " + input.ToString());

        worker.Execute(input);

        Tensor output = worker.PeekOutput();
        //Debug.Log("Output: " + output.ToString());
        float[] outArray = output.ToReadOnlyArray();

        mse = 0;

        for (int i = 0; i < anglesArray.Length; i++)
        {
            mse += (anglesArray[i] - outArray[i]) * (anglesArray[i] - outArray[i]);
        }
        mse /= anglesArray.Length;
        
        //Debug.Log("Length angles: " + anglesArray.Length.ToString() + "; Length outArray: " + outArray.Length.ToString() + "; MSE: " + mse.ToString());

        bool error = false;
        if (mse > th)
        {
            error = true;
        } 
        input.Dispose();
        output.Dispose();
        
        return error;
    }

    public float GetLastMSE()
    {
        return mse;
    }

    // Update is called once per frame
    void Update()
    {
        //not important, fixed code.. i think
        /*if (!test)
        {
            if ((selectOption != manager.selectOption))
            {
                selectOption = manager.selectOption;
                worker.Dispose();
                PrepareWorker();
            }
        }*/
        //worker.Dispose();
        //PrepareWorker();

        /*if (test && Input.GetKeyDown(KeyCode.M)) 
        { 
            TestCheckError(); 
        }*/
    }

    private void TestCheckError()
    {
        List<float> angles = new List<float>();
        for (int i = 0; i < seqLength; i++)
        {
            angles.Add(Random.Range(0f, 10f));
        }
        bool te = CheckError(angles);

        Debug.Log("Checked CheckError. Result is " + te.ToString());
    }

    void OnDestroy()
    {
        Debug.Log("Model Dies");
        worker.Dispose();
    }
}
