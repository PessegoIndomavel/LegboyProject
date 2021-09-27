using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public Button startButton;
    public TextMeshProUGUI statsText;
    public Toggle TBCheckbox;
    public bool textBoxes = true;
    private bool canPressButton = false;

    private void Awake()
    {
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        SetMenuScreen();
        StartCoroutine(ActivateButtons());
    }

    private IEnumerator ActivateButtons()
    {
        yield return new WaitForSecondsRealtime(2f);
        canPressButton = true;
    }

    public void SetMenuScreen()
    {
        SetStatsTexts();
        TBCheckbox.SetIsOnWithoutNotify(TextBoxManager.instance.showTextBoxes);
    }

    private void SetStatsTexts()
    {
        var temp = TabletMenuManager.instance._tabletsAvailable.Count(t => t);
        statsText.text =
            "Dados da última partida:\n\n - Cristais: "+DiamondsManager.instance.GetDiamonds()+"/100\n\n - Tablets: "+temp+"/1\n\n - Mortes:  "+LifeManager.instance.getDeathCounter+"\n\n - Tempo:  "+LevelStatsManager.instance.getFormatedAccumulatedTime();
    }

    public void StartPlaytestLevel()
    {
        if (!canPressButton) return;
        ScenesManager.instance.ChangeSecondaryScene(ScenesManager.instance.testLevelScene, true);
    }

    public void ToggleTextBoxes()
    {
        TextBoxManager.instance.showTextBoxes = !TextBoxManager.instance.showTextBoxes;
    }

    public void ReturnToMainMenu()
    {
        if (!canPressButton) return;
        ScenesManager.instance.ChangeSecondaryScene("Main_Menu", false);
        GameStateManager.instance.ResumeWithScreen();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
