using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UI_Animations_Controller : MonoBehaviour
{
    ///<sumary>
    ///script responsável pelas animações do menu, todas em iTween.
    ///</sumary>
    public Image[] images;
    [Header("Fade screen")]
    public float duration;
    [Header("Fade painéis")]
    public float pDuration;
    public Image[] panels;

    private void Start()
    {
        foreach (var item in images)
        {
            item.DOFade(0, 0);
        }

    }

    public void Fade(float value)
    {
        foreach (var item in images)
        {
            item.DOFade(value, duration);
        }

    }
    public void PanelsFade(float value)
    {
        foreach (var item in panels)
        {
            item.DOFade(value, duration);
        }

    }
}

