using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class NetworkPathResolver {

    public static server FindPlayerAppServer(server origin, out Queue<server> hops)
    {
        List<server> hopList = new List<server>();

        var result = FindPlayerAppServer(origin, origin, hopList);

        if (hopList != null)
        {
            //hopList.Reverse();
            //foreach(server s in hopList)
            //{
            //    UnityEngine.Debug.Log("Packet has next as " + s.Name);
            //}
            //UnityEngine.Debug.Log("Packet has final as " + result.Name);
            hops = new Queue<server>(hopList);
        }
        else
        {
            hops = null;
        }


        return result;
    }

    private static server FindPlayerAppServer(server lastServer, server fromServer, List<server> hops, int depth = 0)
    {
        if (depth > 5)
        {
            UnityEngine.Debug.LogWarning("Max depth pathfinding hit");
            hops = null;
            return null;
        }

        foreach(server next in fromServer.Links.otherEnds)
        {
            if (next == lastServer || next == fromServer)
            {
                continue;
            }
            else if (next.IsPlayerOwned && next.HasApp)
            {
                hops.Add(next);
                return next;
            }
            else
            {
                if (next.Links && next.Links.otherEnds != null)
                {
                    hops.Add(next);

                    server candidate = FindPlayerAppServer(fromServer, next, hops, depth + 1);

                    if (candidate)
                    {
                        return candidate;
                    }
                    else
                    {
                        hops.Remove(next);
                    }
                }
            }
        }

        return null;
    }
}
