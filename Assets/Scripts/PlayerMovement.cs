using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : OverDriver
{
    private InputManager input;
    public Transform cameraTransform;
    public string dataId;
    private OverDriverData data;
    public float speed = 40f;
    private float speed_effect;
    public float legPower = 15f;
    public float leg_interval = 0.5f;
    private float leg_interval_counter = 0f;
    protected bool isBoost = false;
    protected bool isJump = false;
    private float target_odEffect = 1f;
    private Vector3 movement;
    protected override void Start()
    {
        DataManager dataManager = FindObjectOfType<DataManager>();
        if (dataManager != null)
        {
            data = dataManager.GetDataById(dataId);
            if (data != null)
            {
                odPower = data.odPower;
                odTime = data.odTime;
                od_coolTime = data.odCoolTime;
            }
        }
        input = new InputManager();
        input.Enable();
        rb = GetComponent<Rigidbody>();
        speed_effect = Mathf.Log(speed);
        isOD = false;
        odEffect = 1f;
    }
    private void OnMove()
    {
        // Moveアクションの入力値を取得
        Vector2 movementVector = input.Player.Move.ReadValue<Vector2>();
        movement = cameraTransform.forward * movementVector.y + cameraTransform.right * movementVector.x;
        movement.y = 0;
    }

    private void Update()
    {
        if (od_coolTime_counter < od_coolTime)
            od_coolTime_counter += Time.deltaTime;
        else if (od_coolTime_counter > od_coolTime){
            Debug.Log("OD Ready");
            od_coolTime_counter = od_coolTime;
        }

        if (input.Player.Fire.WasPressedThisFrame()){
            Debug.Log("OD Activate");
            isOD = true;
        }else if (input.Player.Fire.WasReleasedThisFrame()){
            Debug.Log("OD Deactivate");
            isOD = false;
        }

        if (leg_interval_counter < leg_interval)
            leg_interval_counter += Time.deltaTime;
        else if (leg_interval_counter > leg_interval){
            Debug.Log("Leg Ready");
            leg_interval_counter = leg_interval;
        }

        if (input.Player.Boost.WasReleasedThisFrame()){
            if (leg_interval_counter == leg_interval){
                Debug.Log("Leg Standby");
                isBoost = true;
            }
            else
                Debug.Log("Leg Recasting...");
        }
        if (input.Player.Jump.WasReleasedThisFrame())
            isJump = true;
    }

    private void FixedUpdate()
    {
        // 入力値を元に3軸ベクトルを作成
        target_odEffect = isOD ? odPower :  1f;

        if (Mathf.Abs(odEffect - target_odEffect) > 0.01f)
        {
            odEffect = Mathf.Lerp(odEffect, target_odEffect, TimeManager.easeCurve.Evaluate(transitionProgress));
        }
        if (transitionProgress == 0f)
        {
            odEffect = target_odEffect;
        }
        Debug.Log(odEffect);
        if (odEffect > target_odEffect){
            rb.AddForce(-rb.velocity * odEffect * speed_effect, ForceMode.Force);
        }
        // rigidbodyのAddForceを使用してプレイヤーを動かす。
        if (isBoost){
            isBoost = false;
            if (movement != Vector3.zero){
                leg_interval_counter = 0f;
                rb.AddForce(movement.normalized * odEffect * legPower, ForceMode.Impulse);
            }
        }else if (isJump){
            isJump = false;
            rb.AddForce(Vector3.up * odEffect * legPower, ForceMode.Impulse);
        }
        else
            rb.AddForce(movement * MathF.Pow(odEffect * speed_effect, 2f), ForceMode.Force);
    }
}
