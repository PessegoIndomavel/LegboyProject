using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private Coroutine shakeTimeCoroutine;

    private float duration, amplitude, frequency;
    private CinemachineBasicMultiChannelPerlin cmPerlin;

    private CinemachineBrain mainCamBrain;
    
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
        ScenesManager.instance.onLoadScene += SetReferences;
    }

    public void ShakeScreen(float duration, float amplitude, float frequency)
    {
        cmPerlin = mainCamBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (cmPerlin == null)
        {
            print("CinemachineBasicMultiChannelPerlin not found!");
            return;
        }

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

        cmPerlin = mainCamBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        if (cmPerlin == null)
        {
            print("CinemachineBasicMultiChannelPerlin not found!");
            return;
        }
        
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

    //called when a new secondary scene is loaded
    private void SetReferences()
    {
        if(ScenesManager.instance.isLevel) mainCamBrain = CinemachineCore.Instance.GetActiveBrain(0);
    }
}
