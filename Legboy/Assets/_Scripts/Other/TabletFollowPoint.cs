using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TabletFollowPoint : MonoBehaviour
{
    /*[SerializeField] private float speed = 5f;
    [SerializeField] private Transform myTransform;
    [SerializeField] private GameObject myLight;
    [SerializeField] private Transform targetTransform;
    [Tooltip("How close this game object has to be from the movement target to start playing the idle animation.")]
    [SerializeField] private float distToIdle = 1f;

    private SpriteRenderer mySR;
    private Animator myAnim;
    private bool isIdle = true;
    private bool followingPlayer = true;
    private bool followingPoint = false;
    private Vector2 followPos;*/

    public static TabletFollowPoint instance;
    private void Awake()
    {
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion

        /*
        mySR = GetComponent<SpriteRenderer>();
        myAnim = GetComponent<Animator>();
        myAnim.enabled = false;*/
    }

    // Update is called once per frame
    /*void Update()
    {
        if(followingPlayer) FollowPlayer();
        else if(followingPoint) FollowPoint();
    }*/

    /*void FollowPlayer()
    {
        myTransform.position = Vector2.Lerp(myTransform.position.AsVector2(),
            targetTransform.position.AsVector2(), Time.deltaTime * speed);
        if (!isIdle && Vector2.Distance(targetTransform.position.AsVector2(),
            myTransform.position.AsVector2()) <= distToIdle)
        {
            isIdle = true;
        }
        else if (isIdle && Vector2.Distance(targetTransform.position.AsVector2(),
            myTransform.position.AsVector2()) > distToIdle)
        {
            isIdle = false;
        }
    }

    void FollowPoint()
    {
        myTransform.position = Vector2.Lerp(myTransform.position.AsVector2(),
            Vector3Extension.AsVector2(followPos), Time.deltaTime * speed);
        if (Vector2.Distance(transform.position, followPos) <= 0.1f)
        {
            followingPlayer = true;
            SetFalse();
        }
    }*/

    public void FlipPos(int side)
    {
        transform.localPosition = new Vector3(side * Mathf.Abs(transform.localPosition.x), transform.localPosition.y, transform.localPosition.z);
    }

    /*public void SetActive(bool isActive)
    {
        myLight.SetActive(isActive);
        mySR.enabled = isActive;
        myAnim.enabled = isActive;
        if(isActive) myAnim.Play("tablet_closing");
    }
    
    public void SetActive(bool isActive, Vector2 pos)
    {
        if (isActive)
        {
            transform.position = pos;
            followingPlayer = false;
            followingPoint = false;
            myLight.SetActive(true);
            mySR.enabled = true;
            myAnim.enabled = true;
            myAnim.Play("tablet_closing");
        }
        else
        {
            followPos = pos;
            followingPlayer = false;
            followingPoint = true;
        }
    }*/

    /*
    //function called by animation event
    public void StartFollowingPlayer()
    {
        followingPlayer = true;
    }*/

    /*
    private void SetFalse()
    {
        myLight.SetActive(false);
        myAnim.enabled = false;
        mySR.enabled = false;
    }*/
}
