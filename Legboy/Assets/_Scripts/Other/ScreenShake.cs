using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private Coroutine shakeTimeCoroutine;

    private float duration, amplitude, frequency;
    private CinemachineBasicMultiChannelPerlin cmPerlin;
    
    public static ScreenShake instance;
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
        CameraZonesManager.instance.onCameraChange += OnCameraChange;
    }

    public void ShakeScreen(float duration, float amplitude, float frequency)
    {
        cmPerlin = !CameraZonesManager.instance.curCamZone ? 
            CameraZonesManager.instance.defaultVcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>() :
            CameraZonesManager.instance.curCamZone.vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        this.duration = duration;
        this.amplitude = amplitude;
        this.frequency = frequency;
        
        cmPerlin.m_AmplitudeGain = amplitude;
        cmPerlin.m_FrequencyGain = frequency;
        
        if (shakeTimeCoroutine != null) StopCoroutine(shakeTimeCoroutine);
        shakeTimeCoroutine = StartCoroutine(ShakeTime(duration));
        
    }

    private IEnumerator ShakeTime(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        StopShake();
    }

    private void StopShake()
    {
        amplitude = 0f;
        frequency = 0f;

        cmPerlin = !CameraZonesManager.instance.curCamZone ? 
            CameraZonesManager.instance.defaultVcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>() :
            CameraZonesManager.instance.curCamZone.vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        cmPerlin.m_AmplitudeGain = 0f;
        cmPerlin.m_FrequencyGain = 0f;
    }

    private void OnCameraChange()
    {
        cmPerlin = !CameraZonesManager.instance.curCamZone ? 
            CameraZonesManager.instance.defaultVcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>() :
            CameraZonesManager.instance.curCamZone.vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        cmPerlin.m_AmplitudeGain = amplitude;
        cmPerlin.m_FrequencyGain = frequency;
    }
}
