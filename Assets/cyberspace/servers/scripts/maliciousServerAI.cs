using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(networkLink))]
[RequireComponent(typeof(server))]
public class maliciousServerAI : MonoBehaviour {
    public Transform evilPacket;

    private networkLink link;

    private server self, target;
    public float TimeToSendPacketSeconds = 1f;
    private Queue<server> hopsToTarget;

    // Use this for initialization
    void Start ()
    {
        this.self = this.GetComponent<server>();
        this.link = this.GetComponent<networkLink>();
        GetFirstPlayerTarget();
        StartCoroutine(SendBadPackets());
    }

    private IEnumerator SendBadPackets()
    {
        while (this.isActiveAndEnabled)
        {
            float tick = server.BaseTickSeconds;
            bool sentPacket = false;

            if (target != null && self.CPUAvailable > 0)
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
        Transform packet = (Transform)GameObject.Instantiate(evilPacket, this.transform.position, Quaternion.identity);
        var packetScript = packet.GetComponent<EvilPacket>();
        packetScript.SetRoute(this.self, this.target, new Queue<server>(this.hopsToTarget));
    }
}
