using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TabletMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject tabletMenu;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private RectTransform maskRect;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private GameObject textScreen;
    [SerializeField] private GameObject selectionScreen;
    [SerializeField] private TextMeshProUGUI tabletText;
    [SerializeField] private Scrollbar tabletTextScrollbar;

    public bool[] _tabletsAvailable;
    private Button _firstButton;
    private List<Button> _buttons = new List<Button>();
    private bool canOpenTabletMenu = false;
    private ColorBlock buttonColors;
    private GameObject currentSelected;
    
    private bool isOpen;

    public static TabletMenuManager instance;

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
        buttonColors = buttonPrefab.GetComponent<Button>().colors;
        _tabletsAvailable = new bool[TabletInfo.GetTabletCount()+1];
        
        InitializeTabletButtons();
        SetControls();
    }

    private void SetControls()
    {
        ControlsManager.instance.controlInput.UI.OpenTabletMenu.performed += ToggleTabletMenu;
        ControlsManager.instance.controlInput.UI.GoBackTabletMenu.performed += GoBack;
        
        ControlsManager.instance.controlInput.UI.OpenTabletMenu.Enable();
        ControlsManager.instance.controlInput.UI.GoBackTabletMenu.Enable();
        ControlsManager.instance.controlInput.UI.Enable();

        GameStateManager.instance.onPause += SaveSelected;
        GameStateManager.instance.onUnpause += delegate { SetSelected(true); };
    }
    
    private void OpenTabletMenu()
    {
        if (!canOpenTabletMenu || GameStateManager.instance.isPaused()) return;
        GameStateManager.instance.PauseTime();
        LevelManager.instance.player.GetComponent<PlayerMovement>().DisableControls();
        
        tabletMenu.SetActive(true);
        OpenSelectionScreen();
        isOpen = true;
        
        GameplayUIManager.instance.ShowUI(false);
    }

    private void CloseTabletMenu()
    {
        if (GameStateManager.instance.isPaused() || !isOpen) return;
        GameStateManager.instance.ResumeTime();
        LevelManager.instance.player.GetComponent<PlayerMovement>().EnableControls();
        
        tabletMenu.SetActive(false);
        isOpen = false;
        
        
        GameplayUIManager.instance.ShowUI();
    }

    //called via input system
    private void GoBack(InputAction.CallbackContext callbackContext)
    {
        if (GameStateManager.instance.isPaused()) return;
        
        if(textScreen.activeSelf) OpenSelectionScreen();
        else if(selectionScreen.activeSelf) CloseTabletMenu(); 
    }
    
    //called via input system
    private void ToggleTabletMenu(InputAction.CallbackContext callbackContext)
     {
         if (tabletMenu.activeSelf) CloseTabletMenu();
         else OpenTabletMenu();
     }

    private void OpenTextScreen(string text)
    {
        CloseSelectionScreen();
        textScreen.SetActive(true);
        tabletText.text = text;
        tabletTextScrollbar.value = 1f;
        SetSelected();
    }

    private void CloseTextScreen()
    {
        textScreen.SetActive(false);
    }
    
    private void OpenSelectionScreen()
    {
        CloseTextScreen();
        selectionScreen.SetActive(true);
        SetSelected();
    }
    
    private void CloseSelectionScreen()
    {
        selectionScreen.SetActive(false);
    }

    private void TabletButton(int index)
    {
        if (_tabletsAvailable[index]) OpenTextScreen(TabletInfo.GetText(index));
    }

    private void InitializeTabletButtons()
    {
        var count = TabletInfo.GetTabletCount();
        for (var i = 1; i <= count; i++)
        {
            var newButtonObj = Instantiate(buttonPrefab, contentRect);
            newButtonObj.name = "Tablet Button " + i;

            
            var i1 = i;
            var newButton = newButtonObj.GetComponentInChildren<Button>();
            newButton.onClick.AddListener(delegate{TabletButton(i1);});
            
            if (_tabletsAvailable[i])
            {
                newButtonObj.GetComponentInChildren<TextMeshProUGUI>().text = TabletInfo.GetTitle(i);
                newButton.colors = buttonColors;
            }
            else
            {
                newButtonObj.GetComponentInChildren<TextMeshProUGUI>().text = "?????";
                var newButtonColors = newButton.colors;
                newButtonColors.normalColor = buttonColors.pressedColor;
                newButtonColors.selectedColor = buttonColors.pressedColor;
                newButton.colors = newButtonColors;
            }
            
            _buttons.Add(newButton);
            
            if (i == 1) _firstButton = newButtonObj.GetComponentInChildren<Button>();
        }
    }

    //called when player delivers tablet at checkpoint
    public void UnlockTablet(int index)
    {
        if (index <= 0 || index > TabletInfo.GetTabletCount()) return;
        _tabletsAvailable[index] = true;
        _buttons[index - 1].interactable = _tabletsAvailable[index];
        _buttons[index - 1].GetComponentInChildren<TextMeshProUGUI>().text = TabletInfo.GetTitle(index);
        _buttons[index - 1].colors = buttonColors;
            
        GameplayUIManager.instance.ShowUI();
    }

    public void ResetTablets()
    {
        if (!ScenesManager.instance.isLevel) return;
        
        var count = TabletInfo.GetTabletCount();
        for (var i = 1; i <= count; i++)
        {
            _tabletsAvailable[i] = false;
            _buttons[i - 1].GetComponentInChildren<TextMeshProUGUI>().text = "?????";
            var deactivatedColors = _buttons[i - 1].colors;
            
            deactivatedColors.normalColor = buttonColors.pressedColor;
            deactivatedColors.selectedColor = buttonColors.pressedColor;
            _buttons[i - 1].colors = deactivatedColors;
        }
    }
    
    //called when loading a scene
    public void SetCanOpenTabletMenuOnStart()
    {
        CloseTabletMenu();
        canOpenTabletMenu = ScenesManager.instance.isLevel;
    }

    private void SetSelected(bool fromPause = false)
    {
        if (fromPause) EventSystem.current.SetSelectedGameObject(currentSelected);
        else if (selectionScreen.activeSelf) EventSystem.current.SetSelectedGameObject(_firstButton.gameObject);
        else if (textScreen.activeSelf) EventSystem.current.SetSelectedGameObject(tabletTextScrollbar.gameObject);
    }

    //called when pausing
    private void SaveSelected()
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;
    }

    public bool IsOpen => isOpen;

    public bool CanOpenTabletMenu { get => canOpenTabletMenu; set => canOpenTabletMenu = value; }
}
