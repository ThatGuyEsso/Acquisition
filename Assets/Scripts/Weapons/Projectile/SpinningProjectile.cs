using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningProjectile : Base_Projectile
{
    [SerializeField] private float rotationSpeed;
    override public void SetRotationSpeed(float rotSpeed)
    {
        rotationSpeed = rotSpeed;
    }
    public void Update()
    {
        transform.Rotate(new Vector3(0f,0f,Time.deltaTime* rotationSpeed));
    }
}
