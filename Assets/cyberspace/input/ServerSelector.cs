using UnityEngine;
using System.Collections;

public class ServerSelector : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButtonUp(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            RaycastHit hitInfo;

            bool hideAll = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider != null)
                {
                    if (hitInfo.collider.transform.CompareTag("Server"))
                    {
                        server server = hitInfo.collider.GetComponent<server>();
                        if (server == null)
                        {
                            //noop
                        }
                        else
                        {
                            GuiInterface.Instance.ClickServer(server, hitInfo.collider.transform);

                            hideAll = false;
                        }
                    }
                }
            }

            if (hideAll)
            {
                GuiInterface.Instance.DeSelectAll();
            }
        }
	}

    public void PenetrateServer()
    {
        GuiInterface.Instance.ToggleSystemPenetration();
    }
}
