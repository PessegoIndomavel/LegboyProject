using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TextBoxZone : MonoBehaviour
{
    public List<String> text;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (text != null) TextBoxManager.instance.ShowAllText(text);
        Destroy(this);
    }
}
