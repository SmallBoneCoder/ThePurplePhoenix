using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStickEvent : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler {
    public Vector3 currentDir;
    private Vector3 originPos;
    private Transform _stick;
    public CMDHandle cmdHandle;
    public SyncHandle syncHandle;
    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new NotImplementedException();
        //currentDir = _stick.localPosition - originPos;
        cmdHandle.Invoke((_stick.localPosition - originPos).ToString("F2"));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new NotImplementedException();
        //currentDir = Vector3.zero;
        cmdHandle.Invoke((Vector3.zero).ToString("F2"));
        //syncHandle();
    }
    
    // Use this for initialization
    void Start () {
        originPos = transform.GetChild(0).localPosition;
        _stick = transform.GetChild(0);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.LeftArrow)&& Input.GetKey(KeyCode.UpArrow))
        {
            print("UL");
            cmdHandle.Invoke(new Vector3(-10, 10, 0).ToString("F2"));
            return;
        }
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow))
        {
            cmdHandle.Invoke(new Vector3(10, 10, 0).ToString("F2"));
            return;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            cmdHandle.Invoke(Vector3.up.ToString());
            return;
        }
        if (Input.GetKey(KeyCode.LeftArrow)&& Input.GetKey(KeyCode.DownArrow))
        {
            cmdHandle.Invoke(new Vector3(-10, -10, 0).ToString("F2"));
            return;
        }
        if (Input.GetKey(KeyCode.RightArrow)&& Input.GetKey(KeyCode.DownArrow))
        {
            print("DR");
            cmdHandle.Invoke(new Vector3(10, -10, 0).ToString("F2"));
            return;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            cmdHandle.Invoke(Vector3.down.ToString());
            return;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            cmdHandle.Invoke(Vector3.left.ToString());
            return;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            cmdHandle.Invoke(Vector3.right.ToString());
            return;
        }
        
    }
}
