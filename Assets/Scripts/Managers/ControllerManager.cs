using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour
{
  //singleton class

  public static ControllerManager instance;

  public GameInputActions inputActions;

  [Header("Bools")]
  [SerializeField] bool _useMouse = false;
  public bool UseMouse { get { return _useMouse; } }


  private void Awake()
  {
    instance = this;
  }

  private void Start()
  {
    inputActions = new GameInputActions();

  }


  public void ChangeControllerType(bool value)
  {
    _useMouse = value;
  }

  public void ToggleControllerType()
  {
    _useMouse = !_useMouse;
  }

  public void EnableGameController()
  {
    inputActions.Enable();
  }

  public void DisableGameController()
  {
    inputActions.Disable();

  }
}