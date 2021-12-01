using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class DiamondPickupVFX : MonoBehaviour
{
    public static DiamondPickupVFX instance;

    private VisualEffect myVFX;

    private void Awake()
    {
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        myVFX = GetComponent<VisualEffect>();
    }

    public void PlayBurst(Vector2 pos)
    {
        transform.position = pos;
        myVFX.Play();
    }
    
    public void OnParticleHit()
    {
        print("uiui");
    }

    /*public override bool canExecuteInEditor { get; }

    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
    {
        Console.WriteLine("uiui");
        myVFX.Play();
    }*/
}
