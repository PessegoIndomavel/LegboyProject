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

    private PlayerMovement mov;
    private bool wallrunning = false;
    private float wallrunPitch = 0f;

    private void Start()
    {
        mov = GetComponent<PlayerMovement>();
        
        _stepsInstance = RuntimeManager.CreateInstance(stepsEventPath);
        _wallrunStepsInstance = RuntimeManager.CreateInstance(wallrunStepsEventPath);
    }

    private void Update()
    {
        if (wallrunning && (!mov.normalWallrun && !mov.backWallrun))
        {
            ResetWallrunStepSound();
            wallrunning = false;
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
        _wallrunStepsInstance = RuntimeManager.CreateInstance(wallrunStepsEventPath);
        _wallrunStepsInstance.setParameterByName("wallrun certo", wallrunPitch);
        _wallrunStepsInstance.start();
        wallrunPitch = (wallrunPitch + 1) % 5;
    }

    private void ResetWallrunStepSound()
    {
        wallrunPitch = 0f;
    }
}
