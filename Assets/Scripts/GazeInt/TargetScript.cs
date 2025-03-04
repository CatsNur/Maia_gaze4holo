using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    //[SerializeField]
    private Material startMaterial;
    [SerializeField] private Material endMaterial;

    private ParticleSystem explosion;

    private TargetManagerScript targetManager;
    private float spawnTime;
    private string targetTextStr;
    private bool isMainTarget;

    private bool useExplosion;
    private float highlightedTime = 0f;

    public static Action mainTargetDestroyed;
    public static Action notMainTargetDestroyed; //don't know how important...

    public bool UseExplosion
    {
        get { return useExplosion; }
        set { useExplosion = value; }
    }
    public bool IsMainTarget { 
        get { return isMainTarget; }
        set { isMainTarget = value; }
    }

    private bool startTimer = false;
    public bool StartTimer
    {
        get { return startTimer; }
        set { startTimer = value; }
    }
    void Awake()
    {
        explosion = GameObject.Find("Explosion").GetComponent<ParticleSystem>();
        //flymanager goes here, when flying
        startMaterial = gameObject.GetComponent<Material>(); //.GetComponent<Renderer>().material.color;
        //Potentially a spawn time

        useExplosion = true;

        //need mrtk happy outline

    
       targetManager = GameObject.Find("TargetManager").GetComponent<TargetManagerScript>(); //for letting know something got destroyed
    }

    public void Highlight()
    {
        // if (startDone) Debug.Log("Used without start done!");
        startTimer = true;
        //outline.enabled = true;
    }

    public void Highlight(bool startTimer_, Color initColor)
    {
        //outline.enabled = true;
        //outline.OutlineColor = initColor;
        startTimer = startTimer_;
    }
    public void DisableHighlight()
    {
        //outline.OutlineColor = Color.green;
        highlightedTime = 0f;
        startTimer = false;
        //outline.enabled = false;
    }
    public bool IsHighlighted()
    {
        /*make mrtk friendlyif (outline != null)
        {
            return outline.enabled;
        }*/
        return false;
    }
    public void SetMainTarget(bool mt) {
        //TODO: make everything come from object labeller script
        Debug.Log("Set Main Target from target called");
        isMainTarget = mt;
        if (mt) {
            targetTextStr = "Interact";
        }
        else
        {
            targetTextStr = "o_o";
        }
    }
    public void WriteTargetText(string text) { 
        targetTextStr = text;
    }
    void Update()
    {
        /*if (currentHealth <= 0)
        {
            Destroy(gameObject);
            return;
        }
        if (currentHealth != oldHealth)
        {
            UpdateColor();
            //this gets called in the Holoet code
            oldHealth = currentHealth;
        }
        if (targetText.text != targetTextStr)
        {
            targetText.text = targetTextStr;
        }*/

        /*if (outline.enabled & startTimer & (highlightedTime < manager.timeToSelect) & (manager.selectOption != SelectionOptions.nod))//* & (manager.selectOption != SelectionOptions.smoothPursuit))
        {
            highlightedTime += Time.deltaTime;
            outline.OutlineColor = Color.red + (1 - (highlightedTime / manager.timeToSelect)) * (Color.green - Color.red);
        }*/

        //Something needs to happen here, change of color...
        //Debug.Log("Target script updating"); is updating a lot. 

    }
    void OnDestroy()
    {
        if (useExplosion)
        {
            explosion.transform.position = transform.position;
            explosion.Play();
            //soundManager.TargetDestroySound(transform.position);
        }

        float lifetime = Time.time - spawnTime;

        /*if (useExplosion)
        {
            if (isMainTarget)
            {
               
                mainTargetDestroyed?.Invoke();
                //*sceneRecorder.AddMsg("MainTargetDestroyed");
            }
            else
            {
                notMainTargetDestroyed?.Invoke();
                
            }
        }

        if (isMainTarget)
        {

            flyManager.MainTargetDestroyed = true;
        }*/
    }
    private void UpdateColor()
    {
        //TODOs Needs to be mrtk friendly, as dwell get longer goes from green to red
        /*Color colorDiff = endColor - startColor;

        gameObject.GetComponent<Renderer>().material.color = startColor + (1 - (currentHealth / startHealth)) * colorDiff;*/
    }
}
