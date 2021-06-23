using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoss 
{
    void SetUseRigidBody(bool use);

    Transform GetTarget();
    Rigidbody2D GetRigidBody();

    public void PlayAnimation(string animName);
}
