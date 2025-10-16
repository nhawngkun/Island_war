using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class UIHome : UICanvas
{
    [SerializeField] private HomeTab[] homeTabs;
    [HideInInspector] public int currentTabIndex = 0;


    public void PlayBtn()
    {
       
        
    }

    private void Start()
    {
        OnTabClick(0);

    }
    public void recruitBtn()
    {
        UIManager.Instance.OpenUI<UIRecruit>();
        UIManager.Instance.CloseUI<UIHome>(0f);
    }

   

    

    public void OnTabClick(int index)
    {
        foreach (var tab in homeTabs)
        {
            if (tab.tabIndex == index)
            {
                tab.AnimationOn();
            }
            else
            {
                tab.AnimationOff();
            }
        }
    }

    public void SettingBtn()
    {
        // Add settings functionality here
    }
}