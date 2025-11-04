using UnityEngine;
using System;
using System.Collections;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance { get; private set; }

    [Header("BPM (beats per minute)")]
    public float bpm = 120f; // ajusta esto para casar con tu música
    public bool startOnAwake = true;

    public float BeatInterval => 60f / Mathf.Max(1f, bpm);
    public event Action OnBeat; // se llama al final de cada intervalo

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        if (startOnAwake) StartCoroutine(BeatLoop());
    }

    public void SetBPM(float newBpm)
    {
        bpm = Mathf.Max(1f, newBpm);
    }

    IEnumerator BeatLoop()
    {
        // Pequeño delay para sincronización visual si quieres
        yield return null;
        while (true)
        {
            yield return new WaitForSeconds(BeatInterval);
            OnBeat?.Invoke();
        }
    }
}
