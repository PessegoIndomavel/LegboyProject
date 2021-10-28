using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private float activeTime = 3f;
    [SerializeField] private GameObject uiObject;
    [SerializeField] private TextMeshProUGUI crystalCounterText;
    [SerializeField] private TextMeshProUGUI tabletCounterText;
    [SerializeField] private float staminaBarUptime = 2f;

    private Animator staminaBarAnim;
    
    private GameObject staminaBar;
    private Image staminaBarFill;
    private Image staminaBarBG;
    private float currentSBarUptime;
    private bool canFade;
    
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

    private void Start()
    {
        ScenesManager.instance.onLoadScene += GetReferences;
    }

    private void Update()
    {
        if(canFade) FadeStaminaBar();
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

    public void ShowStaminaBar()
    {
        staminaBar.SetActive(true);
        canFade = true;
        staminaBarAnim.SetTrigger("idle");
    }

    public void UpdateStaminaBarFill(float value)
    {
        staminaBarFill.fillAmount = value;
        currentSBarUptime = staminaBarUptime;
    }

    private void FadeStaminaBar()
    {
        if (currentSBarUptime > 0f) currentSBarUptime -= Time.deltaTime;
        else
        {
            canFade = false;
            staminaBarAnim.SetTrigger("fadeOut");
        }
    }

    private void GetReferences()
    {
        if (ScenesManager.instance.isLevel)
        {
            staminaBar = LevelManager.instance.player.GetComponent<PlayerBrain>().staminaBar;
            staminaBarAnim = staminaBar.GetComponent<Animator>();
            var temp = staminaBar.GetComponentsInChildren<Image>();
            staminaBarFill = temp[1];
            canFade = true;
        }
        else
        {
            canFade = false;
        }
    }
}
