using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public GameObject shotPrefab;
    public float shotsPerSecond = 1f;
    public float shotSpeed = 22f;
    public float shotSize = 1f;
    public Color32 shotColor = new Color32(255, 255, 255, 255);
    public float shotSpawnX = 6f;
    public float shotSpawnY = 0f;
    [Range(0f, 1f)]
    public float shotSpawnAccuracy = 1f;
    [Range(0f, 1f)]
    public float directionAccuracy = 1f;
    [Range(0f, 1f)]
    public float visualPower = 0f;
    
    public float GetShotSpawnDelay ()
    {
        return 1f / shotsPerSecond;
    }

    public Vector3 GetShotSpawnPosition(float facing = 1f)
    {
        Vector3 basePosition = new Vector3(shotSpawnX * facing, shotSpawnY, 0f);
        if (shotSpawnAccuracy < 1f)
            return basePosition + new Vector3(Random.Range(-(1f - shotSpawnAccuracy), (1f - shotSpawnAccuracy)), Random.Range(-(1f - shotSpawnAccuracy), (1f - shotSpawnAccuracy)), 0f);
        return basePosition;
    }

    public Vector3 GetShotVelocity (float facing = 1f)
    {
        Vector3 basePosition = new Vector2(shotSpeed * facing, 0f);
        if (directionAccuracy < 1f)
        {
            float a = 2f * (1f - directionAccuracy);
            return basePosition + new Vector3(Random.Range(-a, a), Random.Range(-a, a), 0f);
        }
        return basePosition;
    }

    public float GetRecoil (float facing = 1f)
    {
        return 5f * visualPower * facing;
    }

}
