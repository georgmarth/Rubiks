using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {

    public float startfadeAngle = 15f;
    public float endFadeAngle = 30f;

    public float rotateTime = .2f;
    public float scrambleTime = .1f;

    public float angularDrag = 0.5f;
    public float linearDrag = 0.5f;

    #region singleton
    public static Settings Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    #endregion

}
