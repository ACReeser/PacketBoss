using UnityEngine;
using System.Collections;
using UnityEngine.UI.Extensions;

public class ThreeDeeToLineRenderer : MonoBehaviour {

    public Transform anchorTransform;
    public Camera myCamera;
    public RectTransform anchorRectangle;
    public bool rightAlign = false;

    private RectTransform canvas;
    private UILineRenderer line;

    // Use this for initialization
    void Start()
    {
        if (myCamera == null)
            myCamera = Camera.main;

        line = GetComponent<UILineRenderer>();
        canvas = transform.parent.GetComponent<RectTransform>();
        lastScreenPosition = GetScreenPosition();
    }

    private Vector2 lastScreenPosition;

    // Update is called once per frame
    void Update()
    {
        // get the initial position for the element
        Vector2 screenPosition = GetScreenPosition();

        Vector2 velocity = Vector2.zero;

        Vector2 uiPosition = anchorRectangle.anchoredPosition;
        Vector2 offset = new Vector2((anchorRectangle.sizeDelta.x / 2) - line.LineThickness / 2, 0);

        if (rightAlign)
        {
            uiPosition = uiPosition + new Vector2(canvas.sizeDelta.x, 0) - offset;
        }
        else
        {
            uiPosition += offset;
        }

        Vector2 finalPosition = Vector2.SmoothDamp(lastScreenPosition, screenPosition, ref velocity, .015f);
        Vector2 midPosition = new Vector2(uiPosition.x, finalPosition.y);

        line.Points = new Vector2[] {
            uiPosition,
            midPosition,
            finalPosition
        };

        line.SetVerticesDirty();

        lastScreenPosition = finalPosition;
    }

    private Vector2 GetScreenPosition()
    {
        Vector2 viewport = myCamera.WorldToViewportPoint(anchorTransform.position);
        
        Vector2 screenPosition = new Vector2
        (
             (viewport.x * canvas.sizeDelta.x), 
             ((1 - viewport.y) * -canvas.sizeDelta.y)
        );
        return screenPosition;
    }
}
