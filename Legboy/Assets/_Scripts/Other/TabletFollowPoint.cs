using UnityEngine;

public class TabletFollowPoint : MonoBehaviour
{
    public static TabletFollowPoint instance;
    private void Awake()
    {
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion
    }
    
    public void FlipPos(int side)
    {
        transform.localPosition = new Vector3(side * Mathf.Abs(transform.localPosition.x), transform.localPosition.y,
            transform.localPosition.z);
    }

}