using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    //FMOD
    [FMODUnity.EventRef]
    public string soundEventPath;
    FMOD.Studio.EventInstance musicInst;

    public void PlayTheSound()
    {
        musicInst = FMODUnity.RuntimeManager.CreateInstance(soundEventPath);
        musicInst.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(musicInst, transform, GetComponent<Rigidbody>());
       // musicInst.setVolume(PlayerPrefs.GetFloat(PlayerPrefsVariables.sfxVol));
        musicInst.start();
        musicInst.release();
    }

    public void PlayTheSound2D()
    {
        musicInst = FMODUnity.RuntimeManager.CreateInstance(soundEventPath);
        musicInst.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform));
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(musicInst, Camera.main.transform, GetComponent<Rigidbody>());
        musicInst.start();
        musicInst.release();
    }
    
    public void SetSound(string eventPath)
    {
        this.soundEventPath = eventPath;
    }

    public void PlayTheSound(string eventPath, SoundMode mode)
    {
        SetSound(eventPath);
        if(mode == SoundMode.ThreeD) PlayTheSound();
        else PlayTheSound2D();
    }
}

public enum SoundMode{TwoD, ThreeD}