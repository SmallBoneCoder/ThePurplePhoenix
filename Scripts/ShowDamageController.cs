using UnityEngine;
using System.Collections;

public class ShowDamageController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
    private void OnEnable()
    {
        StartCoroutine(DestroyMe());
    }
    private IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(0.5f);
        transform.localScale = new Vector3(1.5f, 1.5f, 1);
        GetComponent<Canvas>().sortingOrder = 1;
        yield return new WaitForSeconds(0.5f);
        GetComponent<Canvas>().sortingOrder = 0;
        transform.localScale = Vector3.one;
        ObjectPool.GetInstance().DestroyObject(gameObject, "showDamage");
    }
	// Update is called once per frame
	void Update () {
        transform.position += Vector3.up * Time.deltaTime;
	}
}
