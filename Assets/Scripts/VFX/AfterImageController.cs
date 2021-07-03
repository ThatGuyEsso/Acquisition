using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageController : MonoBehaviour
{
    [SerializeField] private GameObject afterImageObject;
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private float imageLifeTime;
    [SerializeField] private float timeBetweenImage;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private bool startOnEnable =false;

    private bool isDrawing;
    private float currentTime;
    
    private void Awake()
    {
        if (!targetTransform) targetTransform = transform;
    }

    private void OnEnable()
    {
        if (startOnEnable)
            StartDrawing();
    }
    private void OnDisable()
    {
        if (startOnEnable) StopDrawing();
    }
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
              
                CreateAfterImage(targetRenderer.sprite, imageLifeTime, targetTransform.position, targetTransform.rotation);
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

    public void SetUpRenderer(SpriteRenderer renderer, Transform targetTrans)
    {
        targetRenderer = renderer;
        targetTransform = targetTrans;
    }

    public void SetUpRenderer(SpriteRenderer renderer, float timeBtwnImage,float imageLifeTime)
    {
        targetRenderer = renderer;
        this.imageLifeTime = imageLifeTime;
        timeBetweenImage = timeBtwnImage;
    }
}
