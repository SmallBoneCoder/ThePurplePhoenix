using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PatrolPointFactory : MonoBehaviour {
    public GameObject explosion;
    public GameObject star;
    public GameObject Txt_Star;
    public GameObject enemy;
    public GameObject boss;
    public Transform[] PatrolPoints;
    public static List<GameObject> _allEnemies;
    public static List<GameObject> _allBoss;

    public void CreateEnemy(string enemy_json)
    {
        EnemyInfo e= JsonUtility.FromJson<EnemyInfo>(enemy_json);
        GameObject newE = Instantiate(enemy);
        newE.GetComponent<EnemyController>().InitEnemy(e, PatrolPoints[e.PatrolId]);
    }
    public void CreateBoss(string boss_json)
    {
        BossInfo b = JsonUtility.FromJson<BossInfo>(boss_json);
        GameObject newB = Instantiate(boss);
        newB.GetComponent<BossController>().InitBoss(b, PatrolPoints[b.PatrolId]);
    }
	// Use this for initialization
	void Start () {
        _allEnemies = new List<GameObject>();
        _allBoss = new List<GameObject>();
        for(int i = 0; i < PatrolPoints.Length; i++)
        {
            PatrolPoints[i].GetComponent<PatrolController>().explosion = explosion;
            PatrolPoints[i].GetComponent<PatrolController>().star = star;
            PatrolPoints[i].GetComponent<PatrolController>().Txt_Stars = Txt_Star.GetComponent<Text>();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
