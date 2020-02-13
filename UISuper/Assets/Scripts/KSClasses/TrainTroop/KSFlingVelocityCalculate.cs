using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
public class KSFlingVelocityCalculate
{
    private const int POINT_COUNT = 3;
    private Vector2[] pointList = new Vector2[POINT_COUNT];
    private float[] deltaTimeList = new float[POINT_COUNT - 1];
    private int pointCount = 0;

    public void Reset()
    {
        pointList = new Vector2[POINT_COUNT];
        deltaTimeList = new float[POINT_COUNT - 1];
        pointCount = 0;
    }

    //计算fling速度。添加记录touch的点
    public void addPoint(Vector2 point, float deltaTime)
    {
        Array.Copy(pointList, 0, pointList, 1, POINT_COUNT - 1);
        Array.Copy(deltaTimeList, 0, deltaTimeList, 1, POINT_COUNT - 2);
        pointList[0] = point;
        deltaTimeList[0] = deltaTime;
        ++pointCount;
        if (pointCount > POINT_COUNT)
        {
            pointCount = POINT_COUNT;
        }
    }

    private Vector3 toWorldAxis(Vector2 vector)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(vector.x, vector.y, -Camera.main.transform.position.z));
    }

    //计算fling速度
    public Vector2 computeCurrentVelocity()
    {
        int startPoint = 0;
        if (pointCount < 2)
        {
            return new Vector2(0, 0);
        }
        else if (pointCount == 2)
        {
            startPoint = 0;
        }
        else if ((pointList[0] - pointList[1]).magnitude < 10)
        {
            startPoint = 1;//忽略up的点
        }
        Vector2 distance = pointList[startPoint] - pointList[startPoint + 1];
        float time = deltaTimeList[0];
        for (int i = startPoint + 1; i < pointCount - 1; i++)
        {
            Vector2 preDistance = pointList[i - 1] - pointList[i];
            Vector2 toMergeDistance = pointList[i] - pointList[i + 1];
            if (toMergeDistance.magnitude / 2 > preDistance.magnitude)
            {
                float angle = MathUtil.Vector2Angle(preDistance, toMergeDistance);
                angle = MathUtil.UniformAngle(angle);
                if (angle > 180)
                {
                    angle -= 360;
                }
                angle = Mathf.Abs(angle);

                if (angle < 45)
                {
                    distance += toMergeDistance;
                    time += deltaTimeList[i];
                }
            }

        }

        Vector2 velocity = distance / time;

        return velocity;
    }
}
