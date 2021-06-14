using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnEnable : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private void OnEnable()
    {
        if(animator)
            animator.enabled = true;
    }
}
