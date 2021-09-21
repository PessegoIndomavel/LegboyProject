using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class PlayerSounds : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string stepsEventPath;
    [FMODUnity.EventRef]
    public string wallrunStepsEventPath;
    
    FMOD.Studio.EventInstance _stepsInstance;
    FMOD.Studio.EventInstance _wallrunStepsInstance;

    [SerializeField] private float stepsInterval = 0.1f;
    private PlayerMovement mov;
    private bool wallrunning = false;
    private bool wallrunStarted = false;
    private float wallrunPitch = 0f;
    private Coroutine stepsLoopCoroutine;

    private void Start()
    {
        mov = GetComponent<PlayerMovement>();
        
        if(stepsEventPath != null)
            _stepsInstance = RuntimeManager.CreateInstance(stepsEventPath);
        if(wallrunStepsEventPath != null)
            _wallrunStepsInstance = RuntimeManager.CreateInstance(wallrunStepsEventPath);
    }

    private void Update()
    {
        if (wallrunning && (!mov.normalWallrun && !mov.backWallrun))
        {
            ResetWallrunStepSound();
            //stepsLoopCoroutine = StartCoroutine(nameof(StepsSoundLoop));
            wallrunning = false;
            wallrunStarted = false;
        } else if (wallrunning && !wallrunStarted)
        {
            stepsLoopCoroutine = StartCoroutine(nameof(StepsSoundLoop));
            wallrunStarted = true;
        }
        else wallrunning = (mov.normalWallrun || mov.backWallrun);
    }

    //called via animation event
    public void StepSound()
    {
        _stepsInstance.start();
    }

    //called via animation event
    public void WallrunStepSound()
    {
        /*_wallrunStepsInstance = RuntimeManager.CreateInstance(wallrunStepsEventPath);
        _wallrunStepsInstance.setPitch((wallrunPitch*0.1f)+1f);
        //_wallrunStepsInstance.setParameterByName("wallrun certo", wallrunPitch);
        _wallrunStepsInstance.start();*/
        
        /*wallrunPitch++;*/
    }

    private void ResetWallrunStepSound()
    {
        if(stepsLoopCoroutine != null) StopCoroutine(stepsLoopCoroutine);
        wallrunPitch = 0f;
    }

    IEnumerator StepsSoundLoop()
    {
        while (wallrunning)
        {
            print(wallrunPitch);
            _wallrunStepsInstance = RuntimeManager.CreateInstance(wallrunStepsEventPath);
            _wallrunStepsInstance.setPitch((wallrunPitch*0.1f)+1f);
            _wallrunStepsInstance.start();
        
            wallrunPitch++;
            yield return new WaitForSeconds(stepsInterval);
        }
    }
}
