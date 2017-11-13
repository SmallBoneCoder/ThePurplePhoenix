using UnityEngine;
using System.Collections.Generic;

public class ObjectPool: MonoBehaviour {
    private Dictionary<string, Queue<GameObject>> _allPool;
    private static ObjectPool instance;
    protected ObjectPool()
    {
        _allPool = new Dictionary<string, Queue<GameObject>>();
    }
    public static ObjectPool GetInstance()
    {
        if (instance == null)
        {
            print("??");
            //instance =this;
            
        }
        return instance;
    }
	// Use this for initialization
	void Start () {
        _allPool = new Dictionary<string, Queue<GameObject>>();
        instance = this;
    }
	/// <summary>
    /// 生成对象
    /// </summary>
    /// <param name="obj">对象的预制体</param>
    /// <param name="name">名称（统称）</param>
    /// <returns></returns>
    public GameObject CreateObject(GameObject obj,string name)
    {
        //T obj = default(T);
        if (!_allPool.ContainsKey(name))//没有这种对象
        {
            print(obj);
            print(name);
            Queue<GameObject> temp = new Queue<GameObject>();
            _allPool.Add(name,temp );
            print(_allPool.Count);
            // _allPool[name].Enqueue(Instantiate(obj));
            return Instantiate(obj);//生成新的
        }
        else
        {
            if (_allPool[name].Count > 0)//数量不为0
            {
                GameObject temp = _allPool[name].Dequeue();//出队
                temp.SetActive(true);
                return temp;//
            }
            print("new");
            return Instantiate(obj);//生成新的
        }
    }
    /// <summary>
    /// 销毁对象
    /// </summary>
    /// <param name="obj">实例</param>
    /// <param name="name">名称</param>
    public void DestroyObject(GameObject obj, string name)
    {
        obj.SetActive(false);
        print(obj);
        print(name);
        print(_allPool.Count);
        _allPool[name].Enqueue(obj);//入队
    }

	// Update is called once per frame
	void Update () {
	
	}
}
