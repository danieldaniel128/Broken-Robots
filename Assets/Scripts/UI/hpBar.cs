using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hpBar : MonoBehaviour
{
    [SerializeField] Image[] HPShards;
    Vector2[] InitialPosition;
    private int modifier;
    int difference;
    float opacity;
    [SerializeField] int maxHP;
    [SerializeField] int currentHP;
    [SerializeField] float strength;
    [SerializeField] float fadeDistance = 2;
    [SerializeField] float AnimationSpeed = 0.07f;


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
        if (currentHP < 0) currentHP = 0;
        for (int i = 0; i < HPShards.Length; i++)
        {
            if (i >= maxHP)
            {
                HPShards[i].gameObject.SetActive(false);
            }
            else
            {
                HPShards[i].gameObject.SetActive(true);
            }
            difference = i - currentHP + 1;
            if (difference <0) difference = 0;
            HPShards[i].transform.position = Vector2.Lerp(HPShards[i].transform.position,
                new Vector2(InitialPosition[i].x + difference*strength * (HPShards.Length - Mathf.Sqrt(i)) * Mathf.Sqrt(HPShards.Length - i), InitialPosition[i].y+modifier*difference*strength*(HPShards.Length-i)),
                AnimationSpeed);
            modifier *= -1;
            if (i < currentHP) opacity = Vector3.Distance(HPShards[i].transform.position, InitialPosition[i]) / fadeDistance;
            else opacity = Vector3.Distance(HPShards[i].transform.position, InitialPosition[currentHP]) / fadeDistance;
            if (opacity < 0) opacity = 0;
            if (opacity > 1) opacity = 1;

            HPShards[i].color = new Color(HPShards[i].color.r, HPShards[i].color.g, HPShards[i].color.b, 1 - opacity);
        }
    }

    public void ModifyHP(int mod)
    {
        currentHP += mod;
    }

    public void ModifyMaxHP(int mod)
    {
        maxHP += mod;
    }
}
