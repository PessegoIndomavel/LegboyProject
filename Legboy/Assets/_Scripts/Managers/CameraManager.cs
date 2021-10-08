using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public CinemachineTargetGroup camTargetGroup;
    private int camZoneCounter;
    
    private void Awake()
    {
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion
        camTargetGroup.AddMember(null, 1, 0);
    }
    
    public void IncreaseCamZoneCounter()
    {
        camZoneCounter++;
    }

    public void DecreaseCamZoneCounter()
    {
        if (camZoneCounter > 0) camZoneCounter--;
        if(camZoneCounter==0) ReturnCamToPlayerOnly();
    }

    private void ReturnCamToPlayerOnly()
    {
        instance.camTargetGroup.m_Targets[1].target = null;
    }
}
