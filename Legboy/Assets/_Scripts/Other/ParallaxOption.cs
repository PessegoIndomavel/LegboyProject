using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxOption : MonoBehaviour
{
    public bool moveParallax;

    [SerializeField]
    [HideInInspector]
    private Vector3 storedPosition;

    public void SavePosition() {
        storedPosition = transform.position;
    }

    public void RestorePosition() {
        transform.position = storedPosition;
    }
}
