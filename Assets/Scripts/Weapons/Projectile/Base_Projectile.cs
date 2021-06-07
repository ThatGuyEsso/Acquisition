using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Projectile : MonoBehaviour,IInitialisable
{
    [SerializeField] protected ProjectileSettings settings;
    [SerializeField] private bool inDebug = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        if (inDebug)
            Init();
    }

    public void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    public void SetUp(Vector3 Direction)
    {
        rb.velocity = Direction * settings.projectileSpeed;
    }

}
