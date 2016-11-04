using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Macrotimer : MonoBehaviour {

    private static Macrotimer _instance;
    public static Macrotimer Instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
    }

    public Text Display;
    private DateTime InUniverseDateTime;

    // Use this for initialization
    void Start () {
        InUniverseDateTime = new DateTime(DateTime.Now.Date.Ticks);
        InUniverseDateTime = InUniverseDateTime.AddHours(7);
        StartCoroutine(MacroTick());
	}

    private IEnumerator MacroTick()
    {
        while(this.isActiveAndEnabled)
        {
            yield return new WaitForSeconds(1f);
            InUniverseDateTime = InUniverseDateTime.AddMinutes(1f);
            RefreshDisplay();
        }
    }

    private void RefreshDisplay()
    {
        Display.text = InUniverseDateTime.ToString("yyyy/MM/dd HH:mm");
    }

    // Update is called once per frame
    void Update () {
	
	}
}
