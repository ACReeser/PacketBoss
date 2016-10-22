using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EvilPacket : Packet {
    void Awake()
    {
        EvilBit = true;
    }
}
