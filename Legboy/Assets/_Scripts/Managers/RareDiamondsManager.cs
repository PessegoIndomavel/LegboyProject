using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RareDiamondsManager : MonoBehaviour
{
    //talvez setar numero maximo de diamantes do level so para estar safe
    public static RareDiamondsManager instance;

    private int rDiamonds = 0;
    
    private void Awake()
    {
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion
    }

    public void CountDiamonds()
    {
        var rDiamondsParent = GameObject.Find("Rare Diamonds").transform;
        print(rDiamondsParent.childCount);
    }

    public void ResetRDiamonds()
    {
        if(ScenesManager.instance.isLevel) rDiamonds = 0;
    }

    public void AddRDiamond()
    {
        rDiamonds++;
        GameplayUIManager.instance.ShowUI();
    }

    public void DeductRDiamond()
    {
        if(rDiamonds>0) rDiamonds--;
    }

    public int GetRDiamonds()
    {
        return rDiamonds;
    }
}
