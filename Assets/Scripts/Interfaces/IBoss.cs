using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoss 
{
    void SetUseRigidBody(bool use);
    Rigidbody2D GetRigidBody();

    public void PlayAnimation(string animName);
}
