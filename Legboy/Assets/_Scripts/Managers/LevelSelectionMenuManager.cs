using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectionMenuManager : MonoBehaviour
{
    public static LevelSelectionMenuManager instance;

    private void Awake()
    {
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion
        
        gameObject.SetActive(false);
    }
    
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private Button returnButton;
    [SerializeField] private GameObject startButton;

    private List<Button> sceneButtons = new List<Button>();
    private bool generatedButtons;

    public void GenerateButtons(List<string> scenesNames)
    {
        if (generatedButtons) return;
        
        foreach (var sceneName in scenesNames)
        {
            var sceneButtonGO = Instantiate(buttonPrefab, buttonsParent);
            var button = sceneButtonGO.GetComponent<Button>();
            button.onClick.AddListener(delegate
            {
                ScenesManager.instance.ButtonChangeSecondaryScene(sceneName, true);
            });

            button.GetComponentInChildren<TextMeshProUGUI>().text = sceneName;
            
            sceneButtons.Add(button);
        }

        int i;
        for (i = 0; i < sceneButtons.Count; i++)
        {
            var buttonNavigation = sceneButtons[i].navigation;
            
            buttonNavigation.selectOnRight = returnButton;
            buttonNavigation.selectOnUp = i == 0 ? sceneButtons[sceneButtons.Count - 1] : sceneButtons[i - 1];
            buttonNavigation.selectOnDown = i == sceneButtons.Count - 1 ? sceneButtons[0] : sceneButtons[i + 1];
            
            sceneButtons[i].navigation = buttonNavigation;
        }

        var temp = returnButton.navigation;
        temp.selectOnLeft = sceneButtons[0];
        returnButton.navigation = temp;

        generatedButtons = true;
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(sceneButtons.Count > 0
            ? sceneButtons[0].gameObject
            : returnButton.gameObject);
    }

    public void ReturnToMainMenu()
    {
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        gameObject.SetActive(false);
    }
}
