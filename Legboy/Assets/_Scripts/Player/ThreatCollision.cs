using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreatCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Threat"))
        {
            LifeManager.instance.Die();
        }
    }
}
