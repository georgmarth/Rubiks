using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour {

    #region Singleton

    public static SoundManager Instance { get; private set; }

    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #endregion

    public AudioSource fX;

    public AudioClip[] cubeSound;

    public AudioClip explosion;

    public AudioClip fireWorks;

    public void PlayCubeSound()
    {
        AudioClip clip = cubeSound[Random.Range(0, cubeSound.Length)];
        fX.clip = clip;
        fX.Play();
    }

    public void PlayExplosion()
    {
        fX.clip = explosion;
        fX.Play();
    }

    public void PlayFireWorks()
    {
        fX.clip = fireWorks;
        fX.loop = true;
        fX.Play();
    }

}
