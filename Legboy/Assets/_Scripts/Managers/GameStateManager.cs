using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
    private GameState currentGameState;
    
    public static GameStateManager instance;
    public float pauseTransitionDuration = 1f;
    [Tooltip("Pause transition animation curve.")]
    public Ease pauseTransitionEase = Ease.Linear;

    public GameObject pauseScreen;
    public Action onPause;
    public Action onUnpause;
    
    private Tween transTween;
    private float prevTimeScale = 1f;

    private bool canPause = true;

    private void Awake()
    {
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion

        currentGameState = GameState.Resumed;
    }

    private void Start()
    {
        ControlsManager.instance.controlInput.Legboy.Pause.performed += TogglePauseWithScreen;
    }

    public void StopTimeWithTransition(TweenCallback CallbackFunc)
    {
        float temp = Time.timeScale;
        transTween.Kill();
        transTween = DOTween.To(value => 
                Time.timeScale = value, temp, 0f, pauseTransitionDuration).
            SetEase(pauseTransitionEase).SetUpdate(true).OnComplete(CallbackFunc);
    }
    
    public void ResumeTimeWithTransition(TweenCallback CallbackFunc)
    {
        float temp = Time.timeScale;
        transTween.Kill();
        transTween = DOTween.To(value => 
                Time.timeScale = value, temp, 1f, pauseTransitionDuration).
            SetEase(pauseTransitionEase).SetUpdate(true).OnComplete(CallbackFunc);
    }

    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    public void PauseTime()
    {
        Time.timeScale = 0f;
    }

    public void PauseWithScreen()
    {
        if (!canPause) return;
        Pause();
        pauseScreen.SetActive(true);
    }

    public void ResumeWithScreen()
    {
        Resume();
        pauseScreen.SetActive(false);
    }

    //called via input system
    public void TogglePauseWithScreen(InputAction.CallbackContext callbackContext)
    {
        if(isPaused()) ResumeWithScreen();
        else PauseWithScreen();
    }

    public void EnablePauseControls()
    {
        if (ScenesManager.instance.isLevel) ControlsManager.instance.controlInput.Legboy.Pause.Enable();
    }

    public void DisablePauseControls()
    {
        if (!ScenesManager.instance.isLevel) ControlsManager.instance.controlInput.Legboy.Pause.Disable();
    }

    public void Pause()
    {
        if (currentGameState == GameState.Resumed)
        {
            if (transTween != null && transTween.active) transTween.Pause();
            
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            currentGameState = GameState.Paused;
            onPause();
        }
    }

    public void Resume()
    {
        if (currentGameState == GameState.Paused)
        {
            if (transTween != null && transTween.active) transTween.Play();
            
            Time.timeScale = prevTimeScale;
            currentGameState = GameState.Resumed;
            onUnpause();
        }
    }

    public GameState getGameState()
    {
        return currentGameState;
    }

    public bool isPaused()
    {
        return currentGameState == GameState.Paused;
    }
}

public enum GameState {
    Resumed,
    Paused
}
