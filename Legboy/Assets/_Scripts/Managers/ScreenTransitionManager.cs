using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenTransitionManager : MonoBehaviour
{
    public RectTransform transFigure;
    public float moveDuration = 2f;
    
    [Tooltip("Animation curve.")]
    public Ease moveEase = Ease.Linear;

    private Vector2 startPos;
    private TweenCallback endCallback;
    
    public static ScreenTransitionManager instance;
    private void Awake()
    {
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion

        startPos = transFigure.position;
    }

    public void StartTransition(TweenCallback callback, TweenCallback callbackEnd)
    {
        endCallback = callbackEnd;
        transFigure.position = startPos;
        Vector2 center = new Vector2(Screen.width/2f, Screen.height/2f);
        DOTween.Sequence().Append(transFigure.DOMove(center, moveDuration/3).SetEase(moveEase).SetUpdate(true)).SetUpdate(true).AppendCallback(callback).AppendCallback(StartCompleteTransitionCoroutine);
    }

    private void StartCompleteTransitionCoroutine()
    {
        StartCoroutine(CompleteTransitionAfterTime());
    }

    IEnumerator CompleteTransitionAfterTime()
    {
        yield return new WaitForSecondsRealtime(moveDuration/3);
        CompleteTransition();
    }
    
    private void CompleteTransition()
    {
        if(endCallback != null) DOTween.Sequence().Append(transFigure.DOMove(-startPos, moveDuration/2).SetEase(moveEase).SetUpdate(true)).SetUpdate(true).AppendCallback(endCallback);
        else transFigure.DOMove(-startPos, moveDuration/3).SetEase(moveEase).SetUpdate(true);
    }
}
