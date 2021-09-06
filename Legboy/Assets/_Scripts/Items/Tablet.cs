using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablet : Collectable
{
    public int index;
    
    protected override void Collect()
    {
        TabletOrb.instance.SetActive(true, transform.position);
        LevelManager.instance.AddTabletCollected(this);
        base.Collect();
    }

    public void SaveOnCheckpoint()
    {
        TabletOrb.instance.SetActive(false, CheckpointManager.instance.CurrentCheckpoint.transform.position);
        TabletMenuManager.instance.UnlockTablet(index);
        //play orb animation here
    }

    public override void Respawn()
    {
        base.Respawn();
        TabletOrb.instance.SetActive(false);
    }
}
