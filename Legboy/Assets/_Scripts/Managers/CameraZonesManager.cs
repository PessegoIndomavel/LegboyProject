using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraZonesManager : MonoBehaviour
{
    [HideInInspector]
    public CameraZone curCamZone;
    [HideInInspector]
    public CameraZone lastCheckpointCamZone;
    [HideInInspector]
    public CinemachineVirtualCamera defaultVcam;
    
    public Action onCameraChange;

    public static CameraZonesManager instance;
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
        ScenesManager.instance.onLoadScene += FindDefaultVCam;
    }

    public void OnPlayerRespawn()
    {
        if(curCamZone != null) curCamZone.DisableCam(0.01f);
        if (lastCheckpointCamZone != null) lastCheckpointCamZone.EnableCam(0f);

        Camera.main.GetComponent<CinemachineBrain>().m_IgnoreTimeScale = true;
        StartCoroutine(DontIgnoreTimeScale());
    }

    private IEnumerator DontIgnoreTimeScale()
    {
        yield return new WaitForSecondsRealtime(1f);
        Camera.main.GetComponent<CinemachineBrain>().m_IgnoreTimeScale = false;
    }

    public void CameraChanged()
    {
        onCameraChange();
    }

    private void FindDefaultVCam()
    {
        if(ScenesManager.instance.isLevel) defaultVcam = GameObject.Find("Main CM vcam").GetComponent<CinemachineVirtualCamera>();
    }
}
