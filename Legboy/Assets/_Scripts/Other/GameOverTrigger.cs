using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        LevelStatsManager.instance.StopTimer();
        ScenesManager.instance.ChangeSecondaryScene("Playtest_End", false);
    }
}
