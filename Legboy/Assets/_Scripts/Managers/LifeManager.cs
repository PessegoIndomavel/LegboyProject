using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    private Transform playerTransform;
    private PlayerMovement playerMov;
    private PlayerAnimation playerAnim;

    private bool dead = false;
    private int deathCounter;
    
    public static LifeManager instance;
    private void Awake()
    {
        #region Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
        #endregion

        ScenesManager.instance.onLoadScene += SetPlayer;
    }

    public void SetPlayer()
    {
        if (!ScenesManager.instance.isLevel) return;
        StartCoroutine(SetPlayerWhenPossible());
    }

    IEnumerator SetPlayerWhenPossible()
    {
        while (!LevelManager.instance) yield return null;
        playerTransform = LevelManager.instance.player.transform;
        playerMov = playerTransform.GetComponent<PlayerMovement>();
        playerAnim = playerTransform.GetComponent<PlayerAnimation>();
    }

    public void Die()
    {
        deathCounter++;
        playerMov.StopAllDistanceParticles();
        playerMov.dyingParticles.Play();
        dead = true;
        playerTransform.GetComponent<Animator>().SetTrigger("died");
        playerMov.DisableControls();
        GameStateManager.instance.DisablePauseControls();
        ScreenTransitionManager.instance.StartTransition(ReturnToCheckpoint, Respawn);
        GameStateManager.instance.PauseTime();
    }

    //private float tempVCamBlendTime;
    public void ReturnToCheckpoint()
    {
        playerTransform.position = CheckpointManager.instance.CurrentCheckpoint.transform.position + new Vector3(0f, -0.23f, 0f);
        playerTransform.GetComponent<Animator>().Play("idleLegboy");
        playerTransform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerAnim.Flip(playerMov.side);
        LevelManager.instance.RespawnCollectedAndDefeated();
        CameraZonesManager.instance.OnPlayerRespawn();
        //StartCoroutine(ReturnToCPCoroutine());
    }
    
    //chegar no checkpointmanager e catar a camerazone atual, ai quando chama returntocheckpoint, ativa a camerazone (ou desativa a anterior)

    /*IEnumerator ReturnToCPCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        var vCamBrain = Camera.main.GetComponent<CinemachineBrain>();
        if (vCamBrain != null)
        {
            tempVCamBlendTime = vCamBrain.m_DefaultBlend.m_Time;
            vCamBrain.m_DefaultBlend.m_Time = 0.01f;
            vCamBrain.m_IgnoreTimeScale = true;
        }
    }*/

    public void Respawn()
    {
        dead = false;
        playerMov.EnableControls();
        GameStateManager.instance.ResumeTime();
        /*var vCamBrain = Camera.main.GetComponent<CinemachineBrain>();
        if (vCamBrain != null)
        {
          vCamBrain.m_IgnoreTimeScale = false;
          vCamBrain.m_DefaultBlend.m_Time = tempVCamBlendTime;  
        }*/

        GameStateManager.instance.EnablePauseControls();
    }

    public void ResetDeathCounter()
    { 
        if(ScenesManager.instance.isLevel) deathCounter = 0;
    }

    public int getDeathCounter => deathCounter;

    public bool isDead => dead;
}
