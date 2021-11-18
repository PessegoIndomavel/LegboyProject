using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiamondsManager : MonoBehaviour
{
    //talvez setar numero maximo de diamantes do level so para estar safe
    public static DiamondsManager instance;

    private int diamonds = 0;
    
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
        var diamondsParent = GameObject.Find("Diamonds").transform;
        print(diamondsParent.childCount);
    }

    public void ResetDiamonds()
    {
        if(ScenesManager.instance.isLevel) diamonds = 0;
    }

    public void AddDiamond()
    {
        diamonds++;
        GameplayUIManager.instance.ShowUI();
    }

    public void DeductDiamond()
    {
        if(diamonds>0) diamonds--;
    }

    public int GetDiamonds()
    {
        return diamonds;
    }
}
