using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageController : MonoBehaviour
{
    [SerializeField] private GameObject afterImageObject;
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private float imageLifeTime;
    [SerializeField] private float timeBetweenImage;

    private bool isDrawing;
    private float currentTime;

    public void StartDrawing(float timeBetweenImages)
    {
        timeBetweenImage = timeBetweenImages;
        currentTime = 0f;

        isDrawing = true;
    }
    public void StartDrawing()
    {
       
        currentTime = 0f;

        isDrawing = true;
    }


    public void Update()
    {
        if (isDrawing)
        {
            if(currentTime <= 0f)
            {
                CreateAfterImage(targetRenderer.sprite, imageLifeTime, transform.position,transform.rotation);
                currentTime = timeBetweenImage;
            }
            else
            {
                currentTime -= Time.deltaTime;
            }
        }
    }


    public void StopDrawing()
    {
        isDrawing = false;
    }
    public void CreateAfterImage(Sprite afterImageSprite,float lifeTime, Vector3 pos,Quaternion rot)
    {
        AfterImage afterImage = ObjectPoolManager.Spawn(afterImageObject, pos, rot).GetComponent<AfterImage>();
        if (afterImage) afterImage.StartAfterImage(afterImageSprite, lifeTime);
    }
}
