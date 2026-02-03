using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeUI : MonoBehaviour
{
    public int initialLifeCnt;
    private int lifeCnt;
    public Player player;
    List<Image> images = new List<Image>();
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
            images.Add(transform.GetChild(i).GetComponent<Image>());
        UpdateLifeCnt(initialLifeCnt);
    }

    private void OnEnable()
    {
        player.OnLifeChanged += UpdateLifeCnt;
    }

    private void OnDisable()
    {
        player.OnLifeChanged -= UpdateLifeCnt;
    }

    public void UpdateLifeCnt(int newLifeCnt)
    {
        lifeCnt = newLifeCnt;
        for (int i = 0; i < transform.childCount; i++)
        {
            images[i].enabled = i < lifeCnt;
        }
    }
}
