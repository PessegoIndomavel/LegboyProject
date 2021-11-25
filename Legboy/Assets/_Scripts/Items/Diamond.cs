using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : Collectable
{
    protected override void Collect()
    {
        LevelManager.instance.AddCollected(this);
        DiamondsManager.instance.AddDiamond();
        if(DiamondPickupVFX.instance != null) DiamondPickupVFX.instance.PlayBurst(transform.position.AsVector2());
        else print("DiamondPickupVFX instance not found!");
        base.Collect();
    }

    public override void Respawn()
    {
        base.Respawn();
        DiamondsManager.instance.DeductDiamond();
    }
}
