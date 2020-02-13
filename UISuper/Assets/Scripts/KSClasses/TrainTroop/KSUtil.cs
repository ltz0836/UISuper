using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil
{
    public static int bits(int num, int start, int count)
    {
        int remain = num >> start;
        int mask = (int)Mathf.Pow(2, count) - 1;
        return remain & mask;
    }

    public static int makeBits(params int[] values)
    {
        int num_args = values.Length;

        int value = 0;
        int b0 = 0;

        for (int i = num_args; i > 1; i = i - 2)
        {
            int b = values[i - 2];
            int v = values[i - 1];

            int v2 = bits(v, 0, b);
            if (v != v2)
            {
                if (KSDebug.enableLog) KSDebug.LogError("makebits value out of bit range : " + v);
                return 0;
            }

            v = v << b0;
            value = value + v;
            b0 = b0 + b;
        }

        return value;
    }
    public static List<int> splitbits(int value, params int[] values)
    {
        List<int> list = new List<int>();
        int num_args = values.Length;
        int b0 = 0;
        for (int i = num_args; i > 1; i = i - 1)
        {
            int b = values[i - 1];
            list.Add(bits(value, b0, b));
            b0 = b0 + b;
        }
        list.Reverse();
        return list;
    }

    public static Vector4 getInverseMatrix(Vector2 x, Vector2 y)
    {
        Vector4 ret = new Vector4();
        float a = x.x;
        float b = x.y;
        float c = y.x;
        float d = y.y;

        float defA = a * d - b * c;
        Vector2 v0 = new Vector2(d, -c);
        Vector2 v1 = new Vector2(-b, a);
        v0 /= defA;
        v1 /= defA;

        ret.x = v0.x;
        ret.y = v0.y;
        ret.z = v1.x;
        ret.w = v1.y;
        return ret;
    }

    public static Vector2 ConvertVectorCoord(Vector4 iv, Vector2 v)
    {
        Vector2 v0 = new Vector2(iv.x, iv.y);
        Vector2 v1 = new Vector2(iv.z, iv.w);
        float xPos = Vector2.Dot(v0, v);
        float yPos = Vector2.Dot(v1, v);
        return new Vector2(xPos, yPos);
    }

    public static Vector2 ConvertVectorCoord(Vector2 x, Vector2 y, Vector2 v)
    {
        Vector4 iv = getInverseMatrix(x, y);
        Vector2 v0 = new Vector2(iv.x, iv.y);
        Vector2 v1 = new Vector2(iv.z, iv.w);
        float xPos = Vector2.Dot(v0, v);
        float yPos = Vector2.Dot(v1, v);
        return new Vector2(xPos, yPos);
    }

    public static Vector2 ConvertPointCoord(Vector2 o, Vector2 x, Vector2 y, Vector2 p)
    {
        Vector2 v = p - o;
        return ConvertVectorCoord(x, y, v);
    }

    public static bool isVectorInDiamond(Vector2 edge, Vector2 v)
    {
        Vector2 pos = ConvertVectorCoord(edge, new Vector2(-edge.x, edge.y), v);
        if (pos.x < 0 || pos.x >= 1 || pos.y < 0 || pos.y >= 1)
        {
            return false;
        }
        return true;
    }

    public static bool isPointInDiamond(Vector2 o, Vector2 x, Vector2 p)
    {
        Vector2 v = p - o;
        return isVectorInDiamond(x, v);
    }

    public static bool getSegmentRectIntersect(float ox, float oy, float ndx, float ndy, float r, float l, float miny, float maxy, ref float oix, ref float oiy)
    {
        if (ndx == 0)
        {
            return false;
        }

        float d = r - ox;
        float e = d / ndx;
        if (Mathf.Abs(e) <= Mathf.Abs(l))
        {
            if (e * l >= 0)
            {
                float t = e * ndy + oy;
                if (t >= miny && t <= maxy)
                {
                    oix = e;
                    oiy = t;
                    return true;
                }
            }
        }

        return false;
    }

    public static int getSegmentRectIntersect(Vector2 p0, Vector2 p1, Rect rect, ref Vector2[] op)
    {
        int index = 0;
        int maxIntersectCount = 2;
        if (rect.Contains(p0))
        {
            op[index] = p0;
            index++;
            maxIntersectCount--;
        }

        if (rect.Contains(p1))
        {
            op[index] = p1;
            index++;
            maxIntersectCount--;
        }

        if (maxIntersectCount <= 0)
        {
            return 2;
        }

        float x0 = p0.x;
        float y0 = p0.y;
        Vector2 d = p1 - p0;
        Vector2 nd = d.normalized;
        float l = d.magnitude;

        {
            float oix = 0;
            float oiy = 0;
            if (getSegmentRectIntersect(x0, y0, nd.x, nd.y, rect.xMin, l, rect.yMin, rect.yMax, ref oix, ref oiy))
            {
                op[index].x = oix;
                op[index].y = oiy;
                index++;
            }
        }

        if (index >= 2)
        {
            return index;
        }

        {
            float oix = 0;
            float oiy = 0;
            if (getSegmentRectIntersect(x0, y0, nd.x, nd.y, rect.xMax, l, rect.yMin, rect.yMax, ref oix, ref oiy))
            {
                op[index].x = oix;
                op[index].y = oiy;
                index++;
            }
        }

        if (index >= 2)
        {
            return index;
        }

        {
            float oix = 0;
            float oiy = 0;
            if (getSegmentRectIntersect(y0, x0, nd.y, nd.x, rect.yMin, l, rect.xMin, rect.xMax, ref oiy, ref oix))
            {
                op[index].x = oix;
                op[index].y = oiy;
                index++;
            }
        }

        if (index >= 2)
        {
            return index;
        }

        {
            float oix = 0;
            float oiy = 0;
            if (getSegmentRectIntersect(y0, x0, nd.y, nd.x, rect.yMax, l, rect.xMin, rect.xMax, ref oiy, ref oix))
            {
                op[index].x = oix;
                op[index].y = oiy;
                index++;
            }
        }

        return index;
    }

    public static Rect toRect(Vector2 p0, Vector2 p1)
    {
        float minX = Mathf.Min(p0.x, p1.x);
        float maxX = Mathf.Max(p0.x, p1.x);
        float minY = Mathf.Min(p0.y, p1.y);
        float maxY = Mathf.Max(p0.y, p1.y);
        Rect rect = new Rect(minX, minY, maxX - minX, maxY - minY);
        return rect;
    }

    public static bool isSegmenetRectIntersect(Vector2 p0, Vector2 p1, Rect rect)
    {
        Rect lineRect = toRect(p0, p1);
        if (isCloseIntersect(lineRect, rect))
        {
            return true;
        }
        else
        {
            return false;
        }

        if (rect.Contains(p0))
        {
            return true;
        }

        if (rect.Contains(p1))
        {
            return true;
        }


        float x0 = p0.x;
        float y0 = p0.y;
        Vector2 d = p1 - p0;
        Vector2 nd = d.normalized;
        float l = d.magnitude;

        {
            float oix = 0;
            float oiy = 0;
            if (getSegmentRectIntersect(x0, y0, nd.x, nd.y, rect.xMin, l, rect.yMin, rect.yMax, ref oix, ref oiy))
            {
                return true;
            }
        }

        {
            float oix = 0;
            float oiy = 0;
            if (getSegmentRectIntersect(x0, y0, nd.x, nd.y, rect.xMax, l, rect.yMin, rect.yMax, ref oix, ref oiy))
            {
                return true;
            }
        }

        {
            float oix = 0;
            float oiy = 0;
            if (getSegmentRectIntersect(y0, x0, nd.y, nd.x, rect.yMin, l, rect.xMin, rect.xMax, ref oiy, ref oix))
            {
                return true;
            }
        }

        {
            float oix = 0;
            float oiy = 0;
            if (getSegmentRectIntersect(y0, x0, nd.y, nd.x, rect.yMax, l, rect.xMin, rect.xMax, ref oiy, ref oix))
            {
                return true;
            }
        }

        return false;
    }

    public static float Cross2(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    public static float Vector2Angle(Vector2 a, Vector2 b)
    {
        float angle = Vector2.Angle(a, b);

        float cross = MathUtil.Cross2(a, b);
        if (cross < 0)
        {
            angle = -angle;
        }
        return angle;
    }

    public static float UniformAngle(float angle)
    {
        float remain = angle % 360;
        if (remain < 0)
        {
            remain += 360;
        }
        return remain;
    }

    //	public static bool isIntersect(Rect r0, Rect r1)
    //	{
    //		float maxMinX = Mathf.Max (r0.xMin, r1.xMin);
    //		float maxMinY = Mathf.Max (r0.yMin, r1.yMin);
    //		float minMaxX = Mathf.Min (r0.xMax, r1.xMax);
    //		float minMaxY = Mathf.Min (r0.yMax, r1.yMax);
    //		if(minMaxX > maxMinX && minMaxY > maxMinY)
    //		{
    //			return true;
    //		}
    //		return false;
    //	}

    //	public static bool isCloseIntersect(Rect r0, Rect r1)
    //	{
    //		float maxMinX = Mathf.Max (r0.xMin, r1.xMin);
    //		float maxMinY = Mathf.Max (r0.yMin, r1.yMin);
    //		float minMaxX = Mathf.Min (r0.xMax, r1.xMax);
    //		float minMaxY = Mathf.Min (r0.yMax, r1.yMax);
    //		if(minMaxX >= maxMinX && minMaxY >= maxMinY)
    //		{
    //			return true;
    //		}
    //		return false;
    //	}

    public static bool isIntersect(Rect r0, Rect r1)
    {
        Rect r = intersect(r0, r1);
        return isValid(r);
    }

    public static bool isCloseIntersect(Rect r0, Rect r1)
    {
        Rect r = intersect(r0, r1);
        return isCloseValid(r);
    }

    public static bool isValid(Rect r)
    {
        return r.xMax > r.xMin && r.yMax > r.yMin;
    }

    public static bool isCloseValid(Rect r)
    {
        return r.xMax >= r.xMin && r.yMax >= r.yMin;
    }

    public static Rect intersect(Rect r0, Rect r1)
    {
        float maxMinX = Mathf.Max(r0.xMin, r1.xMin);
        float maxMinY = Mathf.Max(r0.yMin, r1.yMin);
        float minMaxX = Mathf.Min(r0.xMax, r1.xMax);
        float minMaxY = Mathf.Min(r0.yMax, r1.yMax);
        Rect r = new Rect();
        r.xMin = maxMinX;
        r.xMax = minMaxX;
        r.yMin = maxMinY;
        r.yMax = minMaxY;
        return r;
    }

    public static void toRectPoint(float x, float y, float width, float height, out float ltx, out float lty, out float rtx, out float rty, out float lbx, out float lby, out float rbx, out float rby)
    {
        ltx = x - width - height;
        lty = y + width - height;

        rtx = x + width - height;
        rty = y - width - height;

        lbx = x - width + height;
        lby = y + width + height;

        rbx = x + width + height;
        rby = y - width + height;
    }

    public static void getRectLeftTopPoint(float x, float y, float width, float height, out float ltx, out float lty)
    {
        ltx = x - width - height;
        lty = y + width - height;
    }

    public static void toRectEdge(float x, float y, float width, float height, out float l, out float t, out float r, out float b)
    {
        t = x + y - 2 * height;
        b = x + y + 2 * height;
        l = x - y - 2 * width;
        r = x - y + 2 * width;
    }

    public static Rect toRectEdge(float x, float y, float width, float height)
    {
        float l = 0;
        float t = 0;
        float r = 0;
        float b = 0;
        toRectEdge(x, y, width, height, out l, out t, out r, out b);
        Rect rect = new Rect();
        rect.xMin = l;
        rect.xMax = r;
        rect.yMin = t;
        rect.yMax = b;
        return rect;
    }

    public static void toRectEdge(int x, int y, int width, int height, out int l, out int t, out int r, out int b)
    {
        t = x + y - 2 * height;
        b = x + y + 2 * height;
        l = x - y - 2 * width;
        r = x - y + 2 * width;
    }

    public static void toRegion(float l, float t, float r, float b, out float x, out float y, out float width, out float height)
    {
        float xay = (t + b) / 2;
        float x_y = (r + l) / 2;
        x = (xay + x_y) / 2;
        y = (xay - x_y) / 2;
        width = (r - x_y) / 2;
        height = (b - xay) / 2;
    }

    public static void toRectEdge(Rect rect, out int l, out int t, out int r, out int b)
    {
        toRectEdge((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, out l, out t, out r, out b);
    }

    public static void convertPoint(int x, int y, out int nx, out int ny)
    {
        ny = x + y;
        nx = x - y;
    }

    public static void GetCameraArea(ref Rect area)
    {
        Vector2 v0 = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z));
        Vector2 v1 = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, -Camera.main.transform.position.z));

        area.x = v0.x;
        area.y = v0.y;
        area.width = v1.x - v0.x;
        area.height = v1.y - v0.y;
    }

    public static int CeilToInt(float f)
    {
        float af = Mathf.Abs(f);
        int i = (int)af;
        float d = af - i;
        if (d >= 0.01)
        {
            //向上取整
            return Mathf.CeilToInt(f);
        }
        else
        {
            //截断取整
            return (int)f;
        }
    }
}