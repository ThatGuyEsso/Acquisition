using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float rotateRate;

    public bool isRotating = true;


    private void Update()
    {
        if (isRotating)
        {
            transform.Rotate(new Vector3(0.0f, 0.0f, rotateRate * Time.deltaTime));
        }
    }
}
