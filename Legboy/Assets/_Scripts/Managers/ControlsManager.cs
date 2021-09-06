using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    public PlayerControls controlInput;
    
    public static ControlsManager instance;
    private void Awake()
    {
        controlInput = new PlayerControls();
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion
    }

    private void OnEnable()
    {
        controlInput.Enable();
    }

    private void OnDisable()
    {
        controlInput.Disable();
    }
}
