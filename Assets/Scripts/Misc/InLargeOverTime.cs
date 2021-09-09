using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InLargeOverTime : MonoBehaviour
{
    [SerializeField] private float targetSize;
    [SerializeField] private float growRate;
    [SerializeField] private Vector3 intialSize;
    private bool isGrowing;

    [SerializeField] private bool  startOnEnable;
    [SerializeField] private bool selfDestroy = true;

    private void OnEnable()
    {
        if(startOnEnable)
             StartGrowing();
    }
    public void LateUpdate()
    {


        if (isGrowing)
        {
            transform.localScale += Vector3.one * Time.deltaTime * growRate;
            if (transform.localScale.x >= targetSize)
            {

                transform.localScale = Vector3.one * targetSize;
                isGrowing = false;
     

            }
        }

    }
    public void StartGrowing()
    {
        isGrowing = true;
    }

    public void SetInitSize(Vector3 initSize)
    {
        intialSize = initSize;
    }
    public void SetUpGrowSetting(Vector3 initSize, float maxSize, float growthRate,float  growDelay)
    {
        intialSize = initSize;
        targetSize = maxSize;
        growRate = growthRate;
        Invoke("StartGrowing", growDelay);
    }
    public void SetUpGrowSetting(float maxSize, float growthRate, float growDelay)
    {
  
        targetSize = maxSize;
        growRate = growthRate;
        Invoke("StartGrowing", growDelay);
    }
    private void OnDisable()
    {
        gameObject.transform.localScale = intialSize;
        if (selfDestroy) 
            Destroy(this);
    }

}