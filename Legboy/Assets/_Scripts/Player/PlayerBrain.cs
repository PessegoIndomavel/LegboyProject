using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    #region Components

    private PlayerCollision coll;
    private Rigidbody2D rb;
    private PlayerAnimation anim;
    private PlayerJump jump;
    private PlayerSounds sound;
    private Collider2D myColl;

    #endregion
    
    public Action onEnableControls, onDisableControls, onDie;

    public GameObject staminaBar;
    
    private void Awake()
    {
        coll = GetComponent<PlayerCollision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<PlayerAnimation>();
        sound = GetComponentInChildren<PlayerSounds>();
        jump = GetComponent<PlayerJump>();
        myColl = GetComponent<Collider2D>();
    }

    public void Die()
    {
        onDie();
        DisableControls();
    }

    public void DisableControls()
    {
        onDisableControls();
    }

    public void EnableControls()
    {
        onEnableControls();
    }
}

    
