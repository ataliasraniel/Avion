using UnityEngine;

[CreateAssetMenu(fileName = "NewAirplaneData", menuName = "Avion/Airplane Data")]
public class AirplaneData : ScriptableObject
{
    [Header("Basic Information")]
    public string airplaneName = "Plane";
    public string airplaneDescription = "Description here...";
    public Sprite airplaneIcon;
    [Tooltip("The actual playable prefab that will be spawned")]
    public GameObject airplanePrefab;

    [Header("Airframe Physics (Airplane.cs)")]
    public float thrust = 100f;
    public float forceMult = 1000f;
    public Vector3 turnTorque = new Vector3(90f, 25f, 45f);
    public float minSpeed = 30f;
    public float maxSpeed = 150f;

    [Header("Autopilot & Handling (Airplane.cs)")]
    public float actualSensitivity = 5f;
    public float actualTurnAngle = 10f;
    public float lookSensitivity = 2f;
    public float lookTurnAngle = 4f;

    [Header("Motor / Engine (Player_Motor.cs)")]
    public float rotSpeedX = 50f;
    public float rotSpeedY = 50f;
    public float accelerateBoost = 5f;
    public float desacelerateBoost = 3f;

    [Header("Propeller (Propeller.cs)")]
    public float maxRpm = 360f;
    public float rpmMultiplier = 0.1f;
    public float propSpeedMultiplier = 0.1f;
    public float reverseSpeedMultiplier = 0.3f;

    [Header("Boost System (Booster.cs)")]
    public float boostPower = 600f;
    public float boostTime = 1f;
    public float boostCooldown = 1f;

    [Header("Armament (PlayerShootSystem.cs)")]
    public int gunMinDamage = 2;
    public int gunMaxDamage = 3;
    public float fireRate = 0.3f;
    public int maxMagazine = 200;
    public int startingMunition = 1000;

    [Header("Look Sensors (MouseController.cs)")]
    public float camSmoothSpeed = 5f;
    public float mouseSensitivity = 3f;
    public float joystickSensitivity = 60f;
    public float aimDistance = 500f;
}
