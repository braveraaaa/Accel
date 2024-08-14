using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRain : MonoBehaviour
{
    Rigidbody rb;
    public float rainTime = 1.5f;
    private float reset_counter = 0f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        reset_counter += Time.deltaTime * TimeManager.worldTimeScale;
        if (reset_counter > rainTime)
        {
            Debug.Log("Rain Reset");
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.transform.position = new Vector3(0, 3, 3);
            reset_counter = 0f;
        }
    }
}
