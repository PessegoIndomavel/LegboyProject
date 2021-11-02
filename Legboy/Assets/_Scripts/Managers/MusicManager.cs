using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class MusicManager : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string musicEventPath;
    
    FMOD.Studio.EventInstance _musicInstance;

    public static MusicManager instance;
    private void Awake()
    {
        #region Singleton
     
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion
    }
    
    void Start()
    {
        GameStateManager.instance.onPause += PauseMusic;
        GameStateManager.instance.onUnpause += UnpauseMusic;
    }


    public void PlayMusic()
    {
     // if (!ScenesManager.instance.isLevel) return;
     // _musicInstance = RuntimeManager.CreateInstance(musicEventPath);
     // _musicInstance.start();
    }

    public void StopMusic()
    {
        if (ScenesManager.instance.isLevel) return;
        _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void PauseMusic()
    {
        _musicInstance.setPaused(true);
    }

    private void UnpauseMusic()
    {
        _musicInstance.setPaused(false);
    }
}
