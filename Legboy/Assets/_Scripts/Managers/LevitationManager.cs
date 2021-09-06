using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationManager : MonoBehaviour
{
    public float offsetTime = 0.1f;
    private const int syncIndexes = 3;
    public float loopDuration = 2f;
    public float loopAmplitude = 1f;

    private List<float> curPos;
    
    public static LevitationManager instance;
    private void Awake()
    {
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion
        
        curPos = new List<float>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < syncIndexes; i++)
        {
            curPos.Add(0f);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < curPos.Count; i++)
        {
            curPos[i] = Mathf.Sin(Mathf.PI * (Time.time - (offsetTime * (i + 1))) * (1/loopDuration)) * loopAmplitude;
        }
    }

    public float GetLevitation(int syncIndex)
    {
        if (syncIndex < curPos.Count && syncIndex >= 0) return curPos[syncIndex];
        print("Invalid value on syncIndex.");
        return 0f;
    }
}
