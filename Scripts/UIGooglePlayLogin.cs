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
        PlayGamesPlatform.DebugLogEnabled = debugLogEnabled;
#endif
    }

    public void OnClickGooglePlayLogin()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
            switch (status)
            {
                case SignInStatus.InternalError:
                    onLoginFail.Invoke("Internal Errror, cannot Login with Google Play.");
                    break;
                case SignInStatus.Canceled:
                    onLoginFail.Invoke("Login with Google Play was cancelled.");
                    break;
                default:
                    // When google play login success, send login request to server
                    PlayGamesPlatform.Instance.RequestServerSideAccess(false, (idToken) =>
                    {
                        RequestGooglePlayLogin(idToken);
                    });
                    break;
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
