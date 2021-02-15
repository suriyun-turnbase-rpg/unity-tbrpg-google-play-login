using UnityEngine;
using UnityEngine.Events;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class UIGooglePlayLogin : MonoBehaviour
{
    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }
    public UnityEvent onLoginSuccess;
    public UnityEvent onLoginCancelled;
    public StringEvent onLoginFail;
    public bool debugLogEnabled;

    private void Start()
    {
#if UNITY_ANDROID
        var builder = new PlayGamesClientConfiguration.Builder()
            // requests the email address of the player be available.
            // Will bring up a prompt for consent.
            .RequestEmail()
            // requests a server auth code be generated so it can be passed to an
            //  associated back end server application and exchanged for an OAuth token.
            .RequestServerAuthCode(false)
            // requests an ID token be generated.  This OAuth token can be used to
            //  identify the player to other services such as Firebase.
            .RequestIdToken();
        var config = builder.Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = debugLogEnabled;
#endif
    }

    public void OnClickGooglePlayLogin()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.SignOut();
        PlayGamesPlatform.Instance.Authenticate((success, message) => {
            if (success)
            {
                // When google play login success, send login request to server
                var idToken = PlayGamesPlatform.Instance.GetIdToken();
                RequestGooglePlayLogin(idToken);
            }
            else
            {
                // Show error message
                onLoginFail.Invoke(message);
            }
        });
#else
            Debug.Log("Only Android can login with Google Play");
#endif
    }

    private void RequestGooglePlayLogin(string idToken)
    {
        BaseGooglePlayLoginService loginService = FindObjectOfType<BaseGooglePlayLoginService>();
        loginService.LoginWithGooglePlay(idToken, (result) =>
        {
            if (result.Success)
            {
                GameInstance.Singleton.OnGameServiceLogin(result);
                onLoginSuccess.Invoke();
            }
            else
            {
                onLoginFail.Invoke(result.error);
            }
        });
    }
}
