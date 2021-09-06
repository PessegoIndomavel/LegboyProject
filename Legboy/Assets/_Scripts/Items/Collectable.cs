using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectable : MonoBehaviour
{
    protected virtual void Collect()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void Respawn()
    {
        this.gameObject.SetActive(true);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")) Collect();
    }
}
