using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : Collectable
{
    protected override void Collect()
    {
        LevelManager.instance.AddCollected(this);
        DiamondsManager.instance.AddDiamond();
        base.Collect();
    }

    public override void Respawn()
    {
        base.Respawn();
        DiamondsManager.instance.DeductDiamond();
    }
}
