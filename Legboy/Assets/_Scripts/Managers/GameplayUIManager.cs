using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private float activeTime = 3f;
    [SerializeField] private GameObject uiObject;
    [SerializeField] private TextMeshProUGUI crystalCounterText;
    [SerializeField] private TextMeshProUGUI tabletCounterText;

    private Coroutine turnOffCoroutine;

    public static GameplayUIManager instance;

    private void Awake()
    {
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion
    }
    
    //mexer nessa funcao e nas paradas associadas a ela futuramente pq esta um coco
    public void ShowUI(bool turnOffAfterTime = true)
    {
        if (turnOffCoroutine != null) StopCoroutine(turnOffCoroutine);

        crystalCounterText.text = DiamondsManager.instance.GetDiamonds().ToString() + " / 100";
        tabletCounterText.text = TabletMenuManager.instance._tabletsAvailable.Count(t => t) + " / 1";
        
        uiObject.SetActive(true);
        
        if (turnOffAfterTime) turnOffCoroutine = StartCoroutine(TurnOffAfterTime(activeTime));
    }

    IEnumerator TurnOffAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        uiObject.SetActive(false);
    }
}
