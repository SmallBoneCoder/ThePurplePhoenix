using UnityEngine;
using FSMFrame;
using UnityEngine.UI;
using System;
using System.Collections;

[Serializable]
public class EnemyInfo
{
    public string Name;
    public int MaxHealth;
    public int Health;
    public string Pos;
    public int PatrolId;
}
public class EnemyController : MonoBehaviour {
    public Action<Vector3> ExplosionHandle;
    public Action<GameObject> ArticleHandle;
    public string EnemyName;
    public int MaxHealth=10;
    public int Health{ get { return _health; } }
    public float Sight = 10;
    public float Speed = 1;
    public Transform Target;
    public Transform PatrolPoint;//巡逻点，也是怪物出生的点
    public GameObject bullet;//子弹
    private int _health;
    private StateMachine<EnemyController> _fsm;
    private Slider _healthBar;
    private Text _nameText;
    public GameObject showDamage;
    public void CauseDamage(int damage)
    {
        StartCoroutine(BeHitted());
        _health -= damage;
        _healthBar.value = Mathf.Abs((float)_health / MaxHealth);
        GameObject show = ObjectPool.GetInstance().CreateObject(showDamage, "showDamage");
        show.transform.position = transform.position;
        show.transform.GetChild(0).GetComponent<Text>().text = (-damage).ToString();
        if (_health <= 0)
        {
            ExplosionHandle.Invoke(transform.position);
            ArticleHandle.Invoke(gameObject);
            PatrolPoint.GetComponent<PatrolController>().killedCount++;
            PatrolPoint.GetComponent<PatrolController>().currentCount--;
            //Destroy(gameObject);
            ObjectPool.GetInstance().DestroyObject(gameObject, EnemyName);
        }
    }
    public void InitEnemy(EnemyInfo e,Transform patrol)
    {
        MaxHealth = e.MaxHealth;
        _health = e.Health;
        _healthBar.value = Mathf.Abs((float)_health / MaxHealth);
        _nameText.text = e.Name;
        string[] v3 = e.Pos.Substring(1, e.Pos.Length - 2).Split(',');
         transform.position= new Vector3(
            float.Parse(v3[0]),
            float.Parse(v3[1]),
            float.Parse(v3[2]));
        PatrolPoint = patrol;
    }
    public string GetEnemyJson()
    {
        EnemyInfo e = new EnemyInfo();
        e.Health = _health;
        e.MaxHealth = MaxHealth;
        e.Name = name;
        e.PatrolId = PatrolPoint.GetComponent<PatrolController>().Id;
        e.Pos = transform.position.ToString("F3");
        string json = JsonUtility.ToJson(e);
        return json;
    }
    private IEnumerator BeHitted()
    {
        transform.GetComponent<SpriteRenderer>().color = Color.red;
        yield return 1;
        yield return 1;
        transform.GetComponent<SpriteRenderer>().color = Color.white;
        yield return 1;
    }
    private void OnDisable()
    {
        _health = MaxHealth;
        _healthBar.value = 1;
        transform.GetComponent<SpriteRenderer>().color = Color.white;
    }
    // Use this for initialization
    void Start () {
        _health = MaxHealth;
        _fsm = StateMachineController.GetInstance().createEnemyFSM(this);
        _healthBar = transform.GetChild(0).GetChild(0).GetComponent<Slider>();
        _nameText = transform.GetChild(0).GetChild(1).GetComponent<Text>();
        _nameText.text = EnemyName;
        _nameText.color = Color.white;
        showDamage = Resources.Load<GameObject>("Prefabs/ShowDamage");
    }
	
	// Update is called once per frame
	void Update () {
        _fsm.doTick();
        transform.GetChild(0).up = Vector2.up;
        transform.GetChild(0).position = transform.position + new Vector3(0, 1, 0);
    }
}
