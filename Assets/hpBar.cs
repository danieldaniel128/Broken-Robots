using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hpBar : MonoBehaviour
{
    [SerializeField] GameObject[] HPShards;
    Vector2[] InitialPosition;
    private int modifier;
    int difference;
    [SerializeField] int maxHP;
    [SerializeField] int currentHP;
    [SerializeField] float strength;


    void Start()
    {
        InitialPosition = new Vector2[HPShards.Length];
        for (int i = 0; i < HPShards.Length; i++) 
        {
            InitialPosition[i] = new Vector2(HPShards[i].transform.position.x,HPShards[i].transform.position.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        modifier = 1;
       
        for (int i = 0; i < HPShards.Length; i++)
        {
            if (i >= maxHP)
            {
                HPShards[i].SetActive(false);
            }
            else
            {
                HPShards[i].SetActive(true);
            }
            difference = i - currentHP + 1;
            if (difference <0) difference = 0;
            HPShards[i].transform.position = Vector2.Lerp(HPShards[i].transform.position,
                new Vector2(InitialPosition[i].x + difference*strength * (HPShards.Length - i) * (HPShards.Length - i), InitialPosition[i].y+modifier*difference*strength*(HPShards.Length-i)),
                0.07f);
            modifier *= -1;
        }
    }
}
