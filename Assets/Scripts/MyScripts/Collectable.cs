using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{

    private AudioSource _audioSource;
    private MeshRenderer _meshRenderer;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Collected()
    {
        _audioSource.Play();
        _meshRenderer.enabled = false;
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(0.1f);

        _meshRenderer.enabled = true;
        gameObject.SetActive(false);
    }

}