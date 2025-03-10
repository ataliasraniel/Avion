﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
  [Header("Components")]
  [SerializeField]
  [Tooltip("Transform of the aircraft the rig follows and references")]
  private Transform aircraft = null;
  [SerializeField]
  [Tooltip("Transform of the object the mouse rotates to generate MouseAim position")]
  private Transform mouseAim = null;
  [SerializeField]
  [Tooltip("Transform of the object on the rig which the camera is attached to")]
  private Transform cameraRig = null;
  [SerializeField]
  [Tooltip("Transform of the camera itself")]
  private Transform cam = null;

  [Header("Options")]
  [SerializeField]
  [Tooltip("Follow aircraft using fixed update loop")]
  private bool useFixed = true;

  [SerializeField]
  [Tooltip("How quickly the camera tracks the mouse aim point.")]
  private float camSmoothSpeed = 5f;

  [SerializeField]
  [Tooltip("Mouse sensitivity for the mouse flight target")]
  private float mouseSensitivity = 3f;

  [SerializeField]
  [Tooltip("How far the boresight and mouse flight are from the aircraft")]
  private float aimDistance = 500f;

  [Space]
  [SerializeField]
  [Tooltip("How far the boresight and mouse flight are from the aircraft")]

  private bool showDebugInfo = false;

  private Vector3 frozenDirection = Vector3.forward;
  private bool isMouseAimFrozen = false;

  private ControllerManager controllerManager;
  /// <summary>
  /// Get a point along the aircraft's boresight projected out to aimDistance meters.
  /// Useful for drawing a crosshair to aim fixed forward guns with, or to indicate what
  /// direction the aircraft is pointed.
  /// </summary>
  public Vector3 BoresightPos
  {
    get
    {
      return aircraft == null
           ? transform.forward * aimDistance
           : (aircraft.transform.forward * aimDistance) + aircraft.transform.position;
    }
  }

  /// <summary>
  /// Get the position that the mouse is indicating the aircraft should fly, projected
  /// out to aimDistance meters. Also meant to be used to draw a mouse cursor.
  /// </summary>
  public Vector3 MouseAimPos
  {
    get
    {
      if (mouseAim != null)
      {
        return isMouseAimFrozen
            ? GetFrozenMouseAimPos()
            : mouseAim.position + (mouseAim.forward * aimDistance);
      }
      else
      {
        return transform.forward * aimDistance;
      }
    }
  }

  private void Awake()
  {
    if (aircraft == null)
      Debug.LogError(name + "MouseFlightController - No aircraft transform assigned!");
    if (mouseAim == null)
      Debug.LogError(name + "MouseFlightController - No mouse aim transform assigned!");
    if (cameraRig == null)
      Debug.LogError(name + "MouseFlightController - No camera rig transform assigned!");
    if (cam == null)
      Debug.LogError(name + "MouseFlightController - No camera transform assigned!");

    controllerManager = ControllerManager.instance;
    // To work correctly, the entire rig must not be parented to anything.
    // When parented to something (such as an aircraft) it will inherit those
    // rotations causing unintended rotations as it gets dragged around.
    transform.parent = null;
  }

  private void Update()
  {
    if (useFixed == false)
      UpdateCameraPos();

    RotateRig();
  }

  private void FixedUpdate()
  {
    if (useFixed == true)
      UpdateCameraPos();
  }

  private void RotateRig()
  {
    if (mouseAim == null || cam == null || cameraRig == null)
      return;

    // Freeze the mouse aim direction when the free look key is pressed.
    if (Input.GetKeyDown(KeyCode.C))
    {
      isMouseAimFrozen = true;
      frozenDirection = mouseAim.forward;
    }
    else if (Input.GetKeyUp(KeyCode.C))
    {
      isMouseAimFrozen = false;
      mouseAim.forward = frozenDirection;
    }

    // Mouse input.
    float moveX = 0;
    float moveY = 0;
    if (controllerManager.UseMouse)
    {
      moveX = Input.GetAxis("Mouse X") * mouseSensitivity;
      moveY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

    }
    else
    {
      // float mouseX = _inputActions.Game.Look.ReadValue<Vector2>().x * mouseSensitivity;
      moveX = controllerManager.inputActions.Game.Look.ReadValue<Vector2>().x;
      // float mouseY = -_inputActions.Game.Look.ReadValue<Vector2>().x;
      moveY = -controllerManager.inputActions.Game.Look.ReadValue<Vector2>().y;
    }

    // Rotate the aim target that the plane is meant to fly towards.
    // Use the camera's axes in world space so that mouse motion is intuitive.
    mouseAim.Rotate(cam.right, moveY, Space.World);
    mouseAim.Rotate(cam.up, moveX, Space.World);

    // The up vector of the camera normally is aligned to the horizon. However, when
    // looking straight up/down this can feel a bit weird. At those extremes, the camera
    // stops aligning to the horizon and instead aligns to itself.
    Vector3 upVec = (Mathf.Abs(mouseAim.forward.y) > 0.9f) ? cameraRig.up : Vector3.up;

    // Smoothly rotate the camera to face the mouse aim.
    cameraRig.rotation = Damp(cameraRig.rotation,
                              Quaternion.LookRotation(mouseAim.forward, upVec),
                              camSmoothSpeed,
                              Time.deltaTime);
  }

  private Vector3 GetFrozenMouseAimPos()
  {
    if (mouseAim != null)
      return mouseAim.position + (frozenDirection * aimDistance);
    else
      return transform.forward * aimDistance;
  }

  private void UpdateCameraPos()
  {
    if (aircraft != null)
    {
      // Move the whole rig to follow the aircraft.
      transform.position = aircraft.position;
    }
  }

  // Thanks to Rory Driscoll
  // http://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
  /// <summary>
  /// Creates dampened motion between a and b that is framerate independent.
  /// </summary>
  /// <param name="a">Initial parameter</param>
  /// <param name="b">Target parameter</param>
  /// <param name="lambda">Smoothing factor</param>
  /// <param name="dt">Time since last damp call</param>
  /// <returns></returns>
  private Quaternion Damp(Quaternion a, Quaternion b, float lambda, float dt)
  {
    return Quaternion.Slerp(a, b, 1 - Mathf.Exp(-lambda * dt));
  }

  private void OnDrawGizmos()
  {
    if (showDebugInfo == true)
    {
      Color oldColor = Gizmos.color;

      // Draw the boresight position.
      if (aircraft != null)
      {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(BoresightPos, 10f);
      }

      if (mouseAim != null)
      {
        // Draw the position of the mouse aim position.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(MouseAimPos, 10f);

        // Draw axes for the mouse aim transform.
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(mouseAim.position, mouseAim.forward * 50f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(mouseAim.position, mouseAim.up * 50f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(mouseAim.position, mouseAim.right * 50f);
      }

      Gizmos.color = oldColor;
    }
  }
}

