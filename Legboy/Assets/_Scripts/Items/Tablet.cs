using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablet : Collectable
{
    public int index;

    /*[SerializeField] private Transform debugCircle;*/
    [SerializeField] private float speed = 5f;
    [SerializeField] private float circularSpeed = 5f;
    
    /*[Tooltip("How close this game object has to be from the movement target to start playing the idle animation.")]
    [SerializeField] private float distToIdle = 1f;*/

    private Rigidbody2D myRb;
    private Animator myAnim;
    //private bool isIdle = true;
    private bool followingPlayer;
    private bool followingPoint;
    private Vector2 followPos;
    private Transform myTransform;
    private Transform tabletFollowPointTransform;
    
    private void Start()
    {
        myAnim = GetComponent<Animator>();
        myTransform = transform.parent;
        myRb = myTransform.GetComponent<Rigidbody2D>();
        tabletFollowPointTransform = TabletFollowPoint.instance.transform;
    }
    
    void Update()
    {
        if(followingPlayer) FollowPlayer();
        else if(followingPoint) FollowPoint();
    }

    protected override void Collect()
    {
        if (followingPlayer || followingPoint) return;
        
        myAnim.Play("tablet_closing");

        LevelManager.instance.AddTabletCollected(this);
    }

    public void SaveOnCheckpoint()
    {
        followPos = CheckpointManager.instance.CurrentCheckpoint.transform.position;
        followingPlayer = false;
        followingPoint = true;
        
        TabletMenuManager.instance.UnlockTablet(index);
        //play orb animation here
    }

    public override void Respawn()
    {
        myTransform.gameObject.SetActive(false);
        
        GetComponent<Levitation>().ResetToStartPos();
        
        followingPlayer = false;
        followingPoint = false;
        
        myAnim.Play("tablet_idle");
    }

    private void FollowPlayer()
    {
        Vector2 direction = tabletFollowPointTransform.position - myTransform.position;
        myRb.velocity = speed * direction + Vector2.Perpendicular(direction) * circularSpeed;
    }

    private void FollowPoint()
    {
        myRb.velocity = speed * (followPos - myTransform.position.AsVector2());
        
        //got to the checkpoint
        if (!(Vector2.Distance(transform.position, followPos) <= 0.5f)) return;
        followingPoint = false;
        myTransform.gameObject.SetActive(false);
    }
    
    //function called by animation event
    public void StartFollowingPlayer()
    {
        followingPlayer = true;
    }
}
