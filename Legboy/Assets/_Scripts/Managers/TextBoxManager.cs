using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TextBoxManager : MonoBehaviour
{
    public float letterAppearInterval = 0.05f;
    public float canCloseAfterTime = 1f;

    [SerializeField]
    private TextMeshProUGUI textBoxTMPro;
    [SerializeField] private GameObject textBox;
    [SerializeField] private GameObject nextImage;
    
    private List<String> curText = new List<string>();
    private Coroutine teletypeCoroutine;
    private bool teletype;
    private bool canNext = false;
    public bool showTextBoxes;
    
    public static TextBoxManager instance;
    private void Awake()
    {
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion
        showTextBoxes = true;
        curText.Add("");
    }

    public bool GetTextBoxActive()
    {
        return textBox.activeSelf;
    }

    private void ShowTextBox()
    {
        if (showTextBoxes == false || textBox.activeSelf) return;
        GameStateManager.instance.StopTimeWithTransition(ActuallyShowTextBox);
    }

    private void ActuallyShowTextBox()
    {
        ControlsManager.instance.controlInput.UI.Submit.performed += NextPage;
        ControlsManager.instance.controlInput.UI.Enable();
        ControlsManager.instance.controlInput.UI.Submit.Enable();
        if (ScenesManager.instance.isLevel) LevelManager.instance.player.GetComponent<PlayerMovement>().DisableControls();
        textBox.SetActive(true);
        StartCoroutine(CanNextAfterTime(canCloseAfterTime));
    }

    public void HideTextBox()
    {   
        ControlsManager.instance.controlInput.UI.Submit.performed -= NextPage;
        if (ScenesManager.instance.isLevel) LevelManager.instance.player.GetComponent<PlayerMovement>().EnableControls();
        StopTextTeletype();
        textBox.SetActive(false);
        teletype = false;
        curText = null;
        nextImage.SetActive(false);
        canNext = false;
        
        GameStateManager.instance.ResumeTimeWithTransition(null);
    }

    public void ForceHideTextBox()
    {
        ControlsManager.instance.controlInput.UI.Submit.performed -= NextPage;
        if (ScenesManager.instance.isLevel) LevelManager.instance.player.GetComponent<PlayerMovement>().EnableControls();
        StopTextTeletype();
        textBox.SetActive(false);
        teletype = false;
        curText = null;
        nextImage.SetActive(false);
        canNext = false;
        GameStateManager.instance.ResumeTime();
    }

    void NextPage(InputAction.CallbackContext callbackContext)
    {
        if (!canNext || GameStateManager.instance.isPaused()) return;
        if (curText == null || curText.Count <= 1)
        {
            HideTextBox();
            return;
        }

        curText.RemoveAt(0);
        if (teletype) StartTextTeletype(curText);
        else ShowAllText();
    }
    
    //shows text without teletyping
    public void ShowAllText(List<String> text)
    {
        curText = text;
        ShowAllText();
        teletype = false;
    }

    //shows current text without teletyping
    public void ShowAllText()
    {
        //canNext = false;
        //nextImage.SetActive(false);
        textBoxTMPro.text = curText[0];
        ShowTextBox();
    }

    IEnumerator CanNextAfterTime(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        canNext = true;
        nextImage.SetActive(true);
    }

    //shows text with teletyping
    public void StartTextTeletype(List<String> text)
    {
        StopTextTeletype();
        canNext = false;
        nextImage.SetActive(false);
        curText = text;
        textBoxTMPro.text = curText[0];
        ShowTextBox();
        teletypeCoroutine = StartCoroutine(Teletype());
        teletype = true;
    }

    private void StopTextTeletype()
    {
        if(teletypeCoroutine != null) StopCoroutine(teletypeCoroutine);
        textBoxTMPro.text = "";
    }

    private IEnumerator Teletype()
    {
        if (textBoxTMPro == null)
        {
            print("Missing TextMeshProUGUI reference.");
            yield return null;
        }
        
        //SET ALL TO TRANSPARENT
        textBoxTMPro.color = new Color(textBoxTMPro.color.r, textBoxTMPro.color.g, textBoxTMPro.color.b, 0);
 
        // Force and update of the mesh to get valid information.
        textBoxTMPro.ForceMeshUpdate();
 
        TMP_TextInfo textInfo = textBoxTMPro.textInfo;
        int totalCharacters = textBoxTMPro.textInfo.characterCount; // Get # of Total Character in text object
        int visibleCount = 0;
       
        bool reading = true;
 
        Color32[] newVertexColors;
 
        yield return new WaitForSeconds(letterAppearInterval);
        
        while (reading && visibleCount < totalCharacters){
 
            if (GameStateManager.instance.getGameState() != GameState.Paused){
               
                int i = visibleCount;
                //if not visible, skip and up the visible count (or else the while loop will get stuck!)
                if (!textInfo.characterInfo[i].isVisible){
                    visibleCount += 1;
                } else {
                   
 
                    // Get the index of the material used by the current character.
                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                    // Get the vertex colors of the mesh used by this text element (character or sprite).
                    newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                    // Get the index of the first vertex used by this text element.
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;
 
                    // Set all to full alpha
                    newVertexColors[vertexIndex + 0].a = 255;
                    newVertexColors[vertexIndex + 1].a = 255;
                    newVertexColors[vertexIndex + 2].a = 255;
                    newVertexColors[vertexIndex + 3].a = 255;
 
                    //}
 
                    visibleCount += 1;
                    textBoxTMPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
 
                    if (visibleCount >= totalCharacters)
                    {
                        ShowAllText();
                    }
                }
            }
 
            yield return new WaitForSeconds(letterAppearInterval);
        }

        canNext = true;
        nextImage.SetActive(true);
    }
}
