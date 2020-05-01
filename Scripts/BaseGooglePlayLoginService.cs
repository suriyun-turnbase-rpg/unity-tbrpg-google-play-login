using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGooglePlayLoginService : MonoBehaviour
{
    public abstract void LoginWithGooglePlay(string idToken, UnityAction<PlayerResult> onFinish);
}
