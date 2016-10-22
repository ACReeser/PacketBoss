using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Packet : MonoBehaviour
{
    private const float PacketTimeout = 10f;
    public server Origin;
    public server From
    {
        get
        {
            return from;
        }
    }

    internal bool EvilBit = false;
    internal bool Processed = false;

    private float legTime = 0f;
    private float legDuration = 2f;

    private Queue<server> hops;
    private server finalDestination;
    private server from, to;
    private bool isMoving = false;
    //to.transform isn't always where we want to end up
    //see CalcToPosition
    private Vector3 toPosition;
    internal Vector3 FinalPosition
    {
        get
        {
            return toPosition;
        }
    }
    internal Quaternion MovementRotation;

    private bool Delivered = false;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Timeout());
    }

    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(PacketTimeout);

        if (this.Delivered)
        {
            if (!this.Processed)
                this.finalDestination.TimeoutPacket(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            this.transform.position = Vector3.Lerp(from.transform.position, toPosition, legTime);
            legTime += Time.deltaTime * legDuration;

            if (legTime > legDuration)
            {
                if (to == finalDestination)
                {
                    this.Delivered = true;
                    this.transform.position = toPosition;
                    finalDestination.AddToQueue(this);
                    isMoving = false;
                    //I've found that too many active trail renderers will cause new ones to stop rendering
                    this.transform.GetChild(0).GetComponent<TrailRenderer>().enabled = false;
                }
                else
                {
                    from = to;
                    to = hops.Dequeue();
                    legTime = 0f;
                    CalcToPositionAndRotation();
                }
            }
        }
    }

    public void SetRoute(server origin, server target, Queue<server> hops)
    {
        this.finalDestination = target;
        this.Origin = origin;
        this.from = origin;
        this.to = hops.Dequeue();
        this.hops = hops;

        CalcToPositionAndRotation();

        isMoving = true;

        if (!EvilBit)
            UptimeMonitor.Instance.AddAttempt();
    }

    private void CalcToPositionAndRotation()
    {
        //we want to end up slightly outside the destination 
        if (this.to == this.finalDestination)
        {
            toPosition = to.transform.position - (to.transform.position - from.transform.position).normalized;
        }
        else //we want to go through things that are just stops on the way
        {
            toPosition = to.transform.position;
        }

        this.transform.rotation = Quaternion.LookRotation((toPosition - from.transform.position).normalized);
        this.MovementRotation = this.transform.rotation;
    }

    public void DisableRenderers()
    {
        foreach(Renderer r in this.gameObject.GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
    }
}
