using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levitation : MonoBehaviour
{
    [Range(0,2)]
    public int syncIndex = 1;
    private Vector2 startPos;
    public float amplitude = 1f;
    void Awake()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position += new Vector3(0f, LevitationManager.instance.GetLevitation(syncIndex) * Time.deltaTime, 0f);
    }

    public void ResetToStartPos()
    {
        transform.position = startPos + new Vector2(0f, LevitationManager.instance.GetLevitation(syncIndex));
    }
}
