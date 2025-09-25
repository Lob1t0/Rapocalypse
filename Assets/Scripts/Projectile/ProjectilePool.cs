using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;

    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private int poolSize = 30;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(proyectilPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetProjectile(Vector3 position, Quaternion rotation, float scale = 1f)
    {
        if (pool.Count == 0)
        {
            GameObject obj = Instantiate(proyectilPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        GameObject proj = pool.Dequeue();
        proj.transform.position = position;
        proj.transform.rotation = rotation;
        proj.transform.localScale = Vector3.one * scale;
        proj.SetActive(true);

        return proj;
    }

    public void ReturnProjectile(GameObject proyectil)
    {
        proyectil.SetActive(false);
        pool.Enqueue(proyectil);
    }
}