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
    [FMODUnity.EventRef]
    public string jumpEventPath;
    [FMODUnity.EventRef]
    public string fallEventPath;

    private FMOD.Studio.EventInstance _stepsInstance;
    private FMOD.Studio.EventInstance _wallrunStepsInstance;
    private FMOD.Studio.EventInstance _jumpInstance;
    private FMOD.Studio.EventInstance _fallInstance;

    [SerializeField] private float stepsInterval = 0.1f;
    private PlayerMovement mov;
    private bool wallrunning = false;
    private bool wallrunStarted = false;
    private float wallrunPitch = 0f;
    private Coroutine stepsLoopCoroutine;

    private void Start()
    {
        mov = GetComponentInParent<PlayerMovement>();
        
        if(stepsEventPath != null)
            _stepsInstance = RuntimeManager.CreateInstance(stepsEventPath);
        if(wallrunStepsEventPath != null)
            _wallrunStepsInstance = RuntimeManager.CreateInstance(wallrunStepsEventPath);
        if (wallrunStepsEventPath != null)
            _jumpInstance = RuntimeManager.CreateInstance(jumpEventPath);
        if (fallEventPath != null)
            _fallInstance = RuntimeManager.CreateInstance(fallEventPath);
    }

    private void Update()
    {
        if (wallrunning && (!mov.normalWallrun && !mov.backWallrun))
        {
            ResetWallrunStepSound();
            wallrunning = false;
            wallrunStarted = false;
        } else if (wallrunning && !wallrunStarted)
        {
            stepsLoopCoroutine = StartCoroutine(nameof(StepsSoundLoop));
            wallrunStarted = true;
        }
        else wallrunning = (mov.normalWallrun || mov.backWallrun);
    }

    public void FallSound(bool bigFall)
    {
        _fallInstance.setParameterByName("queda", bigFall ? 0f : 1f);
        _fallInstance.start();
    }

    public void JumpSound()
    {
        _jumpInstance.start();
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
            _wallrunStepsInstance = RuntimeManager.CreateInstance(wallrunStepsEventPath);
            _wallrunStepsInstance.setPitch((wallrunPitch*0.1f)+1f);
            _wallrunStepsInstance.start();
        
            wallrunPitch++;
            yield return new WaitForSeconds(stepsInterval);
        }
    }
}
