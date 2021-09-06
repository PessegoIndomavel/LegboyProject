using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager instance;
    public bool isLevel;
    private string currentSecondaryScene;

    public Action onLoadScene;

    private void Awake()
    {
        #region Singleton
     
         if (instance == null)
             instance = this;
         else if (instance != this)
             Destroy(this);

        #endregion
        
        currentSecondaryScene = SceneManager.GetActiveScene().name;
    }

    private void Start()
    {
        onLoadScene += GameStateManager.instance.EnablePauseControls;
        onLoadScene += GameStateManager.instance.DisablePauseControls;
        onLoadScene += LevelStatsManager.instance.StartRecording;
        onLoadScene += DiamondsManager.instance.ResetDiamonds;
        onLoadScene += LifeManager.instance.ResetDeathCounter;
        onLoadScene += TextBoxManager.instance.ForceHideTextBox;
        onLoadScene += TabletMenuManager.instance.SetCanOpenTabletMenu;
        onLoadScene += TabletMenuManager.instance.ResetTablets;
        onLoadScene += MusicManager.instance.PlayMusic;
        onLoadScene += MusicManager.instance.StopMusic;
        
        LoadSecondaryScene("Main_Menu", false);
    }

    public void LoadSecondaryScene(string sceneName, bool isLevel = true)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, isLevel));
    }

    IEnumerator LoadSceneCoroutine(string sceneName, bool isLevel = true)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //loadOperation.completed += AsyncSceneLoaded;
        currentSecondaryScene = sceneName;
        this.isLevel = isLevel;
        //loadOperation.allowSceneActivation = false;
        while (!loadOperation.isDone)
        {
            //print(Mathf.Clamp01(loadOperation.progress/0.9f));
            yield return null;
        }

        onLoadScene();
        yield return null;
    }

    /*private void AsyncSceneLoaded(AsyncOperation obj)
    {
        onLoadScene();
    }*/

    public void ChangeSecondaryScene(string sceneName, bool isLevel = true)
    {
        SceneManager.UnloadSceneAsync(currentSecondaryScene);

        StartCoroutine(LoadSceneCoroutine(sceneName, isLevel));
    }

    public bool IsLevel => isLevel;
}
