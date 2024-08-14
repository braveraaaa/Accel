using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "OverDriverData", menuName = "ScriptableObjects/OverDriverData", order = 1)]
public class OverDriverData : ScriptableObject
{
    public string id;
    public float odPower;
    public float odTime;
    public float odCoolTime;
    public float speed;
    public float legPower;
    public float leg_interval;
}

public class DataManager : MonoBehaviour
{
private Dictionary<string, OverDriverData> dataDictionary = new Dictionary<string, OverDriverData>();

    public AssetReference[] dataReferences; // Inspector で設定する Addressable Asset の参照リスト

    private void Start()
    {
        foreach (var reference in dataReferences)
            reference.LoadAssetAsync<OverDriverData>().Completed += OnDataLoaded;
    }
    private void OnDataLoaded(AsyncOperationHandle<OverDriverData> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            OverDriverData data = handle.Result;
            if (!dataDictionary.ContainsKey(data.id))
                dataDictionary.Add(data.id, data);
        }
        else
            Debug.LogError("Failed to load OverDriverData.");
    }

    public OverDriverData GetDataById(string id)
    {
        if (dataDictionary.TryGetValue(id, out OverDriverData data))
            return data;
        return null;
    }
}