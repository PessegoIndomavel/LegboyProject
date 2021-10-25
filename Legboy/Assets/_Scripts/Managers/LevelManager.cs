using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private void Awake()
    {
        #region Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
        #endregion
        checkpoints = FindObjectsOfType<Checkpoint>();
        
        collected = new List<Collectable>();
        defeated = new List<EnemyLife>(); 
        tabletsCollected = new List<Tablet>();
    }

    [HideInInspector]
    public Checkpoint[] checkpoints;
    public Checkpoint initialCheckpoint;
    public GameObject player;
    public Image staminaBarFill;
    public GameObject exclamation;

    private List<Tablet> tabletsCollected;
    private List<Collectable> collected;
    private List<EnemyLife> defeated;

    public void RespawnCollectedAndDefeated()
    {
        foreach (var item in collected)
        {
            item.Respawn();
        }
        
        foreach (var item in tabletsCollected)
        {
            item.Respawn();
        }

        foreach (var enemy in defeated)
        {
            enemy.Respawn();
        }
        
        tabletsCollected.Clear();
        collected.Clear();
        defeated.Clear();
    }

    //called when player collides with a checkpoint
    public void SaveCollectedAndDefeated()
    {
        if((collected.Count > 0) || (tabletsCollected.Count > 0) || (defeated.Count > 0))
        {
            CheckpointManager.instance.CurrentCheckpoint.myAnim.Play("checkpointEnable");
        }
        
        foreach (var tablet in tabletsCollected)
        {
            tablet.SaveOnCheckpoint();
        }
        
        tabletsCollected.Clear();
        collected.Clear();
        defeated.Clear();
    }

    public void AddTabletCollected(Tablet temp)
    {
        tabletsCollected.Add(temp);
    }
    
    public void AddCollected(Collectable temp)
    {
        collected.Add(temp);
    }

    public void AddDefeated(EnemyLife temp)
    {
        defeated.Add(temp);
    }
}
