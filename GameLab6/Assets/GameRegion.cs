using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class GameRegion : MonoBehaviour
{
    public TMP_Dropdown dropdownRegion;
    private List<Region.RegionData> regions;
    public static int selectedRegionId;

    private IEnumerator GetRegion()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://localhost:7208/api/APIGame/GetAllRegions"))
        {
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                Region.Response response = JsonConvert.DeserializeObject<Region.Response>(json);
                dropdownRegion.SetValueWithoutNotify(0);
                dropdownRegion.RefreshShownValue();
                DropdownValueChanged(dropdownRegion);
            }
        }
    }

    private void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        int index = dropdown.value;
        if(index < 0 || index >= regions.Count)
        {
            Debug.LogWarning("Invalid dropdown index selected");
            return;
        }
        selectedRegionId = regions[index].regionId;
        Debug.Log("Selected Region ID: " + selectedRegionId);
    }

    private void Start()
    {
        StartCoroutine(GetRegion());
        dropdownRegion.onValueChanged.AddListener(delegate {DropdownValueChanged(dropdownRegion); });
    }
}
