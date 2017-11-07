using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWorksSpawner : MonoBehaviour {

    public GameObject fireworks;

    private GameObject instance;

    public void Spawn()
    {
        if (instance == null)
        {
            instance = Instantiate<GameObject>(fireworks, transform);
        }
    }
}
