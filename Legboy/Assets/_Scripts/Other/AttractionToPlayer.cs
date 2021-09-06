using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractionToPlayer : MonoBehaviour
{
    public float attractionSpeed = 5f;
    private Transform playerTransform;
    private Transform myTransform;
    private bool attracting = false;
    private Vector2 originalPos;

    private void Awake()
    {
        originalPos = transform.position;
    }

    private void Update()
    {
        if (attracting)
        {
            if (LifeManager.instance.isDead)
            {
                attracting = false;
                transform.position = originalPos;
                if (GetComponent<Levitation>()) GetComponent<Levitation>().enabled = true;
                return;
            }
            myTransform.position = Vector2.Lerp(myTransform.position, playerTransform.position,
                Time.deltaTime * attractionSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;
        if (myTransform == null) myTransform = transform;
        playerTransform = other.GetComponentInParent<Transform>();
        if (GetComponent<Levitation>()) GetComponent<Levitation>().enabled = false;
        attracting = true;
    }
}
