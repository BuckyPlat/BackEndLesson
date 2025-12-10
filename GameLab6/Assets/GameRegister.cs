using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using System.Collections;
using Register;
using UnityEngine.Networking;
using System.Linq;

public class GameRegister : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField nameInput;
    public GameObject notification;
    public static int selectedRegionId;

    IEnumerator Register()
    {
        selectedRegionId = GameRegion.selectedRegionId;
        RegisterRequestData requestData = new RegisterRequestData(
            emailInput.text,
            passwordInput.text,
            nameInput.text, "",
            selectedRegionId
            );

        string body = JsonUtility.ToJson( requestData );
        if(emailInput.text == ""|| passwordInput.text == ""|| nameInput.text == "")
        {
            notification.SetActive(true);
            notification.GetComponentsInChildren<TMP_Text>()[1].text = "Vui lòng nhập đầy đủ thông tin!";
            yield break;
        }
        using (UnityWebRequest www = new UnityWebRequest("https://localhost:7208/api/APIGame/Register", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                string responseJson = www.downloadHandler.text;
                Debug.Log(responseJson);
                ResponseUserError responseErr = JsonConvert.DeserializeObject<ResponseUserError>(responseJson);
                if(responseErr != null && responseErr.data != null && responseErr.data.Count > 0)
                {
                    notification.SetActive(true);
                    var textComponents = notification.GetComponentsInChildren<TMP_Text>();

                    string error = string.Join(", ", responseErr.data.Select(e => e.description));

                    if(textComponents.Length > 1)
                    {
                        textComponents[1].text = error;
                    }
                    else
                    {
                        string json = www.downloadHandler.text;
                        ResponseUserSuccess response = JsonConvert.DeserializeObject<ResponseUserSuccess>(json);
                        var data = response.data;
                        if (response.isSuccess)
                        {
                            notification.SetActive(true);
                            notification.GetComponentsInChildren<TMP_Text>()[1].text =
                                "Đăng ký thành công, vui lòng quay lại trang và đăng nhập!" + data.name;
                        }
                    }
                } 
            }
        }
        
    }

    public void OnButtonClickRegister()
    {
        StartCoroutine(Register());
    }
}
