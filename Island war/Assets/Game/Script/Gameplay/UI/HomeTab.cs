using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HomeTab : MonoBehaviour
{
    public GameObject homeTab;
    public UIHome uiHome;
    public int tabIndex = 0;

    public void OnClick()
    {
        if (uiHome.currentTabIndex == tabIndex) return;
        uiHome.OnTabClick(tabIndex);
    }
    
    public void AnimationOff() 
    {
        homeTab.GetComponent<RectTransform>().localScale = Vector3.one;
        homeTab.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.05f).SetEase(Ease.OutBack);
    }

    public void AnimationOn()
    {
        uiHome.currentTabIndex = tabIndex;
        
        // Move the parent tab GameObject (hero, shop, etc.) to the end of the sibling list
        this.transform.SetAsLastSibling();
        
        homeTab.GetComponent<RectTransform>().localScale = Vector3.zero;
        homeTab.GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }
}