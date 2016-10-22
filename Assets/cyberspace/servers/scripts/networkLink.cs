using UnityEngine;
using System.Collections;

public class networkLink : MonoBehaviour {
    public static Transform _edgeParent;
    public static Transform EdgeParent
    {
        get
        {
            if (_edgeParent == null)
                _edgeParent = GameObject.Find("Edges").transform;

            return _edgeParent;
        }
    }

    public server[] otherEnds;
    public Material pipeMat;

    
	// Use this for initialization
	void Start () {
        if (otherEnds != null)
        {
            for (int i = 0; i < otherEnds.Length; i++)
            {
                if (EdgeParent.FindChild(this.transform.GetInstanceID()+"_to_"+otherEnds[i].GetInstanceID()) != null ||
                    EdgeParent.FindChild(otherEnds[i].GetInstanceID()  + "_to_" + this.transform.GetInstanceID()) != null)
                {

                }
                else
                {
                    var edge = new GameObject(this.transform.GetInstanceID() + "_to_" + otherEnds[i].GetInstanceID());
                    edge.transform.SetParent(EdgeParent);
                    var lineRenderer = edge.AddComponent<LineRenderer>();
                    lineRenderer.material = pipeMat;
                    lineRenderer.SetVertexCount(2);
                    lineRenderer.SetPositions(new Vector3[2] { this.transform.position, otherEnds[i].transform.position });
                    lineRenderer.SetWidth(.2f, .2f);
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
