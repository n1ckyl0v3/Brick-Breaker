﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public enum ballState
    {
        aim,
        fire,
        wait,
        endShot,
    }

    public ballState currentBallState;
    public Rigidbody2D rbBall;
    private Vector2 mouseStartPosition;
    private Vector2 mouseEndPosition;
    private float ballVelocityX;
    private float ballVelocityY;
    public float constantSpeed;
    public GameObject arrowPrefab;
    public LineRenderer lineRenderer; // LineRenderer của Ball
    public Transform ballTransform;
    public LayerMask collisionLayerMask;
    public GameObject arrowHeadPrefab;
    private GameObject arrowHeadInstance;
    public float circleOffset = 0.2f;

    private void Awake()
    {
        lineRenderer = arrowPrefab.GetComponent<LineRenderer>(); // Lấy LineRenderer của Ball

        arrowHeadInstance = Instantiate(arrowHeadPrefab); // Khởi tạo hình tròn
        arrowHeadInstance.SetActive(false); // Ẩn nó ban đầu
    }

    private void Start()
    {
        currentBallState = ballState.aim;
    }

    private void Update()
    {
        switch (currentBallState)
        {
            case ballState.aim:
                if (Input.GetMouseButtonDown(0))
                {
                    MouseClicked();
                }
                if (Input.GetMouseButton(0))
                {
                    MouseDragged();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    ReleaseMouse();
                }
                break;

            case ballState.fire:
                break;

            case ballState.wait:
                break;

            case ballState.endShot:
                break;

            default:
                break;
        }
    }

    public void MouseClicked()
    {
        mouseStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lineRenderer.enabled = true; // Hiển thị line
        arrowHeadInstance.SetActive(true); // Hiển thị hình tròn
    }

    public void MouseDragged()
    {
        Vector2 tempMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = (mouseStartPosition - tempMousePosition).normalized;
        RaycastHit2D hit = Physics2D.Raycast(
            ballTransform.position,
            direction,
            Mathf.Infinity,
            collisionLayerMask
        );

        Vector3 lineEndPosition;

        lineRenderer.SetPosition(0, ballTransform.position);

        if (hit.collider != null)
        {
            lineEndPosition = hit.point;
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineEndPosition = ballTransform.position + (Vector3)(direction * 10);
            lineRenderer.SetPosition(1, ballTransform.position + (Vector3)(direction * 10));
        }

        // Cập nhật vị trí của hình tròn
        Vector3 adjustedPosition = lineEndPosition - (Vector3)(direction * circleOffset);
        arrowHeadInstance.transform.position = adjustedPosition;
        arrowHeadInstance.transform.rotation = Quaternion.Euler(
            0f,
            0f,
            -Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x)
        );
    }

    public void ReleaseMouse()
    {
        mouseEndPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ballVelocityX = mouseStartPosition.x - mouseEndPosition.x;
        ballVelocityY = mouseStartPosition.y - mouseEndPosition.y;

        Vector2 tempVelocity = new Vector2(ballVelocityX, ballVelocityY).normalized;
        rbBall.velocity = constantSpeed * tempVelocity;

        if (rbBall.velocity == Vector2.zero)
        {
            return;
        }
        currentBallState = ballState.fire;

        arrowHeadInstance.SetActive(false);
        lineRenderer.enabled = false;
    }
}
