using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class UIHome : UICanvas
{
    [SerializeField] private HomeTab[] homeTabs;
    [HideInInspector] public int currentTabIndex = 0;

    // --- THÊM MỚI ---
    [Header("Camera Control")]
    [Tooltip("Kéo object có script CameraViewSwitcher vào đây")]
    [SerializeField] private CameraViewSwitcher cameraSwitcher;
    // --- KẾT THÚC THÊM MỚI ---


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
        
        // (Tùy chọn) Chuyển camera về view chính khi vào Recruit
        if (cameraSwitcher != null)
        {
            cameraSwitcher.MoveToMainView();
        }
    }

    /// <summary>
    /// HÀM SỬA ĐỔI: Thêm logic chuyển camera
    /// </summary>
    public void troopBtn()
    {
        UIManager.Instance.OpenUI<UITroop>();
        UIManager.Instance.CloseUI<UIHome>(0f);


        if (cameraSwitcher != null)
        {
            cameraSwitcher.MoveToSecondaryView();
        }
    }
    public void city()
    {
        UIManager.Instance.CloseUI<UIHome>(0f);
        if (cameraSwitcher != null)
        {
            cameraSwitcher.MoveToMainView();
        }
        Debug.Log("Thang Ga");
    }



    /// <summary>
    /// HÀM SỬA ĐỔI: Thêm logic chuyển camera
    /// </summary>
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

        // --- THÊM MỚI ---
        // Khi quay về Home (Tab 0), chuyển camera về đảo chính
        if (index == 0 && cameraSwitcher != null)
        {
            cameraSwitcher.MoveToMainView();
        }
        // --- KẾT THÚC THÊM MỚI ---
    }

    public void SettingBtn()
    {

    }
}