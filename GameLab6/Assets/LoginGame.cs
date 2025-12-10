using Newtonsoft.Json;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class LoginGame : MonoBehaviour
{
    public TMPro.TMP_InputField emailInput;
    public TMPro.TMP_InputField passwordInput;
    public GameObject notification;
    private string baseUrl = "https://localhost:7208";

    IEnumerator Login()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        
        Login.RequestLoginData loginData = new Login.RequestLoginData(email, password);
        string body = JsonConvert.SerializeObject(loginData);

        using (UnityWebRequest www = new UnityWebRequest(baseUrl+"/ap/APIGame/Login", "POST"))
        {
            byte[] bodyraw = System.Text.Encoding.UTF8.GetBytes(body);
            www.uploadHandler = new UploadHandlerRaw(bodyraw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if(www.error != null)
            {
                Login.ResponseLogin response = JsonConvert.DeserializeObject<Login.ResponseLogin>(www.downloadHandler.text);
                notification.SetActive(true);
                notification.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[1].text = response.notification;
            }
            else
            {
                Login.ResponseLogin response = JsonConvert.DeserializeObject<Login.ResponseLogin>(www.downloadHandler.text);
                if (response.isSuccess)
                {
                    PlayerPrefs.SetString("token", response.data.token);
                    PlayerPrefs.SetString("userId", response.data.user.id);
                    PlayerPrefs.SetString("email", response.data.user.email);
                    PlayerPrefs.SetString("name", response.data.user.name);
                    PlayerPrefs.SetString("linkAvatar", baseUrl + "/uploads/avatars/"+ response.data.user.avatar);
                    PlayerPrefs.SetInt("regionId", response.data.user.regionId);
                    notification.SetActive(true);
                    notification.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[1].text = "Login Success";
                }
                else
                {
                    notification.SetActive(true);
                    notification.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[1].text = response.notification;
                }
            }
        }
    }

    public void OnLoginButtonClicked()
    {
        StartCoroutine(Login());
    }
}
