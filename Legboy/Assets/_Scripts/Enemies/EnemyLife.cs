using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Patrol))]
public class EnemyLife : MonoBehaviour
{
    public Animator anim;
    public float disappearTime = 1f;
    public float playerColRadius = 2f;
    public float playerColHeight = 0.5f;
    public LayerMask playerFeetLayer;
    
    private Patrol myEnemyPatrol;
    private bool dead;
    private Coroutine disappearCoroutine;
    
    private void Awake()
    {
        myEnemyPatrol = GetComponent<Patrol>();
    }

    private void Update()
    {
        if (dead) return;
        CheckForPlayerCollision();
    }

    void Die()
    {
        dead = true;
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
        dead = false;
        myEnemyPatrol.enabled = true;
        myEnemyPatrol.RestartTween();
        anim.Rebind();
        anim.Update(0f);
    }

    private void CheckForPlayerCollision()
    {
        var hit = Physics2D.OverlapCircle(transform.position, playerColRadius, playerFeetLayer);
        if (!hit || LifeManager.instance.isDead) return;
        if (transform.position.y + playerColHeight < hit.bounds.center.y - hit.bounds.extents.y)
        {
            LevelManager.instance.player.GetComponent<PlayerMovement>().EnemyHeadJump();
            Die();
        }
        else
        {
            LifeManager.instance.Die();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, playerColRadius);
        Gizmos.DrawLine(new Vector3(-0.5f, playerColHeight, 0f) + transform.position, new Vector3(0.5f, playerColHeight, 0f) + transform.position);
    }
}
