using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningBlades : MonoBehaviour,IDamage
{
    [SerializeField] private GhostBlade[] blades;
    [SerializeField] private float damagePerBlade;
    [SerializeField] private float rotateRate;
    private int activeCount =0;

    private GameObject owner;
    public void Awake()
    {
        blades = gameObject.GetComponentsInChildren<GhostBlade>();
      

    }


    private void OnEnable()
    {
        activeCount = blades.Length;
        foreach(GhostBlade blade in blades)
        {
            blade.gameObject.SetActive(true);

            blade.Init();
        }
    }



    public void LateUpdate()
    {
        if (owner)
        {
            transform.position = owner.transform.position;
        }
    }
    public void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, Time.deltaTime * rotateRate));
    }
    public void SetUp(GameObject owner)
    {
        this.owner = owner;
        foreach (GhostBlade blade in blades)
        {
            blade.gameObject.SetActive(true);
            blade.SetUpProjectile(damagePerBlade, Vector2.zero, 0.0f, 0f,1,  owner);
            blade.OnBladeDestroyed += OnBladeBroken;
        }

        activeCount = blades.Length;
    }


    public void OnBladeBroken(GhostBlade blade )
    {
        blade.OnBladeDestroyed -= OnBladeBroken;
        activeCount--;
        if (activeCount <= 0)
        {
            ObjectPoolManager.Recycle(gameObject);
        }
    }

    public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
      //
    }
}
