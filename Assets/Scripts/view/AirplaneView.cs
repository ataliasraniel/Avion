using UnityEngine;

public class AirplaneView : MonoBehaviour
{
    [Header("Airplane Identity")]
    public AirplaneData airplaneData;

    private void Awake()
    {
        // Inject properties from ScriptableObject on initialization
        if (airplaneData != null)
        {
            InjectData();
        }
    }

    public void InjectData()
    {
        // 1. Airplane.cs Physics & Flight handling
        if (TryGetComponent<Airplane>(out Airplane airplane))
        {
            airplane.thrust = airplaneData.thrust;
            airplane.forceMult = airplaneData.forceMult;
            airplane.turnTorque = airplaneData.turnTorque;
            airplane.minSpeed = airplaneData.minSpeed;
            airplane.maxSpeed = airplaneData.maxSpeed;
            airplane.actualSensitivity = airplaneData.actualSensitivity;
            airplane.actualTurnAngle = airplaneData.actualTurnAngle;
        }

        // 2. Booster.cs
        if (TryGetComponent<Booster>(out Booster booster))
        {
            booster.boostPower = airplaneData.boostPower;
            booster.boostTime = airplaneData.boostTime;
            booster.cooldown = airplaneData.boostCooldown;
        }

        // 3. Player_Motor.cs
        if (TryGetComponent<Player_Motor>(out Player_Motor motor))
        {
            motor.rotSpeedX = airplaneData.rotSpeedX;
            motor.rotSpeedY = airplaneData.rotSpeedY;
            motor.accelerateBoost = airplaneData.accelerateBoost;
            motor.desacelerateBoost = airplaneData.desacelerateBoost;
            // If MaxSpeed exists on the motor as well:
            motor.maxSpeed = airplaneData.maxSpeed;
        }

        // 4. Propeller.cs
        if (TryGetComponent<Propeller>(out Propeller propeller))
        {
            propeller.maxRpm = airplaneData.maxRpm;
            propeller.rpmMultiplier = airplaneData.rpmMultiplier;
            propeller.speedMultiplier = airplaneData.propSpeedMultiplier;
            propeller.reverseSpeedMultiplier = airplaneData.reverseSpeedMultiplier;
        }

        // 5. PlayerShootSystem.cs
        if (TryGetComponent<PlayerShootSystem>(out PlayerShootSystem shooter))
        {
            shooter.gunMinDamage = airplaneData.gunMinDamage;
            shooter.gunMaxDamage = airplaneData.gunMaxDamage;
            shooter.fireRate = airplaneData.fireRate;
            shooter.MaxMagazine = airplaneData.maxMagazine;
            // Refilling current magazine logic
            shooter.magazine = airplaneData.maxMagazine;
            shooter.munition = airplaneData.startingMunition;
        }
    }
}
