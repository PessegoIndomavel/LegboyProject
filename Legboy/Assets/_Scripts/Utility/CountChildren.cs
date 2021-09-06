using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountChildren : MonoBehaviour
{
    void Start()
    {
        print(transform.childCount);
    }
}
