using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;
[Serializable]
public class PlayerInfo
{
    public string NickName;
    public int MaxHealth;
    public int Health;
    public string Pos;
}

public class PlayerController : MonoBehaviour {
    public GameObject explosion;
    public string IPName;//名字
    public string NickName;//昵称
    public int MaxHealth;
    private int _health=10;//生命值
    private bool _isAlive = true;
    public Dictionary<string, GameObject> AllSkill=new Dictionary<string, GameObject>();
    public float speed;
    public SyncHandle syncHandle;
    public Action<string> SendMsgHandle;
    private Slider _healthBar;
    private Text _nameText;
    private Text _curHpText;
    private Text _maxHpText;
    private GameObject showDamage;
    //public CMDHandle destroyHandle;
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
    }
    //private Queue<Vector3> _dirs;
    //private Queue<float> _waits;
    // Use this for initialization
    void Start () {
        _maxHpText= transform.GetChild(2).GetChild(2).GetComponent<Text>();
        _curHpText = transform.GetChild(2).GetChild(4).GetComponent<Text>();
        _healthBar = transform.GetChild(2).GetChild(0).GetComponent<Slider>();
        _nameText = transform.GetChild(2).GetChild(1).GetComponent<Text>();
        _nameText.text = NickName;
        _nameText.color = Color.yellow;
        _health = MaxHealth;
        _curHpText.text = MaxHealth + "";
        _maxHpText.text = MaxHealth + "";
        showDamage = Resources.Load<GameObject>("Prefabs/ShowDamage");
        //AllSkill = new Dictionary<string, GameObject>();
        //_dirs = new Queue<Vector3>();
        //_waits = new Queue<float>();
    }
    private Vector3 _dir;
    public Vector3 Dir;
    public void DamageHandle(int damage)
    {
        if(Login.CurrentMode== PlayMode.Single)
        {
            CauseDamage(damage);
        }
        else{
            string msg = IPName + "#" + "BeDamaged#" + damage.ToString();
            SendMsgHandle.Invoke(msg);
        }
        
    }
    public void CauseDamage(int damage)
    {
        StartCoroutine(BeHitted());
        if (_health - damage > MaxHealth)
        {
            MaxHealth -= damage;
            _maxHpText.text = MaxHealth + "";
        }
        _health -= damage;
        _curHpText.text = _health + "";
        if (_health <= 0) _isAlive = false;
        _healthBar.value = Mathf.Abs((float)_health/MaxHealth);
        //
        GameObject show = ObjectPool.GetInstance().CreateObject(showDamage, "showDamage");
        show.transform.position = transform.position;
        show.transform.GetChild(0).GetComponent<Text>().text = (-damage).ToString();
        //
        if (!IsAlive)
        {
            GameObject ex = ObjectPool.GetInstance().CreateObject(explosion, "explosion1");
            ex.transform.position = transform.position;
            gameObject.SetActive(false);
        }
    }
    public void AddSkill(GameObject skill)
    {
        //GameObject _skill = Instantiate(skill);
        skill.GetComponent<SkillController>().PlayerPosition = transform;
        AllSkill.Add(skill.GetComponent<SkillController>().SkillId, skill);
    }
    public void SetCurrentDir(string dir,string wait)
    {
        
        string[] v3 = dir.Substring(1, dir.Length - 2).Split(',');
        //_dirs.Enqueue( new Vector3(
        //    float.Parse(v3[0]),
        //    float.Parse(v3[1]),
        //    float.Parse(v3[2]) )
        //    );
        _dir = (new Vector3(
           float.Parse(v3[0]),
           float.Parse(v3[1]),
           float.Parse(v3[2]))
           );
        //Dir = _dir;
        time = float.Parse(wait);
        transform.position += _dir.normalized * speed * time;
        //_waits.Enqueue(float.Parse(wait));
        //print(_dir);
    }

    public void ExecuteSkill(string skillID)
    {
        //print(_dir);
        //if (_dir == Vector3.zero)
        //{
        //    return;
        //}
        //GameObject bullet = Instantiate(AllSkill[skillID].GetComponent<SkillController>().Skillbullet);
        //bullet.transform.position = transform.GetChild(1).position;
        //bullet.transform.LookAt(PlayerPosition.GetComponent<PlayerController>().Dir, Vector3.forward);
        //bullet.transform.up = transform.GetComponent<PlayerController>().Dir;
        //bullet.transform.GetComponent<BulletController>().destroyHandle = destroyHandle;
        //bullet.GetComponent<Rigidbody2D>().
        //    AddForce(transform.GetComponent<PlayerController>().Dir.normalized * 20,
        //    ForceMode2D.Impulse);
        //Destroy(bullet, 5f);
        GameObject bullet = ObjectPool.GetInstance().
            CreateObject(AllSkill[skillID].GetComponent<SkillController>().Skillbullet, "bullet" + skillID);
        bullet.GetComponent<BulletController>().InitState(transform.GetChild(1), skillID);
    }

    private IEnumerator BeHitted()
    {
        transform.GetComponent<SpriteRenderer>().color = Color.red;
        yield return 1;
        yield return 1;
        transform.GetComponent<SpriteRenderer>().color = Color.white;
        yield return 1;
    }
    float time = 0;
    //float waitTime = 0;
    bool isSend = false;

   

    //bool isWaitting = false;
    // Update is called once per frame
    void Update () {
        #region
        // _dir = Joystick.GetComponent<JoyStickEvent>().currentDir.normalized;
        //if (!isWaitting && _waits.Count > 0)
        //{

        //    waitTime = _waits.Dequeue();
        //    time = waitTime;
        //    isWaitting = true;
        //}
        //if (waitTime >= 0 || isWaitting)
        //{
        //    waitTime -= Time.deltaTime;
        //    if (waitTime <= 0)
        //    {
        //        if (_dirs.Count > 0) _dir = _dirs.Dequeue();
        //        isWaitting = false;
        //    }

        //}
        //transform.position = Vector3.MoveTowards(
        //    transform.position, 
        //    transform.position + _dir.normalized * speed * time, 
        //    speed*Time.deltaTime);

        //transform.Translate( _dir.normalized*speed*Time.deltaTime);
#endregion
        if (_dir != Vector3.zero)
        {
            isSend = false;
            //print(Dir);
            Dir = _dir;
            transform.up = _dir.normalized;
            //transform.GetChild(0).localPosition = Dir.normalized/2;
            //transform.GetChild(1).localPosition = Dir.normalized*1.5f;
            //transform.GetChild(0).up = Dir.normalized;

            transform.GetChild(2).up = Vector2.up;
            transform.GetChild(2).position = transform.position + new Vector3(0, 2, 0);
        }
        else
        {
            if (syncHandle != null&&!isSend)
            {
                syncHandle();
                isSend = true;
            }
        }

	}
}
