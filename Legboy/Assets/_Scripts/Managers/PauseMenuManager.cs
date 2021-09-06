using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public Button continueButton;

    private void OnEnable()
    {
        SelectFirstButton();
    }

    void SelectFirstButton()
    {
        StartCoroutine(SelectFirstButtonAfterFrame());
    }

    //waits 1 frame because >Unity<
    IEnumerator SelectFirstButtonAfterFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
    }

    public void ReturnToMainMenu()
    {
        GameStateManager.instance.ResumeWithScreen();
        ScenesManager.instance.ChangeSecondaryScene("Main_Menu", false);
    }
}
