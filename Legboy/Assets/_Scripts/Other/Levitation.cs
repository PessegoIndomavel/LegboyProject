using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levitation : MonoBehaviour
{
    [Range(0,2)]
    public int syncIndex = 1;
    private float startPosY;
    public float amplitude = 1f;
    void Awake()
    {
        startPosY = transform.position.y;
    }

    void Update()
    {
        transform.position += new Vector3(0f, LevitationManager.instance.GetLevitation(syncIndex) * Time.deltaTime, 0f);
    }
}
