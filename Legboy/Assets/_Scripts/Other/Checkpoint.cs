using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Animator myAnim;
    private static readonly int Enabled = Animator.StringToHash("enabled");

    private void OnEnable()
    {
        myAnim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.instance.EnableCheckpoint(this);
            LevelManager.instance.SaveCollectedAndDefeated();
        }
    }

    public Checkpoint EnableCheckpoint()
    {
        myAnim.SetBool(Enabled, true);
        return this;
    }

    public void DisableCheckpoint()
    {
        myAnim.SetBool(Enabled, false);
    }
}
