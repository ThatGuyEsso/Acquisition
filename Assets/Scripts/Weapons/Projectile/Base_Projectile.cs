using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Projectile : MonoBehaviour,IInitialisable
{
    [SerializeField] public float damagerPerProjectile = 1.0f;
    [SerializeField] private float projectileSpeed = 10.0f;
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
        rb.velocity = Direction * projectileSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

}
