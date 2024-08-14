using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverDriver : MonoBehaviour
{
    public Rigidbody rb;
    public float odPower;
    public float odEffect { get; protected set; }
    public float odTime;
    protected float odTime_counter = 0f;
    public float od_coolTime;
    protected float od_coolTime_counter = 0f;
    public bool isOD { get; protected set; }
    protected float transitionProgress = 0f;
    protected virtual void Start()
    {
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }
}
