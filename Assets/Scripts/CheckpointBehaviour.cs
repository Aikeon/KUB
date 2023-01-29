using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject pulsePrefab;
    public float pulseLifetime;
    public float pulseScale;
    public int pulseNumber;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(startPulses());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator startPulses()
    {
        for (int i = 0; i < pulseNumber; i++)
        {
            StartCoroutine(pulse());
            yield return new WaitForSeconds(pulseLifetime/pulseNumber);
        }
    }

    IEnumerator pulse()
    {
        var timeEllapsed = 0f;
        var wave = Instantiate(pulsePrefab,transform.position,transform.rotation);
        var waveRend = wave.GetComponent<Renderer>();
        var waveColor = waveRend.material.color;
        wave.transform.SetParent(transform);
        while (timeEllapsed < pulseLifetime)
        {
            wave.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one + new Vector3((pulseScale-1)/transform.localScale.x, (pulseScale-1)/transform.localScale.y, 1), timeEllapsed/pulseLifetime);
            waveRend.material.color = Color.Lerp(waveColor,new Color(waveColor.r,waveColor.g,waveColor.b,(1 - timeEllapsed/pulseLifetime) * waveColor.a),timeEllapsed/pulseLifetime);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(wave);
        StartCoroutine(pulse());
    }

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "Player")
        {
            collisionInfo.gameObject.transform.parent.TryGetComponent<PlayerControl>(out var pc);
            pc.basePos = transform.position;
            GameManager.Instance.KUBBaseRot = GameManager.Instance.KUB.transform.rotation;
        }
    }
}
