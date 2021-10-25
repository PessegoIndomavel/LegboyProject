using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TransparentWall : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Renderer rend;
    [SerializeField] private float transitionSpeed = 5f;
    [SerializeField] private float cutoutSize = 3f;
    private Material mat;

    private float lerp;
    private int lerpIncrement;
    private void Start()
    {
        if(playerTransform == null) playerTransform = LevelManager.instance.player.transform;
        if (mat == null) mat = GetComponent<Renderer>().sharedMaterial;
        lerp = 0f;
        lerpIncrement = -1;
        mat.SetFloat("_CutoutSize", 0f);
    }
    
    private void Update()
    {
        //the cutout size animation only plays on play mode
        if(!Application.isPlaying) rend.sharedMaterial.SetVector("_CutoutPosition", playerTransform.position);
        else
        {
            lerp += Time.deltaTime * transitionSpeed * lerpIncrement;
            lerp = Mathf.Clamp(lerp, 0f, 1f);
            mat.SetVector("_CutoutPosition", playerTransform.position);
            mat.SetFloat("_CutoutSize", Mathf.Lerp(0f, cutoutSize, lerp));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == playerTransform.name)
        {
            lerpIncrement = 1;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == playerTransform.name)
        {
            lerpIncrement = -1;
        }
    }
}
