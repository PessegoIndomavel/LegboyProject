using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFlipbookIntervalSeed : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Renderer>().material.SetVector("_RandomIntervalSeed", transform.position.AsVector2());
    }
}
