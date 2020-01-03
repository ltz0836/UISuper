using System.Collections;
using System.Collections.Generic;
using Tacticsoft;
using UnityEngine;

public class MyTableviewController : MonoBehaviour, ITableViewDataSource
{
    public MyTableViewCell m_cellPrefab;
    public TableView m_tableView;
    private ArrayList datas;

    public TableViewCell GetCellForRowInTableView(TableView tableView, int row)
    {
        MyTableViewCell cell = tableView.GetReusableCell(m_cellPrefab.reuseIdentifier) as MyTableViewCell;
        if (cell == null)
        {
            cell = (MyTableViewCell)GameObject.Instantiate(m_cellPrefab);
            cell.name = "MyTableViewCellInstance_" + row;
            cell.cellCallback = CellCallback;
        }
        cell.updateInfo(datas[row] as MyCellInfo);
        return cell;
    }

    public float GetHeightForRowInTableView(TableView tableView, int row)
    {
        return (datas[row] as MyCellInfo).height;
    }

    public int GetNumberOfRowsForTableView(TableView tableView)
    {
        return datas.Count;
    }

    void CellCallback(MyTableViewCell cell, MyCellInfo info)
    {
        m_tableView.NotifyCellDimensionsChanged(info.index);
    }
    // Start is called before the first frame update
    void Start()
    {
        m_tableView.dataSource = this;
        datas = new ArrayList();
        for(int i = 0; i < 100; i++)
        {
            MyCellInfo info = new MyCellInfo
            {
                index = i,
                height = 100,
                name = "name: " + i
            };
            datas.Add(info);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
