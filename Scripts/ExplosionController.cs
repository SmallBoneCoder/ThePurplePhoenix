using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour {
	// Use this for initialization
	void Start () {
	    
	}
    private void OnEnable()
    {
        StartCoroutine(DestroyMe());
    }
    private IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(1.2f);
        ObjectPool.GetInstance().DestroyObject(gameObject, "explosion1");
    }
	// Update is called once per frame
	void Update () {
	
	}
}
