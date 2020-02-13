using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//占位元素，测试使用。在自己的位置画一个圆点
public class PlaceHolder : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, 0.5f);
    }
}

public delegate void OnSelectedChanged(int index);
public delegate void OnScrollStatusChanged(UITurnTable.TableStatus oldStatus, UITurnTable.TableStatus newStatus);
public interface TurnTableAdapter
{
    //返回最大的个数
    int GetMaxCount();
    //返回实际需要显示的个数
    int GetCount();
    //返回显示的对象
    GameObject GetGameObject(UITurnTable table, int index);
}

public abstract class TurnTableItem : MonoBehaviour
{
    //通知item位置变化。pos 表示相对圆心（0,0,0）的位置。radius表示圆的半径
    public abstract void OnPositionChanged(Vector3 pos, float roundTableRadius);
}

public class UITurnTable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum TableStatus
    {
        Idle,
        Touch,
        Fling
    }

    public enum SelectedMode
    {
        Top, Bottom, Right, Left, Center
    }

    //圆盘的倾斜角度
    public float roundTableTiltDegree = 25;
    //占位的倾斜圆盘
    public GameObject tiltTablePlaceHolder;
    //显示的圆盘，不倾斜
    public GameObject uiRoundTable;
    //圆盘的半径
    public float roundTableRadius = 633;
    //fling的阻力
    public float flingResistance = 600;
    //圆盘显示、选择的位置
    public SelectedMode selectMode = SelectedMode.Bottom;
    //松手后，滚动模式的选择。true则为齿轮模式，false 为惯性fling模式
    public bool isGearScrollMode = true;
    //齿轮fling模式下，水平速度的阀值，超过阀值滚动到下一个item，否则停靠在当前的item
    public float gearScrollModeFlingThreshold = 500;
    //松手后fling动画的速度参数，越大运动越慢，越小运动越快
    public float flingSpeed = 2.5f;
    //数据最大的份数
    private int maxCount = 0;
    //数据实际的份数
    private int count = 0;
    //可转动到的最小弧度
    private float minRotationRadian;
    //可转动到的最大弧度
    private float maxRotationRadian;
    //选中item的弧度
    private float selectedRadian;
    //每个item占的弧度
    private float eachRadian;
    private float selectMaxRadian;
    private float selectMinRadian;
    //旋转的弧度
    private float roundTableRotationRadian = 0;
    private float lastRoundTableRotationRadian = -1;
    private float startRoundTableRotationRadian = 0;
    //松手后滑动到的目标
    private float flingToTargetRandian;
    //齿轮模式下，滑动的中间速度记录（这里不是设置滑动速度的变量，只是计算中间产物）
    private float gearRotateVelocity = 0;
    //占位元素对象的引用
    private GameObject[] placeHolders;
    //显示元素的对象引用（不能用child的index来确定元素，兄弟节点位置会变）
    private GameObject[] images;
    //触摸的上一个点
    private Vector2 lastPosition;
    //计算拖动速度的工具
    private KSFlingVelocityCalculate flingVelocityCalculate;
    //数据适配器
    private TurnTableAdapter adapter;
    //数据选择改变监听器
    public OnSelectedChanged selectedListener;
    public OnScrollStatusChanged scrollStatusListener;

    //当前选中的index。初始为-1，第一次显示的时候一定会触发选择改变。
    private int selectedIndex = -1;

    private bool _updateItemDone = false;
    public bool updateItemDone
    {
        get
        {
            return _updateItemDone;
        }
    }

    private TableStatus status = TableStatus.Idle;

    private void SetStatus(TableStatus newStatus)
    {
        TableStatus oldStatus = this.status;
        this.status = newStatus;
        if (scrollStatusListener != null)
        {
            scrollStatusListener(oldStatus, newStatus);
        }
    }

    void Start()
    {
        flingVelocityCalculate = new KSFlingVelocityCalculate();
        flingVelocityCalculate.Reset();

        SetStatus(TableStatus.Idle);
    }

    //设置数据绑定对象。只能设置一次，并且在start调用才生效
    public void SetAdapter(TurnTableAdapter adapter, bool coroutineLoading = false)
    {
        this.adapter = adapter;

        //摆斜占位盘子、z轴偏移位置
        float xOffset = 0;
        float zOffset = 0;
        float yOffset = 0;
        switch (selectMode)
        {
            case SelectedMode.Bottom:
                selectedRadian = Mathf.PI * 3 / 2;
                xOffset = 0;
                zOffset = Mathf.Cos(roundTableTiltDegree * Mathf.Deg2Rad) * roundTableRadius;
                yOffset = Mathf.Sin(roundTableTiltDegree * Mathf.Deg2Rad) * roundTableRadius;
                break;
            case SelectedMode.Top:
                selectedRadian = Mathf.PI / 2;
                xOffset = 0;
                zOffset = -Mathf.Cos(roundTableTiltDegree * Mathf.Deg2Rad) * roundTableRadius;
                yOffset = -Mathf.Sin(roundTableTiltDegree * Mathf.Deg2Rad) * roundTableRadius;
                break;
            case SelectedMode.Right:
                selectedRadian = 0;
                xOffset = -Mathf.Cos(selectedRadian) * roundTableRadius;
                zOffset = 0;
                yOffset = 0;
                break;
            case SelectedMode.Left:
                selectedRadian = Mathf.PI;
                xOffset = -Mathf.Cos(selectedRadian) * roundTableRadius;
                zOffset = 0;
                yOffset = 0;
                break;
            case SelectedMode.Center:
                selectedRadian = Mathf.PI / 2;
                xOffset = 0;
                zOffset = 0;
                yOffset = 0;
                break;
        }

        tiltTablePlaceHolder.transform.localPosition = new Vector3(xOffset, yOffset, zOffset);
        tiltTablePlaceHolder.transform.rotation = Quaternion.Euler(new Vector3(-roundTableTiltDegree, 0, 0));
        //通过adapter获取数据数量
        maxCount = adapter.GetMaxCount();
        count = adapter.GetCount();
        //计算每个item占的角度、从而计算出可滑动最大、最小角度
        eachRadian = 2 * Mathf.PI / maxCount;
        minRotationRadian = -(eachRadian * (count - 1) + eachRadian / 2);
        maxRotationRadian = eachRadian / 2;
        selectMaxRadian = selectedRadian + eachRadian / 2;
        selectMinRadian = selectedRadian - eachRadian / 2;

        for (int i = 0; i < tiltTablePlaceHolder.transform.childCount; i++)
        {
            GameObject obj = tiltTablePlaceHolder.transform.GetChild(i).gameObject;
            Destroy(obj);
        }
        for (int i = 0; i < uiRoundTable.transform.childCount; i++)
        {
            Destroy(uiRoundTable.transform.GetChild(i).gameObject);
        }
        if (coroutineLoading)
        {
            //动态添加占位元素 和 显示元素
            placeHolders = new GameObject[count];
            images = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                GameObject placeHolder = new GameObject();
                placeHolder.name = "" + i;
                placeHolder.AddComponent<PlaceHolder>();
                placeHolder.transform.SetParent(tiltTablePlaceHolder.transform, false);
                GameObject imageObj = new GameObject();
                imageObj.transform.SetParent(uiRoundTable.transform, false);
                placeHolders[i] = placeHolder;
                images[i] = imageObj;
            }
            lastRoundTableRotationRadian = roundTableRotationRadian - 1;
            UpdateLayout();
            UpdateItem(count);
        }
        else
        {
            _updateItemDone = true;
            //动态添加占位元素 和 显示元素
            placeHolders = new GameObject[count];
            images = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                GameObject placeHolder = new GameObject();
                placeHolder.name = "" + i;
                placeHolder.AddComponent<PlaceHolder>();
                placeHolder.transform.SetParent(tiltTablePlaceHolder.transform, false);

                GameObject imageObj = adapter.GetGameObject(this, i);
                imageObj.transform.SetParent(uiRoundTable.transform, false);
                imageObj.AddComponent<KSPrivateDataReference>().setData<int>(i);
                TurnTableItem item = imageObj.GetComponent<TurnTableItem>();
                placeHolders[i] = placeHolder;
                images[i] = imageObj;
            }
            lastRoundTableRotationRadian = roundTableRotationRadian - 1;
            UpdateLayout();
        }
    }

    void UpdateItem(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject imageObj = adapter.GetGameObject(this, i);
            if (imageObj == null)
            {
                continue;
            }
            imageObj.transform.SetParent(uiRoundTable.transform, false);
            imageObj.AddComponent<KSPrivateDataReference>().setData<int>(i);
            TurnTableItem item = imageObj.GetComponent<TurnTableItem>();
            Destroy(images[i]);
            images[i] = imageObj;
            lastRoundTableRotationRadian = roundTableRotationRadian - 1;
            UpdateLayout();
        }
        _updateItemDone = true;
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        //绘制可触摸区域，方便调试
        Gizmos.color = Color.red;
        RectTransform t = ((RectTransform)this.transform);
        Vector3[] corners = new Vector3[4];
        t.GetWorldCorners(corners);
        for (int i = 0; i < 4; i++)
        {
            //UnityEditor.Handles.DrawLine(corners[i % 4], corners[(i + 1) % 4]);
            Gizmos.DrawLine(corners[i % 4], corners[(i + 1) % 4]);
        }

        float xOffset = 0;
        float zOffset = 0;
        float yOffset = 0;
        switch (selectMode)
        {
            case SelectedMode.Bottom:
                selectedRadian = Mathf.PI * 3 / 2;
                xOffset = 0;
                zOffset = Mathf.Cos(roundTableTiltDegree * Mathf.Deg2Rad) * roundTableRadius;
                yOffset = Mathf.Sin(roundTableTiltDegree * Mathf.Deg2Rad) * roundTableRadius;
                break;
            case SelectedMode.Top:
                selectedRadian = Mathf.PI / 2;
                xOffset = 0;
                zOffset = -Mathf.Cos(roundTableTiltDegree * Mathf.Deg2Rad) * roundTableRadius;
                yOffset = -Mathf.Sin(roundTableTiltDegree * Mathf.Deg2Rad) * roundTableRadius;
                break;
            case SelectedMode.Right:
                selectedRadian = 0;
                xOffset = -Mathf.Cos(selectedRadian) * roundTableRadius;
                zOffset = 0;
                yOffset = 0;
                break;
            case SelectedMode.Left:
                selectedRadian = Mathf.PI;
                xOffset = -Mathf.Cos(selectedRadian) * roundTableRadius;
                zOffset = 0;
                yOffset = 0;
                break;
            case SelectedMode.Center:
                selectedRadian = Mathf.PI / 2;
                xOffset = 0;
                zOffset = 0;
                yOffset = 0;
                break;
        }
        Vector3 roundTableCenter = t.localToWorldMatrix.MultiplyPoint(new Vector3(xOffset, yOffset, zOffset));
        Vector3 roundTableDirection = new Vector3(0, Mathf.Sin(Mathf.PI / 2 - roundTableTiltDegree * Mathf.Deg2Rad), -Mathf.Cos(Mathf.PI / 2 - roundTableTiltDegree * Mathf.Deg2Rad));

        UnityEditor.Handles.color = Color.yellow;
        //绘制圆盘
        UnityEditor.Handles.DrawWireDisc(roundTableCenter, roundTableDirection, roundTableRadius / 10);
        //绘制选中指针
        Vector3 roundTableArcDirection = new Vector3(0, Mathf.Sin(roundTableTiltDegree * Mathf.Deg2Rad), Mathf.Cos(roundTableTiltDegree * Mathf.Deg2Rad));//圆盘向上的方向
        roundTableArcDirection = Quaternion.AngleAxis(90, roundTableDirection) * roundTableArcDirection;//转动到摆放的起始方向
        roundTableArcDirection = Quaternion.AngleAxis(-selectedRadian * Mathf.Rad2Deg, roundTableDirection) * roundTableArcDirection;//逆时针转动到选中方向
        UnityEditor.Handles.DrawSolidArc(roundTableCenter, roundTableDirection, roundTableArcDirection, 0.5f, roundTableRadius / 10);
