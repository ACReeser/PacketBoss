using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.ImageEffects;

public class RTSCamera : MonoBehaviour {
    public Transform panTransform;

    private Quaternion originalLocalRot;

    private float mouseX = 0f, mouseY = 45f;
    private NoiseAndScratches noise;
    private bool paused = false;

    // Use this for initialization
	void Start () {
        originalLocalRot = this.transform.localRotation;
        this.noise = this.GetComponent<NoiseAndScratches>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        bool togglePaused = Input.GetKeyUp(KeyCode.Escape);

        if (togglePaused)
        {
            paused = !paused;
            this.noise.enabled = paused;
        }

        if (paused)
        {
            return;
        }

        bool rotating = Input.GetMouseButton(1);

        if (rotating)
        {
            mouseX += Input.GetAxis("Mouse X");
            mouseY -= Input.GetAxis("Mouse Y");
            transform.localEulerAngles = new Vector3(mouseY, 0, 0);
            panTransform.localEulerAngles = new Vector3(0, mouseX, 0);
        }
        else if (Input.GetMouseButtonUp(2))
        {
            transform.localRotation = originalLocalRot;
            panTransform.localRotation = Quaternion.identity;
        }

        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0f)
        {
            Camera.main.fieldOfView -= wheel * 10f;
        }

        CheckEdgesToMove(rotating);
    }
    private void CheckEdgesToMove(bool rotating)
    {
        var mousePosX = Input.mousePosition.x;
        var mousePosY = Input.mousePosition.y;
        int scrollDistance = 44;
        float scrollSpeed = 10;

        if (Input.GetKey(KeyCode.A) || (!rotating && mousePosX < scrollDistance))
        {
            _translate(Vector3.left, scrollSpeed);
        }
        if (Input.GetKey(KeyCode.D) || (!rotating && mousePosX >= Screen.width - scrollDistance))
        {
            _translate(Vector3.right, scrollSpeed);
        }

        if (Input.GetKey(KeyCode.S) || (!rotating && mousePosY < scrollDistance))
        {
            _translate(Vector3.back, scrollSpeed);
        }
        if (Input.GetKey(KeyCode.W) || (!rotating && mousePosY >= Screen.height - scrollDistance))
        {
            _translate(Vector3.forward, scrollSpeed);
        }
    }

    private void _translate(Vector3 direction, float scrollSpeed)
    {
        panTransform.Translate(direction * (scrollSpeed * Time.deltaTime), Space.Self);
    }
}
