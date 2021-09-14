using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGOWhenDisabled : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;

    [SerializeField] private Transform targetTransform;

    private void OnDisable()
    {
        if (objectToSpawn) ObjectPoolManager.Spawn(objectToSpawn, targetTransform.position, transform.rotation);
    }

}
