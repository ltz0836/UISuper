using System.Collections;
using System.Collections.Generic;
using Tacticsoft;
using Tacticsoft.Examples;
using UnityEngine;

public class KSTableviewController : MonoBehaviour, ITableViewDataSource
{
    public KSTableViewCell m_cellPrefab;
    public TableView m_tableView;

    public int m_numRows;
    private int m_numInstancesCreated = 0;

    public TableViewCell GetCellForRowInTableView(TableView tableView, int row)
    {
        KSTableViewCell cell = tableView.GetReusableCell(m_cellPrefab.reuseIdentifier) as KSTableViewCell;
        if (cell == null)
        {
            cell = (KSTableViewCell)GameObject.Instantiate(m_cellPrefab);
            cell.name = "KSTableviewCellInstance_" + (++m_numInstancesCreated).ToString();
        }
        return cell;
    }

    public float GetHeightForRowInTableView(TableView tableView, int row)
    {
        int value = row % 3;
        if(value == 0)
        {
            return 150.0f;
        }
        else if(value == 1)
        {
            return 120.0f;
        }
        else
        {
            return 180.0f;
        }
    }

    public int GetNumberOfRowsForTableView(TableView tableView)
    {
        return m_numRows;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_tableView.dataSource = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
