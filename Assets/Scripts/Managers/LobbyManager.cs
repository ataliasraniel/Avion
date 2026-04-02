using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInputManager))]
public class LobbyManager : MonoBehaviour
{
  public static LobbyManager instance;

  public PlayerInputManager playerInputManager;


  private void Awake()
  {
    instance = this;
    playerInputManager = GetComponent<PlayerInputManager>();
  }

  public void OnPlayerJoined(PlayerInput playerInput)
  {
    Debug.Log("Player Joined: " + playerInput.playerIndex);


  }

}