using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEffect : MonoBehaviour
{
    [SerializeField] private GameObject lgihtEffect;
    [SerializeField] private float x = 0.35f;
    [SerializeField] private float y = 0f;
    [SerializeField] private float z = 0f;

    //public GameObject c_ani;
    //public GameObject c_mon;
    //public GameObject a_ani;
    //public GameObject a_mon;

    private void Update()
    {
        lgihtEffect.gameObject.transform.Rotate(x, y, z);

        //if(x >= 180f)
        //{
        //    c_ani.SetActive(false);
        //    c_mon.SetActive(true);
        //}
    }
}