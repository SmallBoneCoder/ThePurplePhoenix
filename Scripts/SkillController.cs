using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillController : MonoBehaviour,IPointerDownHandler{
    public string SkillId = "0";
    public CMDHandle cmdHandle;//命令处理委托
    public GameObject Skillbullet;//技能子弹
    public Transform PlayerPosition;//玩家位置
    private Transform img_0;//技能图片
    private Transform img_1;
    private bool isCoolDown = true;//是否冷却完成
    public float coolDownTime = 1f;//冷却时间
    private float _time = 0;//时间
    private float _rate = 0.1f;//计时器频率
    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new NotImplementedException();
        
        if (isCoolDown)
        {
            img_1.GetComponent<Image>().fillAmount = 1;
            cmdHandle.Invoke(SkillId);
            isCoolDown = false;
            InvokeRepeating("Timer", 0, _rate);

        }
    }
    
    // Use this for initialization
    void Start () {
        //PlayerPosition.GetComponent<PlayerController>().AddSkill(gameObject);
        img_0 = transform.GetChild(0).GetChild(0);
        img_1 = transform.GetChild(0).GetChild(1);
	}
	
    private void Timer()
    {
        _time += _rate;
        img_1.GetComponent<Image>().fillAmount = (coolDownTime - _time) / coolDownTime;
        if (_time >= coolDownTime)
        {
            isCoolDown = true;
            _time = 0;
            CancelInvoke();
        }
    }
    private void OnGUI()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            OnPointerDown(null);
        }
    }
    // Update is called once per frame
    void Update () {
        
	}
}
