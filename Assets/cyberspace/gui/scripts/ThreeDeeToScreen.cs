using UnityEngine;
using System.Collections;

public class ThreeDeeToScreen : MonoBehaviour {

    public Transform anchor;
    public Camera myCamera;

    private RectTransform rect, canvas;

	// Use this for initialization
	void Start () {
        if (myCamera == null)
            myCamera = Camera.main;

        rect = GetComponent<RectTransform>();
        canvas = transform.parent.GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
        // get the initial position for the element
        Vector2 viewport = myCamera.WorldToViewportPoint(anchor.position);

        Vector2 screenPosition = new Vector2
        (
            //viewport.x * rect.sizeDelta.x,
            //viewport.y * rect.sizeDelta.y

             ((viewport.x * canvas.sizeDelta.x) - (canvas.sizeDelta.x * 0.5f)),
             ((viewport.y * canvas.sizeDelta.y) - (canvas.sizeDelta.y * 0.5f))
        );

        Vector2 velocity = Vector2.zero;
        rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, screenPosition, ref velocity, .015f);
    }
}
