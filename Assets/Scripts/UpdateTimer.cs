using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateTimer : MonoBehaviour {

    private TextMeshProUGUI timerText;

	// Use this for initialization
	void Start () {
        timerText = GetComponent<TextMeshProUGUI>();
	}
	
	// Update is called once per frame
	void Update () {
        int mins = ((int) PlayManager.Instance.timer) / 60;
        int secs = ((int)PlayManager.Instance.timer) % 60;
        int mills = (int)((PlayManager.Instance.timer % 1f) * 1000);

        timerText.text =string.Format("Timer: {0:00}:{1:00}:{2:000}", mins, secs, mills);
	}
}
