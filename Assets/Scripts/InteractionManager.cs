using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    #region Singleton

    private static InteractionManager _singleton;

    public static InteractionManager Singleton { get { return _singleton; } }

    private void Awake()
    {
        if (_singleton != null && _singleton != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _singleton = this;
        }
    }

    #endregion

    #region UI

    public Text InteractionText;

    #endregion
}
