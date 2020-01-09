using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoldierGroupContentView : MonoBehaviour
{
    public GameObject soldierView;
    // Start is called before the first frame update
    void Start()
    {
        for(int i= 0; i < 5; i++)
        {
            GameObject item = Instantiate(soldierView);
            item.transform.parent = this.transform;
            item.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
