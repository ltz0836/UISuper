using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITrainTroop : MonoBehaviour
{
    public UITurnTable turnTable;
    public GameObject soldierItemPrefab;
    public UIInfo info;

    private ArmyConfig currentArmyConfig;
    private List<ArmyConfig> armyConfigList;
    private int max = 200;
    private int amount;

    void Start()
    {
        InitTurnTable();
    }

    void InitTurnTable()
    {
        armyConfigList = new List<ArmyConfig>();
        for (int i = 1; i <= 12; i++)
        {
            int index = i + 20;
            ArmyConfig config = new ArmyConfig
            {
                level = i,
                image_name = "Army_Model_" + index,
                name = i + "级将军"
            };
            armyConfigList.Add(config);
        }

        //Unity卡死问题
        amount = max;
        int defaultIndex = this.GetDefaultSelectedSoldier();
        currentArmyConfig = armyConfigList[defaultIndex];
        turnTable.SetAdapter(new TroopTypeAdapter(armyConfigList, soldierItemPrefab), true);
        turnTable.selectedListener += OnSelectedChanged;
        turnTable.scrollStatusListener += OnScrollStatusChanged;
        turnTable.SetSelected(defaultIndex);
    }

    private void OnSelectedChanged(int index)
    {
        currentArmyConfig = armyConfigList[index];
        info.UpdateInfo(currentArmyConfig.level, currentArmyConfig.name);
    }

    private void OnScrollStatusChanged(UITurnTable.TableStatus oldStatus, UITurnTable.TableStatus newStatus)
    {

    }

    //获取初始显示的兵种
    private int GetDefaultSelectedSoldier()
    {
        int order = 0;
        foreach (ArmyConfig army in armyConfigList)
        {
            if (army.level > 0)
            {
                if (army.level <= 1)
                {
                    order = armyConfigList.IndexOf(army);
                }
            }
        }
        return order;
    }

    //士兵列表的适配器
    private class TroopTypeAdapter : TurnTableAdapter
    {
        private List<ArmyConfig> armyConfigList;
        private GameObject soldierItemPrefab;

        public TroopTypeAdapter(List<ArmyConfig> armyConfigList, GameObject soldierItemPrefab)
        {
            this.armyConfigList = armyConfigList;
            this.soldierItemPrefab = soldierItemPrefab;
        }

        //返回最大的个数
        public int GetMaxCount()
        {
            return armyConfigList.Count + 2;
        }
        //返回实际需要显示的个数
        public int GetCount()
        {
            return armyConfigList.Count;
        }
        //返回显示的对象
        public GameObject GetGameObject(UITurnTable table, int index)
        {
            GameObject itemObject = Instantiate(soldierItemPrefab) as GameObject;

            UISoldierItem item = itemObject.GetComponent<UISoldierItem>();
            item.SetArmyConfig(index, armyConfigList[index]);
            table.selectedListener += item.OnSelectedChanged;
            table.scrollStatusListener += item.OnScrollStatusChanged;
            itemObject.name = "" + index;
            return itemObject;
        }
    }
}

public class ArmyConfig
{
    public int level;
    public string image_name;
    public string name;
}
