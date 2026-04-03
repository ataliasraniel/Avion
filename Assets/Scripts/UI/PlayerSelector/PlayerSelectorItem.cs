using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSelectorItem : MonoBehaviour
{
  [Header("UI Elements")]
  public Image iconImage;
  public TextMeshProUGUI nameText;

  [HideInInspector]
  public AirplaneData data;

  /// <summary>
  /// Recebe os dados do avião e injeta nas variáveis visuais da UI.
  /// </summary>
  public void Setup(AirplaneData airplaneData)
  {
    data = airplaneData;
    if (nameText != null)
    {
      nameText.text = data.airplaneName;
    }

    if (iconImage != null && data.airplaneIcon != null)
    {
      iconImage.sprite = data.airplaneIcon;
    }
  }
}
