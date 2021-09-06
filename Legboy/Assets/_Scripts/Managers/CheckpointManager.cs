using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Checkpoint[] checkpoints;
    //when starting level, set first checkpoint as current
    private Checkpoint currentCheckpoint;
    
    public static CheckpointManager instance;
    private void Awake()
    {
        #region Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
        #endregion
    }

    private void Start()
    {
        ScenesManager.instance.onLoadScene += SetLevelCheckpoints;
    }

    public void SetLevelCheckpoints()
    {
        if (!ScenesManager.instance.isLevel) return;
        checkpoints = LevelManager.instance.checkpoints; 
        EnableCheckpoint(LevelManager.instance.initialCheckpoint);
    }

    public void EnableCheckpoint(Checkpoint currentCP)
    {
        foreach (var cp in checkpoints)
        {
            cp.DisableCheckpoint();
        }

        currentCheckpoint = currentCP.EnableCheckpoint();
        CameraZonesManager.instance.lastCheckpointCamZone = CameraZonesManager.instance.curCamZone;
    }
    
    public Checkpoint CurrentCheckpoint => currentCheckpoint;
}
