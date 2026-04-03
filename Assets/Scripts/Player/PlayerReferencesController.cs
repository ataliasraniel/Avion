using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReferencesController : MonoBehaviour
{
  public PlayerInput playerInput;

  public void Setup(PlayerInput input)
  {
    playerInput = input;
    print("PlayerReferencesController received PlayerInput reference: " + input.name);
  }
}