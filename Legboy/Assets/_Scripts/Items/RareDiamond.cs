public class RareDiamond : Collectable
{
    protected override void Collect()
    {
        LevelManager.instance.AddCollected(this);
        RareDiamondsManager.instance.AddRDiamond();
        base.Collect();
    }

    public override void Respawn()
    {
        base.Respawn();
        RareDiamondsManager.instance.DeductRDiamond();
    }
}
