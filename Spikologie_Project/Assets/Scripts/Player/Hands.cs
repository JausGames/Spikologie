using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    [SerializeField] private float xCoeff = 0.1f;
    [SerializeField] private float yCoeff = 0.05f;

    public void Move(Vector2 direction)
    {
        transform.localPosition = new Vector3(Mathf.Abs(direction.x) * xCoeff, direction.y * yCoeff, transform.localPosition.z);
    }
}
