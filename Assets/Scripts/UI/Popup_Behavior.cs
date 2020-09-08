using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class Popup_Behavior : MonoBehaviour
{
    public string messege;
    public TextMeshProUGUI text;
    [Header("Animação")]
    public float time;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = messege;
        Destroy(this.gameObject, 1.5f);
        text.DOFade(0, time);
    }
}
