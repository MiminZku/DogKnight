using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float rotateSpeed;
    float offsetY;
    public float currentZoom;
    float minZoom = 4.0f;
    float maxZoom = 20.0f;
    public float angle = 0.0f;
    double radianAngle;
    private void Start() {
        transform.position = target.position + offset * currentZoom;
        offsetY = offset.y;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            angle = (angle + rotateSpeed * Time.deltaTime) % 360;
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            angle = (angle - rotateSpeed * Time.deltaTime) % 360;
        }
        radianAngle = Mathf.PI * angle / 180.0;
        float x = Mathf.Sin((float)radianAngle);
        float z = Mathf.Sqrt((1 - Mathf.Pow(Mathf.Sin((float)radianAngle), 2)));
        if (angle < 0) {
            if (-270 < angle && angle <= -90) offset.Set(x, offsetY, z);
            else offset.Set(x, offsetY, -z);
        }
        else {
            if (90 <= angle && angle < 270) offset.Set(x, offsetY, z);
            else offset.Set(x, offsetY, -z);
        }
        offset.Normalize();
        // 마우스 휠로 줌 인아웃
        currentZoom -= 3*Input.GetAxis("Mouse ScrollWheel");
        // 줌 최소 및 최대 설정 
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        transform.position = target.position + offset.normalized * currentZoom;
        transform.LookAt(target);
    }
}
