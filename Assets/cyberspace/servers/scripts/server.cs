using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum MachineState { ON, OFF, REBOOTING}

[RequireComponent(typeof(networkLink))]
public class server : MonoBehaviour {
    private static float timeForPacket = 1f;
    private static int cpuPerPacketBase = 1;
    public static float BaseTickSeconds = .25f;
    private static float RebootSeconds = 3f;

    public string Name;
    public Dictionary<int, bool> HasBeenProbed;
    public bool IsPlayerOwned;
    public bool IsCommand;
    public bool HasExternalLogin;
    public bool IsPlayerZombie;
    public bool IsCompromised;
    public bool HasData;
    public bool IsBricked;
    public bool HasApp;
    public Dictionary<int, int> CrackProgress;
    public Dictionary<int, int> HasZombie;
    public int CPUInstalled; //in GFLOPS 
    public int Vulnerability;
    public bool IsSelected = false;

    internal MachineState State = MachineState.ON;
    internal bool IsOverrequested;
    internal int CPUAvailable; // in GFLOPS
    internal int CPUPercentage
    {
        get
        {
            if (CPUInstalled == 0)
                return 100;
            else
                return (int)((CPUInstalled - CPUAvailable) / CPUInstalled * 100f);
        }
    }

    public networkLink Links;

    void Awake()
    {
        if (CPUInstalled < 1)
            CPUInstalled = 1;

        Links = this.GetComponent<networkLink>();
    }

	// Use this for initialization
	void Start () {
        CPUAvailable = CPUInstalled;
        StartCoroutine(TickLoop());
	}

    private IEnumerator TickLoop()
    {
        while(this.isActiveAndEnabled)
        {
            float waitTime = BaseTickSeconds;

            MachineState thisCycleState = State;

            switch (thisCycleState)
            {
                case MachineState.ON:
                    ProcessPackets(ref waitTime);
                    break;
                case MachineState.REBOOTING:
                    waitTime = RebootSeconds;
                    ClearPacketQueue();
                    CPUAvailable = CPUInstalled;
                    break;
            }

            this.IsOverrequested = packetQueue.Count > CPUInstalled;

            yield return new WaitForSeconds(waitTime);

            switch (thisCycleState)
            {
                case MachineState.ON:
                    AfterProcessPackets();
                    break;
                case MachineState.REBOOTING:
                    State = MachineState.ON;
                    break;
            }
        }
    }

    private void ClearPacketQueue()
    {
        for (int i = packetQueue.Count - 1; i > -1; i--)
        {
            Packet p = packetQueue.Dequeue();
            if (p)
                Destroy(p.gameObject);
        }
        packetQueue.Clear();
        PacketBuffer = new Dictionary<server, List<Packet>>();
    }

    private List<Packet> processedPackets;

    private void ProcessPackets(ref float waitTime)
    {
        processedPackets = null;

        while (CPUAvailable > 0 && packetQueue.Count > 0)
        {
            Packet packet = packetQueue.Dequeue();

            if (packet)
            {
                if (processedPackets == null)
                    processedPackets = new List<Packet>();

                CPUAvailable -= cpuPerPacketBase;
                packet.Processed = true;
                if (!packet.EvilBit)
                    UptimeMonitor.Instance.AddSuccess();

                //waitTime = timeForPacket;
                waitTime = BaseTickSeconds;
                processedPackets.Add(packet);
                RemovePacketFromVisualQueue(packet);
            }
        }
    }

    private void RemovePacketFromVisualQueue(Packet packet)
    {
        PacketBuffer[packet.From].Remove(packet);
        packet.DisableRenderers();
    }

    private void AfterProcessPackets()
    {
        if (processedPackets != null)
        {
            int numOfPacketsProcessed = processedPackets.Count;
            CPUAvailable += numOfPacketsProcessed * cpuPerPacketBase;

            for (int i = numOfPacketsProcessed - 1; i > -1; i--)
            {
                DestroyPacket(processedPackets[i]);
                processedPackets.Remove(processedPackets[i]);
            }

            if (packetQueue.Count > 2 * numOfPacketsProcessed)
            {
                RefreshSpiral();
            }
        }
    }

    private void DestroyPacket(Packet p)
    {
        if (p && p.gameObject)
            Destroy(p.gameObject);
    }

    // Update is called once per frame
    void Update () {
	}

    private struct spiralEntry
    {
        public int spiralStep;
        public int count;
    }

    private Queue<Packet> packetQueue = new Queue<Packet>();
    private Dictionary<server, List<Packet>> PacketBuffer = new Dictionary<server, List<Packet>>();

    public void AddToQueue(Packet p)
    {
        packetQueue.Enqueue(p);
        
        if (!PacketBuffer.ContainsKey(p.From))
        {
            PacketBuffer[p.From] = new List<Packet>();
        }

        int spiralStep = PacketBuffer[p.From].Count;
        PacketBuffer[p.From].Add(p);
        RotatePacketIntoSpiral(p, spiralStep);
    }

    private static void RotatePacketIntoSpiral(Packet p, int spiralStepCount)
    {
        float theta = Mathf.Rad2Deg * (spiralStepCount + 8) * Mathf.PI / 8f;
        Vector3 offset = (Vector3.right * .001f * theta) + (Vector3.left * .1f);
        Vector3 pivot = p.FinalPosition;
        p.transform.rotation = p.MovementRotation;
        p.transform.position = pivot + p.transform.TransformVector(offset);
        p.transform.RotateAround(pivot, Vector3.forward, theta);
    }

    private void RefreshSpiral()
    {
        foreach(server from in PacketBuffer.Keys)
        {
            int index = 0;
            foreach(Packet p in PacketBuffer[from])
            {
                RotatePacketIntoSpiral(p, index);
                index++;
            }
        }
    }

    public void TimeoutPacket(Packet p)
    {
        RemovePacketFromVisualQueue(p);
        DestroyPacket(p);
    }

    public void Reboot()
    {
        if (State == MachineState.ON)
            State = MachineState.REBOOTING;
    }
}
