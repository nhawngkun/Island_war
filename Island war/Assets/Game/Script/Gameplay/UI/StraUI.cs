using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.OpenUI<UIHome>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
