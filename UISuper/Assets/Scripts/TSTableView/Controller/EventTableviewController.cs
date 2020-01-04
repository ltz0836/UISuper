using System.Collections;
using System.Collections.Generic;
using Tacticsoft;
using UnityEngine;

public class EventTableviewController : MonoBehaviour, ITableViewDataSource
{
    public EventTableviewCell m_cellPrefab;
    public TableView m_tableView;
    private ArrayList datas;

    public TableViewCell GetCellForRowInTableView(TableView tableView, int row)
    {
        EventTableviewCell cell = tableView.GetReusableCell(m_cellPrefab.reuseIdentifier) as EventTableviewCell;
        if (cell == null)
        {
            cell = (EventTableviewCell)GameObject.Instantiate(m_cellPrefab);
            cell.name = "MyTableViewCellInstance_" + row;
            cell.callback = CellCallback;
        }
        cell.updateInfo(datas[row] as MyEventInfo);
        return cell;
    }

    public float GetHeightForRowInTableView(TableView tableView, int row)
    {

        return (datas[row] as MyEventInfo).height;
    }

    public int GetNumberOfRowsForTableView(TableView tableView)
    {
        return datas.Count;
    }

    void CellCallback(EventTableviewCell cell, MyEventInfo info)
    {
        m_tableView.NotifyCellDimensionsChanged(info.index);
    }
    // Start is called before the first frame update
    void Start()
    {
        m_tableView.dataSource = this;
        datas = new ArrayList();
        for (int i = 0; i < 100; i++)
        {
            MyEventInfo info = new MyEventInfo
            {
                index = i,
                height = 200,
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
