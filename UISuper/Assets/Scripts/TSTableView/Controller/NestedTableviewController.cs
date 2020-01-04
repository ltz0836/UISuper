using System.Collections;
using System.Collections.Generic;
using Tacticsoft;
using UnityEngine;
using UnityEngine.UI;

public class NestedTableviewController : MonoBehaviour, ITableViewDataSource
{
    public EventTableviewCell m_cellPrefab;
    public TableView m_tableView;
    private ArrayList datas;
    public int rows;

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
        datas = new ArrayList();
        for (int i = 0; i < rows; i++)
        {
            MyEventInfo info = new MyEventInfo
            {
                index = i,
                height = 200,
                name = "name: " + i
            };
            datas.Add(info);
        }

        LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = rows * 200 + (rows - 1) * 10 + 90*2;

        m_tableView.dataSource = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}