using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(networkLink))]
[RequireComponent(typeof(server))]
public class client : MonoBehaviour {
    public int ChanceToSendRequestEachTick = 10;
    public Transform packetPrefab;

    private networkLink link;

    private server self, target;
    private Queue<server> hopsToTarget;

    public float TimeToSendPacketSeconds = 1f;

    // Use this for initialization
    void Start()
    {
        this.self = this.GetComponent<server>();
        this.link = this.GetComponent<networkLink>();
        GetFirstPlayerTarget();
        StartCoroutine(SendPacketsOccaisionally());
    }

    private IEnumerator SendPacketsOccaisionally()
    {
        while (this.isActiveAndEnabled)
        {
            float tick = server.BaseTickSeconds;
            bool sentPacket = false;

            if (self.CPUAvailable > 0 && FeelsLikeSendingRequest())
            {
                self.CPUAvailable--;
                sentPacket = true;
                tick = TimeToSendPacketSeconds;
                DDOSAttackTarget();
            }

            yield return new WaitForSeconds(tick);

            if (sentPacket)
            {
                self.CPUAvailable++;
            }
        }
    }

    private bool FeelsLikeSendingRequest()
    {
        return UnityEngine.Random.Range(1, 101) > ChanceToSendRequestEachTick;
    }

    private void GetFirstPlayerTarget()
    {
        if (link)
        {
            target = NetworkPathResolver.FindPlayerAppServer(this.self, out this.hopsToTarget);
        }
    }

    // Update is called once per frame
    //void Update () {
    //}

    private void DDOSAttackTarget()
    {
        Transform packet = (Transform)GameObject.Instantiate(packetPrefab, this.transform.position, Quaternion.LookRotation((target.transform.position - this.transform.position).normalized));
        var packetScript = packet.GetComponent<Packet>();
        packetScript.SetRoute(this.self, this.target, new Queue<server>(this.hopsToTarget));
    }
}
