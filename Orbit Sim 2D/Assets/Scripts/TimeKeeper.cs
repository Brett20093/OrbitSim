using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour
{
    #region Singleton
    public static TimeKeeper instance;
    private void Awake() {
        if (instance != null) {
            Debug.LogWarning("More than one instance of TimeKeeper found!");
            return;
        }
        instance = this;
    }
    #endregion
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        time = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * Globals.timeMultiplier;
    }
}
