using UnityEngine;
using System.Collections;
using VirutalMachine;

public class NormalFishAI : MonoBehaviour, IVMEntity
{
    public int m_aiType = 1;
    public float speed = 1.0f;
    public Vector3 m_targetPos;
	// Use this for initialization
	void Start () 
    {
        VMManager.GetInstance().CreateVMachine(this, m_aiType);
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.position = Vector3.MoveTowards(transform.position, m_targetPos, speed * Time.deltaTime);
	}

    public GameObject GetAttachObj()
    {
        return gameObject;
    }

    public void MoveToPos(Vector3 targetPos)
    {
        m_targetPos = targetPos;
    }
}
