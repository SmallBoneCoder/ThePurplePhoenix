using UnityEngine;
using BehaviorTreeFrame;
using UnityEngine.UI;
using System;
using System.Collections;
[Serializable]
public class BossInfo
{
    public string Name;
    public int MaxHealth;
    public int Health;
    public string Pos;
    public int PatrolId;
}
public class BossController : MonoBehaviour {
    public Action<Vector3> ExplosionHandle;
    public Action<GameObject> ArticleHandle;
    public GameObject NormalBullet;
    public GameObject Lazer1;
    public GameObject Lazer2;
    public Transform PatrolPoint;//巡逻点
    public float Sight = 5;
    public float Speed = 3;
    public GameObject[] bullets;//子弹
    public string BossName;//名称
    public int MaxHealth = 500;//最大生命值
    public int Health { get { return _health; } }
    private int _health;//当前生命
    public Transform Target;//目标
    private Slider _healthBar;//血条
    private Text _nameText;//名字文本框
    BTNode<BossController> _bevTree;
    public Transform Head;
    public GameObject showDamage;
    // Use this for initialization
    void Start () {
        Head = transform.GetChild(1);
        _health = MaxHealth;
        _healthBar = transform.GetChild(0).GetChild(0).GetComponent<Slider>();
        _nameText = transform.GetChild(0).GetChild(1).GetComponent<Text>();
        _nameText.text = BossName;
        _bevTree = BevTreeFactory.GetInstance().CreateBevTree_Boss(this);
        showDamage = Resources.Load<GameObject>("Prefabs/ShowDamage");
    }
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
            PatrolPoint.GetComponent<PatrolController>().BossCount--;
            //Destroy(gameObject);
            ObjectPool.GetInstance().DestroyObject(gameObject, BossName);
        }
    }
    private IEnumerator BeHitted()
    {
        transform.GetComponent<SpriteRenderer>().color = Color.red;
        yield return 1;
        yield return 1;
        transform.GetComponent<SpriteRenderer>().color = Color.white;
        yield return 1;
    }
    public void InitBoss(BossInfo b, Transform patrol)
    {
        MaxHealth = b.MaxHealth;
        _health = b.Health;
        _healthBar.value = Mathf.Abs((float)_health / MaxHealth);
        _nameText.text = b.Name;
        string[] v3 = b.Pos.Substring(1, b.Pos.Length - 2).Split(',');
        transform.position = new Vector3(
           float.Parse(v3[0]),
           float.Parse(v3[1]),
           float.Parse(v3[2]));
        PatrolPoint = patrol;
    }
    public string GetBossJson()
    {
        BossInfo b = new BossInfo();
        b.Health = _health;
        b.MaxHealth = MaxHealth;
        b.Name = name;
        b.PatrolId = PatrolPoint.GetComponent<PatrolController>().Id;
        b.Pos = transform.position.ToString("F3");
        string json = JsonUtility.ToJson(b);
        return json;
    }
    // Update is called once per frame
    void Update () {
        
        _bevTree.DoTick();//执行行为树
        transform.GetChild(0).up = Vector2.up;
        transform.GetChild(0).position = transform.position + new Vector3(0, 5, 0);
    }
    private void OnDisable()
    {
        _health = MaxHealth;
        _healthBar.value = 1;
        transform.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
