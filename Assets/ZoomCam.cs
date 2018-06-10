using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCam : MonoBehaviour {

    Camera camera;
    public float zoomSpeed = 5;
    public float cameraSpeed = 2;

	// Use this for initialization
	void Start () {
        camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        camera.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * -1 * Time.deltaTime;

        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 0.1f, 0.75f);

        camera.transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.InverseLerp(0, 0.75f, camera.orthographicSize) * cameraSpeed * Time.deltaTime;

        camera.transform.position = new Vector3(Mathf.Clamp(camera.transform.position.x, -1f, 1f), Mathf.Clamp(camera.transform.position.y, -1f, 1f), -1);
	}
}
