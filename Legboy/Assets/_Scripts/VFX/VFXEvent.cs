using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class VFXEvent : VFXOutputEventAbstractHandler
{
    public override bool canExecuteInEditor { get; }
    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
    {
        print("uiui");
    }
}
