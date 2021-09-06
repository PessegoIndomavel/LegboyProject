using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public float transitionSpeed  = 1;
    [SerializeField] 
    private CinemachineVirtualCamera vCam;

    public CinemachineBrain mainCam;

    private void Awake()
    {
        vCam.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnableCam();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        DisableCam();
    }

    public void EnableCam()
    {
        mainCam.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseIn;
        mainCam.m_DefaultBlend.m_Time = transitionSpeed;
        vCam.enabled = true;
        CameraZonesManager.instance.curCamZone = this;
    }

    public void EnableCam(float blendTime)
    {
        mainCam.m_DefaultBlend.m_Style = blendTime <= 0f ? CinemachineBlendDefinition.Style.Cut : CinemachineBlendDefinition.Style.EaseIn;
        mainCam.m_DefaultBlend.m_Time = blendTime;
        vCam.enabled = true;
        CameraZonesManager.instance.curCamZone = this;
    }

    public void DisableCam()
    {
        mainCam.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseIn;
        mainCam.m_DefaultBlend.m_Time = transitionSpeed;
        vCam.enabled = false;
        if (CameraZonesManager.instance.curCamZone == this) CameraZonesManager.instance.curCamZone = null;
    }
    
    public void DisableCam(float blendTime)
    {
        mainCam.m_DefaultBlend.m_Style = blendTime <= 0f ? CinemachineBlendDefinition.Style.Cut : CinemachineBlendDefinition.Style.EaseIn;
        mainCam.m_DefaultBlend.m_Time = blendTime;
        vCam.enabled = false;
        if (CameraZonesManager.instance.curCamZone == this) CameraZonesManager.instance.curCamZone = null;
    }
}
