using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ResizeBackground : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Camera mainCam;
    
     void Start ()
     {
         spriteRenderer = GetComponent<SpriteRenderer>();
         mainCam = Camera.main;
         
         var height = mainCam.orthographicSize * 2;
         var width = height * Screen.width/ Screen.height; // basically height * screen aspect ratio
 
         Sprite s = spriteRenderer.sprite;
         var unitWidth = s.textureRect.width / s.pixelsPerUnit;
         var unitHeight = s.textureRect.height / s.pixelsPerUnit;
 
         spriteRenderer.transform.localScale = new Vector3(width / unitWidth, height / unitHeight);
     }

     private void Update()
     {
         var height = mainCam.orthographicSize * 2;
         var width = height * Screen.width/ Screen.height; // basically height * screen aspect ratio
 
         Sprite s = spriteRenderer.sprite;
         var unitWidth = s.textureRect.width / s.pixelsPerUnit;
         var unitHeight = s.textureRect.height / s.pixelsPerUnit;
 
         spriteRenderer.transform.localScale = new Vector3(width / unitWidth, height / unitHeight);
     }
}
