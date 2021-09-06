using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Patrol))]
[RequireComponent(typeof(Collider2D))]
public class EnemyLife : MonoBehaviour
{
    public GameObject threatCollider;
    public Animator anim;
    public float disappearTime = 1f;
    
    private Patrol myEnemyPatrol;
    private Collider2D myCol;
    private Coroutine disappearCoroutine;
    
    private void Awake()
    {
        myEnemyPatrol = GetComponent<Patrol>();
        myCol = GetComponent<Collider2D>();
    }
    
    void Die()
    {
        threatCollider.SetActive(false);
        myCol.enabled = false;
        myEnemyPatrol.StopTween();
        myEnemyPatrol.enabled = false;
        anim.SetTrigger("Die");
        disappearCoroutine = StartCoroutine(Disappear());
        LevelManager.instance.AddDefeated(this);
    }
    
    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(disappearTime);
        this.gameObject.SetActive(false);
    }

    public void Respawn()
    {
        if(disappearCoroutine != null) StopCoroutine(disappearCoroutine);
        disappearCoroutine = null;
        
        this.gameObject.SetActive(true);
        threatCollider.SetActive(true);
        myCol.enabled = true;
        myEnemyPatrol.enabled = true;
        myEnemyPatrol.RestartTween();
        anim.Rebind();
        anim.Update(0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerFeet") && !LifeManager.instance.isDead)
        {
            if (!GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().falling) return;
            Die();
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().EnemyHeadJump();
        }
    }
}
