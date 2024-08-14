using System;
using UnityEngine;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public static float worldTimeScale { get; private set; }
    public static float worldDeltaTime { get; private set; }
    public static float transitionProgress { get; private set; }
    public static AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public static float easeSpeed { get; private set; }
    private OverDriver[] OverDrivers;
    private float target_timeScale = 1.0f;
    public float worldTimeScale_underLimit = 0.01f;
    protected int count = 0;
    private bool isOD_world = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        worldTimeScale = 1.0f;
        easeSpeed = 0.3f;
        transitionProgress = 0f;
    }
    void Update()
    {
        // OverDriverを継承しているすべてのインスタンスを取得
        OverDrivers = FindObjectsByType<OverDriver>(FindObjectsSortMode.None);
        count = 0;
        float maxPower = 1f;
        // isODがTrueのものをカウント
        foreach (OverDriver driver in OverDrivers){
            if (driver.isOD){
                count++;
                if (driver.odPower > maxPower)
                    maxPower = driver.odPower;
            }
            if (driver.odEffect > 1f){
                driver.rb.useGravity = false;
                driver.rb.AddForce(Physics.gravity * driver.odEffect, ForceMode.Acceleration);
            }
            else
                driver.rb.useGravity = true;
        }

        
        if (count > 0 && !isOD_world){
            isOD_world = true;
            target_timeScale = Mathf.Max(worldTimeScale_underLimit, 1.0f / maxPower);
            Physics.simulationMode = SimulationMode.Script;
        }

        else if (count == 0 && isOD_world)
        {
            isOD_world = false;
            target_timeScale = 1.0f;
        }

        if (worldTimeScale != target_timeScale)
        {
            transitionProgress += Time.deltaTime * easeSpeed * Mathf.Abs(target_timeScale - worldTimeScale);
            worldTimeScale = Mathf.Lerp(worldTimeScale, target_timeScale, easeCurve.Evaluate(transitionProgress));
        }
        else
        {
            transitionProgress = 0f;
            if (worldTimeScale == 1f)
                Physics.simulationMode = SimulationMode.FixedUpdate;
        }
    }

    void FixedUpdate()
    {
        if (Physics.simulationMode == SimulationMode.Script)
        {
            worldDeltaTime = worldTimeScale * Time.fixedDeltaTime;
            Physics.Simulate(worldDeltaTime);
        }
    }
}