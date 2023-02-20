using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float gravParameter = 398.6f; // (km*10)^3 * s^-2
    public float radius = 637.81f; // km*10
    // Start is called before the first frame update

    void Start()
    {
        transform.localScale = new Vector2(radius * 2.0f, radius * 2.0f);
    }
}
