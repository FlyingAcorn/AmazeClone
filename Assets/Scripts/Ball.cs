using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private bool isMoving;
    private float _movementX;
    private float _movementZ;
    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        // test ettin iyi gibi touch controller ile yap
        // iki opsiyonun var ray atmak veya topun collision radiusunu kısıp collision olduğunda onu ortalamak 
        // duvarlara deymemesi için
#if UNITY_EDITOR
        if (!isMoving)
        {
            _movementX = Input.GetAxisRaw("Horizontal");
            _movementZ = Input.GetAxisRaw("Vertical");
            if (Input.GetButtonDown("Horizontal")||Input.GetButtonDown("Vertical"))
            {
                isMoving = true;
            }
        }
        var movementDirection = new Vector3(_movementX, 0, _movementZ);
        transform.position += movementDirection * Time.deltaTime * 5;
#endif
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Block block))
        {
            isMoving = false;
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.TryGetComponent(out Floor floor))
        {
            floor.gameObject.TryGetComponent(out Renderer renderer);
            renderer.material.color = Color.blue;
        }
    }
}
