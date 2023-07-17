using UnityEngine;

/// <summary>
/// 用于计算数学方法
/// </summary>
public static class MathfHelp
{
    /// <summary>
    /// CalcBoxVertex 计算得到的顶点，绘制序列，4个为一个面，共6个面，顺序为上下左右前后
    /// </summary>
    /// <returns></returns>
    public static int[] GetBoxSurfaceIndex()
    {
        return new int[] {
                0,1,2,3,//上
                4,5,6,7,//下
                2,6,5,3,//左
                0,4,7,1,//右
                1,7,6,2,//前
                0,3,5,4//后
            };
    }

    /// <summary>
    /// 计算box 的 8个顶点
    /// <para>顺时针顶面 [0,1,2,3]</para>
    /// <para>顺时针底面 [4,5,6,7]</para>
    /// </summary>
    private static Vector3[] CalcBoxVertex(Vector3 size)
    {
        Vector3 halfSize = size / 2f;

        Vector3[] points = new Vector3[8];

        //上面-顺时针
        //  3 → 0
        //  ↑      ↓
        //  2 ← 1
        points[0] = new Vector3(halfSize.x, halfSize.y, halfSize.z);
        points[1] = new Vector3(halfSize.x, halfSize.y, -halfSize.z);
        points[2] = new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
        points[3] = new Vector3(-halfSize.x, halfSize.y, halfSize.z);

        //下面-顺时针
        //  5 ← 4
        //  ↓      ↑
        //  6 → 7
        points[4] = new Vector3(halfSize.x, -halfSize.y, halfSize.z);
        points[5] = new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
        points[6] = new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
        points[7] = new Vector3(halfSize.x, -halfSize.y, -halfSize.z);

        return points;
    }

    /// <summary>
    /// 计算box 的 8个顶点
    /// </summary>
    /// <param name="size"></param>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static Vector3[] CalcBoxVertex(Vector3 size, Matrix4x4 matrix)
    {
        Vector3[] points = CalcBoxVertex(size);

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = matrix.MultiplyPoint(points[i]);
        }

        return points;
    }
}