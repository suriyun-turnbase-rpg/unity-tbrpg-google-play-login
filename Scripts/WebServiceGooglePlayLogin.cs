using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WebServiceClient))]
public class WebServiceGooglePlayLogin : BaseGooglePlayLoginService
{
    private WebServiceClient webServiceClient;

    private void Awake()
    {
        webServiceClient = GetComponent<WebServiceClient>();
    }

    public override void LoginWithGooglePlay(string idToken, UnityAction<PlayerResult> onFinish)
    {
        var dict = new Dictionary<string, object>();
        dict.Add("idToken", idToken);
        webServiceClient.PostAsDecodedJSON<PlayerResult>("/login-with-google-play", (www, result) =>
        {
            onFinish(result);
        }, dict);
    }
}
