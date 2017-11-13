using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PatrolController : MonoBehaviour {
    public int Id;
    public int maxCount = 10;
    public int currentCount;
    public int killedCount;
    public int BossCount;
    public GameObject enemy;
    public GameObject boss;
    public GameObject star;
    public GameObject explosion;
    public Text Txt_Stars;
    public System.Action<string> SendMsgHandle;
    private static int _totalScore;
	// Use this for initialization
	void Start () {
	
	}
    float time = 1f;
    public void CreateEnemy()
    {
        //GameObject e = Instantiate(enemy);
        GameObject e = ObjectPool.GetInstance().CreateObject(enemy, "杂鱼" + "(" + Id + ")");
        e.transform.position = transform.position + new Vector3(1, 1, 0);
        e.GetComponent<EnemyController>().PatrolPoint = transform;
        e.GetComponent<EnemyController>().EnemyName = "杂鱼"+"(" + Id + ")";
        e.GetComponent<EnemyController>().ArticleHandle = CreateArticle;
        e.GetComponent<EnemyController>().ExplosionHandle = CreateExplosion;
    }
    public void CreateBoss()
    {
        //GameObject b = Instantiate(boss);
        GameObject b = ObjectPool.GetInstance().CreateObject(boss, "蛇皮怪" + "(" + Id + ")");
        b.transform.position = transform.position + new Vector3(1, 1, 0);
        b.GetComponent<BossController>().PatrolPoint = transform;
        b.GetComponent<BossController>().BossName = "蛇皮怪"+"(" + Id + ")";
        b.GetComponent<BossController>().ArticleHandle = CreateArticle;
        b.GetComponent<BossController>().ExplosionHandle = CreateExplosion;
    }
    private void CreateExplosion(Vector3 pos)
    {
        GameObject ex = ObjectPool.GetInstance().CreateObject(explosion, "explosion1");
        ex.transform.position = pos;
    }
    private void CreateArticle(GameObject sender)
    {
        GameObject article = ObjectPool.GetInstance().CreateObject(star,"star");
        article.GetComponent<ArticleController>().ScoreHandle = AddScore;
        article.transform.position = sender.transform.position;
        if (sender.tag == "Enemy")
        {
            article.GetComponent<ArticleController>().Score = 1;
        }
        if (sender.tag == "Boss")
        {
            article.GetComponent<ArticleController>().Score = 50;
        }
    }
    private void AddScore(int score)
    {
        _totalScore += score;
        Txt_Stars.text = _totalScore.ToString();
    }
    // Update is called once per frame
    void Update () {
        if (!Login.isRoomMaster) return;//房主才能创建怪物
        if (currentCount >= maxCount) return;
        if (time <= 0)
        {
            time = 10f;
            if (Login.CurrentMode == PlayMode.Mutiplayer)
            {
                SendMsgHandle.Invoke(Login.ownerIPName + "#enemy#" + Id);
            }
            else
            {
                CreateEnemy();
            }
            currentCount++;
            if (killedCount >= maxCount&&BossCount<=0)
            {
                if (Login.CurrentMode == PlayMode.Mutiplayer)
                {
                    SendMsgHandle.Invoke(Login.ownerIPName + "#boss#" + Id);
                }
                else
                {
                    CreateBoss();
                }
                BossCount++;
            }
        }
        time -= Time.deltaTime;
	}
}
