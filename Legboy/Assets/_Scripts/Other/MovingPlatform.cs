using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Patrol))]
public class MovingPlatform : MonoBehaviour
{
    private PlayerMovement playerMov;
    private PlayerCollision playerCol;

    private bool playerAttached;
    private bool playerColliding;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            if (!playerMov)
            {
                playerMov = other.gameObject.GetComponent<PlayerMovement>();
                playerCol = other.gameObject.GetComponent<PlayerCollision>();
            }

            playerColliding = true;
            AttachPlayer();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            DetachPlayer();
            playerColliding = false;
        }
    }

    void AttachPlayer()
    {
        playerMov.transform.SetParent(transform);
        playerAttached = true;
    }

    void DetachPlayer()
    {
        playerMov.transform.SetParent(null);
        playerAttached = false;
    }

    private void Update()
    {
        if (playerAttached)
        {
            if (playerCol.onGround)//checks if player is actually on top of this platform
            {
                //if none of the three ground detection raycasts detects this platform -> detach
                if((Physics2D.Raycast((Vector2) playerCol.transform.position + playerCol.groundDetectOffsetLeft,
                    Vector2.down, playerCol.groundDetectDistance, playerCol.groundLayer).transform != transform) && 
                   
                   (Physics2D.Raycast((Vector2) playerCol.transform.position + playerCol.groundDetectOffsetRight,
                         Vector2.down, playerCol.groundDetectDistance, playerCol.groundLayer).transform != transform) &&
                    
                   (Physics2D.Raycast((Vector2) playerCol.transform.position + new Vector2(0f, playerCol.groundDetectOffsetLeft.y),
                       Vector2.down, playerCol.groundDetectDistance, playerCol.groundLayer).transform != transform))
                    
                    DetachPlayer();
            }
        }else if (playerColliding)
        {
            if((Physics2D.Raycast((Vector2) playerCol.transform.position + playerCol.groundDetectOffsetLeft,
                   Vector2.down, playerCol.groundDetectDistance, playerCol.groundLayer).transform == transform) || 
                   
               (Physics2D.Raycast((Vector2) playerCol.transform.position + playerCol.groundDetectOffsetRight,
                   Vector2.down, playerCol.groundDetectDistance, playerCol.groundLayer).transform == transform) ||
                    
               (Physics2D.Raycast((Vector2) playerCol.transform.position + new Vector2(0f, playerCol.groundDetectOffsetLeft.y),
                   Vector2.down, playerCol.groundDetectDistance, playerCol.groundLayer).transform == transform))
                
                AttachPlayer();
        }
        
    }
}
