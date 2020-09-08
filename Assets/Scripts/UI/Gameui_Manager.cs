using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gameui_Manager : MonoBehaviour
{
    public TextMeshProUGUI cameraText;
    public Transform popupPos;
    public GameObject popupPrefab;

    private void Start()
    {
        
    }
    public void ShowPopup(string messege)
    {
        var clone = Instantiate(popupPrefab, popupPos);
        clone.GetComponent<Popup_Behavior>().messege = messege;

    }
    #region animações

    #endregion
}