#endif
    }
    public void OnDestroy()
    {

    }

    //过滤旋转弧度，确保不越界
    private float FliterRotationRadian(float rawRotationRadian)
    {
        float tempRadian = rawRotationRadian;
        if (selectMode != SelectedMode.Center)
        {
            if (tempRadian <= minRotationRadian)
            {
                tempRadian = minRotationRadian;
            }
            else if (tempRadian >= maxRotationRadian)
            {
                tempRadian = maxRotationRadian;
            }
        }
        return tempRadian;
    }

    //这里用水平滑动过的距离约等于圆弧弧长，和圆周长比例的关系算出圆弧弧度。
    //原公式是：delta.x / (roundTableRadius * 2*PI) * 2*PI;
    private float TouchDelat2RotationRadian(float delatX, float delatY)
    {
        switch (selectMode)
        {
            case SelectedMode.Bottom:
                return delatX / (roundTableRadius);
            case SelectedMode.Top:
                return -delatX / (roundTableRadius);
            case SelectedMode.Right:
                return delatY / (roundTableRadius);
            case SelectedMode.Left:
                return -delatY / (roundTableRadius);
            case SelectedMode.Center:
                return delatX / (roundTableRadius);
        }
        return 0;
    }
    private float GearFlingVelocity2RotationRadian(Vector2 velocity)
    {
        if (Mathf.Abs(startRoundTableRotationRadian - roundTableRotationRadian) >= eachRadian)
        {
            //拖动已经过了一个item的量，不再判断速度，停靠到当前item
            return 0;
        }
        switch (selectMode)
        {
            case SelectedMode.Bottom:
                if (velocity.x > gearScrollModeFlingThreshold)
                {
                    //水平速度够快，停靠到下一个item
                    return eachRadian / 2;
                }
                else if (velocity.x < -gearScrollModeFlingThreshold)
                {
                    //水平速度够快，停靠到下一个item
                    return -eachRadian / 2;
                }
                else
                {
                    //水平速度慢，停靠在当前item
                    return 0;
                }
            case SelectedMode.Top:
                if (velocity.x > gearScrollModeFlingThreshold)
                {
                    return -eachRadian / 2;
                }
                else if (velocity.x < -gearScrollModeFlingThreshold)
                {
                    return eachRadian / 2;
                }
                else
                {
                    return 0;
                }
            case SelectedMode.Right:
                if (velocity.y > gearScrollModeFlingThreshold)
                {
                    return eachRadian / 2;
                }
                else if (velocity.y < -gearScrollModeFlingThreshold)
                {
                    return -eachRadian / 2;
                }
                else
                {
                    return 0;
                }
            case SelectedMode.Left:
                if (velocity.y > gearScrollModeFlingThreshold)
                {
                    return -eachRadian;
                }
                else if (velocity.y < -gearScrollModeFlingThreshold)
                {
                    return eachRadian;
                }
                else
                {
                    return 0;
                }
        }
        return 0;
    }
    public ScrollRect fallbackTouchObject;
    private bool passTouchToFallback = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (fallbackTouchObject != null)
        {
            switch (selectMode)
            {
                case SelectedMode.Bottom:
                case SelectedMode.Top:
                    //turntable这种情况只处理左右水平滑动，否则把所有事件丢给fallbackTouchObject处理
                    if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
                    {
                        //水平滑动
                        passTouchToFallback = false;
                    }
                    else
                    {
                        passTouchToFallback = true;
                    }
                    break;
                case SelectedMode.Right:
                case SelectedMode.Left:
                    //turntable这种情况只处理上下垂直滑动，否则把所有事件丢给fallbackTouchObject处理
                    if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
                    {
                        //水平滑动
                        passTouchToFallback = true;
                    }
                    else
                    {
                        passTouchToFallback = false;
                    }
                    break;
                case SelectedMode.Center:
                    passTouchToFallback = true;
                    passTouchToFallback = true;
                    break;
            }
            if (passTouchToFallback)
            {
                fallbackTouchObject.OnBeginDrag(eventData);
                return;
            }
        }

        SetStatus(TableStatus.Touch);
        startRoundTableRotationRadian = roundTableRotationRadian;
        lastPosition = eventData.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (passTouchToFallback)
        {
            fallbackTouchObject.OnDrag(eventData);
            return;
        }

        if (status != TableStatus.Touch)
        {
            return;
        }
        Vector3 delta = eventData.position - lastPosition;

        roundTableRotationRadian += TouchDelat2RotationRadian(delta.x, delta.y);
        roundTableRotationRadian = FliterRotationRadian(roundTableRotationRadian);
        lastPosition = eventData.position;
        flingVelocityCalculate.addPoint(eventData.position, Time.deltaTime);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (passTouchToFallback)
        {
            fallbackTouchObject.OnEndDrag(eventData);
            return;
        }
        if (status != TableStatus.Touch)
        {
            return;
        }
        SetStatus(TableStatus.Fling);

        flingVelocityCalculate.addPoint(eventData.position, Time.deltaTime);
        Vector2 velocity = flingVelocityCalculate.computeCurrentVelocity();
        if (KSDebug.enableLog) KSDebug.Log("fling velocity = " + velocity.ToString());
        flingVelocityCalculate.Reset();

        float targetRadian;
        if (isGearScrollMode)
        {
            //齿轮滚动模式
            targetRadian = FliterRotationRadian(roundTableRotationRadian + GearFlingVelocity2RotationRadian(velocity));
        }
        else
        {
            //惯性滚动模式
            //松手后要滚动的距离
            float xf = velocity.x == 0 ? 1 : velocity.x / Mathf.Abs(velocity.x);
            float yf = velocity.y == 0 ? 1 : velocity.y / Mathf.Abs(velocity.y);
            float xs = xf * Mathf.Pow(velocity.x, 2) / (2 * flingResistance);
            float ys = yf * Mathf.Pow(velocity.y, 2) / (2 * flingResistance);
            targetRadian = roundTableRotationRadian + TouchDelat2RotationRadian(xs, ys);
            targetRadian = FliterRotationRadian(targetRadian);
        }

        //停靠到最进的一个弧度上
        float maxNearRandian;
        float minNearRandian;
        if (targetRadian >= 0)
        {
            minNearRandian = ((int)(targetRadian / eachRadian)) * eachRadian;
            maxNearRandian = minNearRandian + eachRadian;
        }
        else
        {
            maxNearRandian = ((int)(targetRadian / eachRadian)) * eachRadian;
            minNearRandian = maxNearRandian - eachRadian;
        }



        float toRandian;
        if (maxNearRandian >= maxRotationRadian)
        {//达到顺时针的边界
            toRandian = minNearRandian;
        }
        else if (minNearRandian <= minRotationRadian)
        {//达到逆时针边界
            toRandian = maxNearRandian;
        }
        else
        {
            toRandian = Mathf.Abs(targetRadian - maxNearRandian) > Mathf.Abs(minNearRandian - targetRadian) ? minNearRandian : maxNearRandian;
        }

        if (Mathf.Abs(toRandian - roundTableRotationRadian) < 0.01)
        {
            roundTableRotationRadian = toRandian;
            SetStatus(TableStatus.Idle);
        }
        else
        {
            //记录fling目标，在后续update中进行动画滚动
            flingToTargetRandian = toRandian;
        }
    }

    //在update的时候调用，更新ui
    public void UpdateLayout()
    {
        //如果没旋转，保持原样，不必刷新位置
        if (lastRoundTableRotationRadian == roundTableRotationRadian)
        {
            return;
        }
        lastRoundTableRotationRadian = roundTableRotationRadian;
        int newSelectedIndex = selectedIndex;
        for (int i = 0; i < count; i++)
        {
            //计算占位元素的位置（相对圆心的位置）
            float angleRadian = roundTableRotationRadian + i * Mathf.PI * 2 / maxCount + selectedRadian;
            Vector3 pos = new Vector3(Mathf.Cos(angleRadian), 0, Mathf.Sin(angleRadian)) * roundTableRadius;
            //设置占位元素的相对位置
            GameObject placeHolder = placeHolders[i];
            placeHolder.gameObject.transform.localPosition = pos;

            GameObject imageObj = images[i];
            //通知item位置设置变化（item可在此修改自己alpha 亮度等）
            TurnTableItem item = imageObj.GetComponent<TurnTableItem>();
            if (item != null)
            {
                item.OnPositionChanged(pos, roundTableRadius);
            }

            if (angleRadian >= selectMinRadian && angleRadian <= selectMaxRadian)
            {
                newSelectedIndex = i;
            }
        }
        if (newSelectedIndex != selectedIndex)
        {
            selectedIndex = newSelectedIndex;
            //通知选择变化
            if (selectedListener != null)
            {
                selectedListener(selectedIndex);
            }
        }
    }

    void Update()
    {
        if (status == TableStatus.Fling)
        {
            //如果正处于fling模式，处理动画
            roundTableRotationRadian = FliterRotationRadian(Mathf.SmoothDamp(roundTableRotationRadian, flingToTargetRandian, ref gearRotateVelocity, Time.deltaTime * flingSpeed));
            if (Mathf.Abs(flingToTargetRandian - roundTableRotationRadian) < 0.01)
            {
                roundTableRotationRadian = flingToTargetRandian;
                SetStatus(TableStatus.Idle);
            }
        }
        UpdateLayout();
    }

    public void LateUpdate()
    {
        /**
         * follow 的行为现在还不能加变化判断。否则在ios设备上无法正确follow
         */
        for (int i = 0; i < count; i++)
        {
            GameObject placeHolder = placeHolders[i];

            //把占位元素的绝对位置，设置给显示元素的绝对位置
            GameObject imageObj = images[i];
            imageObj.transform.position = placeHolder.transform.position;

        }
        //根据显示元素的z值，确定遮挡关系，设置显示元素兄弟节点的排序
        int childCount = uiRoundTable.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            for (int j = 1; j < childCount - i; j++)
            {
                GameObject imageObj1 = uiRoundTable.transform.GetChild(j).gameObject;
                GameObject imageObj2 = uiRoundTable.transform.GetChild(j - 1).gameObject;
                if (imageObj2.transform.position.z < imageObj1.transform.position.z)
                {
                    imageObj1.transform.SetSiblingIndex(j - 1);
                }
            }
        }
    }

    /// <summary>
    /// 向前或者向后移位
    /// </summary>
    /// <param name="front">向前移位为true</param>
    public void Shift(bool front)
    {
        if (status != UITurnTable.TableStatus.Idle)
            return;
        if (front && selectedIndex < count - 1)
            SetSelected(selectedIndex + 1, true);
        else if (!front && selectedIndex > 0)
            SetSelected(selectedIndex - 1, true);
    }

    //设置选中
    public void SetSelected(int index, bool animation = false)
    {
        if (index < 0 || index >= maxCount)
        {
            if (KSDebug.enableLog) KSDebug.LogError("SetSelected index = " + index + " out of rang!!!!");
            return;
        }

        this.selectedIndex = -1;
        this.lastRoundTableRotationRadian = -1;
        float targetRadian = -index * eachRadian;
        targetRadian = FliterRotationRadian(targetRadian);
        if (animation)
        {
            if (status == TableStatus.Idle)
            {
                SetStatus(TableStatus.Fling);
            }
            flingToTargetRandian = targetRadian;
        }
        else
        {
            roundTableRotationRadian = targetRadian;
            UpdateLayout();
        }
    }

    public int GetSelected()
    {
        return selectedIndex;
    }

    public GameObject getObjView(int key)
    {
        if (images == null)
        {
            return null;
        }

        return this.images[key];
    }

    public int getOjbCount()
    {
        return count;
    }
}
