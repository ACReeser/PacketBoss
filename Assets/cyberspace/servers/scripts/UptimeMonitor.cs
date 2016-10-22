using UnityEngine;
using System.Collections;
using System;

public class UptimeMonitor : MonoBehaviour {
    private const int NumberOfUptimeBuckets = 6;
    private static UptimeMonitor _instance;
    public static UptimeMonitor Instance { get { return _instance; } }

    private class UptimeLog
    {
        public int Attempts;
        public int Successes;
    }


    private UptimeLog[] UptimeHistory = new UptimeLog[NumberOfUptimeBuckets]
    {
        new UptimeLog(), //2.5 minutes ago
        new UptimeLog(), //two minutes ago
        new UptimeLog(), //1.5 minutes ago
        new UptimeLog(), //a minute ago
        new UptimeLog(), //30s ago
        new UptimeLog(), //now
    };

    private int CurrentAverage = 100;
    private const int HappyThreshold = 75, NeutralThreshold = 50;

    void Awake()
    {
        _instance = this;
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(SetUptime());
	}

    private IEnumerator SetUptime()
    {
        while(isActiveAndEnabled)
        {
            yield return new WaitForSeconds(5f);

            CurrentAverage = GetAverage();
            RefreshUI();
            ShiftLeftAndBlankNewest();
        }
    }

    private void RefreshUI()
    {
        bool happy = (CurrentAverage >= HappyThreshold), neutral = (CurrentAverage < HappyThreshold && CurrentAverage >= NeutralThreshold);
        GuiInterface.Instance.happy.enabled = happy;
        GuiInterface.Instance.neutral.enabled = neutral;
        GuiInterface.Instance.sad.enabled = (CurrentAverage < NeutralThreshold);

        string color = "red";
        if (happy)
            color = "#33FF00FF";
        else if (neutral)
            color = "yellow";

        GuiInterface.Instance.statusRight.text = String.Format("<color='{1}'>{0}% Uptime</color>", CurrentAverage, color);
    }

    private int GetAverage()
    {
        int attempts = 0, successes = 0;
        for (int i = 0; i < UptimeHistory.Length; i++)
        {
            attempts += UptimeHistory[i].Attempts;
            successes += UptimeHistory[i].Successes;
        }
        return Math.Min((int)((float)successes / Math.Max(attempts, 1) * 100f), 100);
    }

    private void ShiftLeftAndBlankNewest()
    {
        var firstEntry = UptimeHistory[0];
        var newArray = new UptimeLog[NumberOfUptimeBuckets];
        Array.Copy(UptimeHistory, 1, newArray, 0, NumberOfUptimeBuckets - 1);
        UptimeHistory = newArray;
        firstEntry.Attempts = 0;
        firstEntry.Successes = 0;
        UptimeHistory[NumberOfUptimeBuckets - 1] = firstEntry;
    }

    public void AddAttempt()
    {
        UptimeHistory[NumberOfUptimeBuckets - 1].Attempts++;
    }

    public void AddSuccess()
    {
        UptimeHistory[NumberOfUptimeBuckets - 1].Successes++;
    }

    // Update is called once per frame
    //void Update () {
    //
    //}
}
