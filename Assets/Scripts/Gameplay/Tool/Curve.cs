using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Dremu.Gameplay.Tool {

    public sealed class Curve {

        const int PrecisionPerPart = 300;

        readonly List<Vector2> points = new List<Vector2>();

        

        /// <summary>
        /// 初始化曲线
        /// </summary>
        /// <param name="Points">端点，数量应为贝塞尔曲线数量+1</param>
        /// <param name="Nodes">各条贝塞尔曲线的节点（非常，非常，非常建议每条贝塞尔曲线的阶数都不要超过10，要不计算能干死你）</param>
        public Curve(List<Vector2> Points, List<List<Vector2>> Nodes) {
            List<List<Vector2>> Parts = new List<List<Vector2>>();
            for (int i = 0; i < Points.Count - 1; i++) {
                List<Vector2> Bessel = new List<Vector2> { Points[i] };
                Bessel.AddRange(Nodes[i]);
                Bessel.Add(Points[i + 1]);
                Parts.Add(Bessel); 
            }

            int LastPrecision = PrecisionPerPart - 1;
            int perPrecision = Mathf.CeilToInt(1f * (PrecisionPerPart - 1) / Parts.Count);

            foreach (var part in Parts) {
                int Precision = Mathf.Min(perPrecision, LastPrecision);
                int[] Tri = new int[part.Count];
                long[] Fact = new long[part.Count];
                //计算杨辉三角
                Fact[0] = 1;
                for (int i = 1; i < part.Count; i++) Fact[i] = Fact[i - 1] * i;

                for (int i = 0; i < part.Count; i++)
                {
                    Tri[i] = (int)(Fact[part.Count - 1] / Fact[part.Count - 1 - i] / Fact[i]);
                }

                for (int i = 0; i < Precision; i++) {
                    Vector2 point = Vector2.zero;
                    float t = 1f * i / Precision;

                    //计算贝塞尔曲线
                    for (int j = 0; j < part.Count; j++)
                        point += Tri[j] * part[j] * Mathf.Pow(1 - t, part.Count - j - 1) * Mathf.Pow(t, j);

                    points.Add(point);
                }

                LastPrecision -= Precision;
            }

            points.Add(Points[Points.Count - 1]);
        }

        public Curve(List<Vector2> points)
        {
            this.points = points;
        }



        /// <summary>
        /// 获取曲线的近似微分点组
        /// </summary>
        /// <returns>点组</returns>
        public List<Vector2> GetPoints() {
            return points;
        }
        
        /// <summary>
        /// 获取子曲线（默认直线）
        /// </summary>
        /// <param name="start">起始点（0~1）</param>
        /// <param name="end">终止点（0~1）</param>
        /// <returns>点组</returns> 
        public List<Vector2> SubCurveByStartAndEnd(float start, float end, EaseTypeEnumer.EaseType easeType = EaseTypeEnumer.EaseType.LINEAR) {
            List<Vector2> points_ = new List<Vector2>();
            int pointCount = Mathf.Max(Mathf.FloorToInt(Mathf.Abs(end - start) * points.Count), 10);
            List<float> easeDistList = EaseTypeEnumer.GetEase(end - start, pointCount, easeType);
            for (int i = 0; i < pointCount; i++)
                points_.Add(GetPoint(start + easeDistList[i]));
            Vector2 startPoint = points_[0];
            for (int i = 0; i < points_.Count; i++)
                points_[i] -= startPoint;
            return points_;
        }

        /// <summary>
        /// 获取子曲线
        /// </summary>
        /// <param name="mid">中点（0~1）</param>
        /// <param name="length">长度（0~1）</param>
        /// <returns>点组</returns>
        public List<Vector2> SubCurveByMidAndLength( float mid, float length ) {
            float start = mid - length / 2;
            float end = mid + length / 2;
            List<Vector2> points_ = new List<Vector2>();
            int pointCount = (int) Mathf.Max((end - start) * points.Count, 10);
            for (float i = 0; i <= pointCount; i++)
                points_.Add(GetPoint(start + (end - start) * i / pointCount));
            Vector2 midPoint = points_[points_.Count / 2];
            for (int i = 0; i < points_.Count; i++)
                points_[i] -= midPoint;
            return points_;
        }

        /// <summary>
        /// 获取点
        /// </summary>
        /// <param name="at">位置</param>
        /// <returns>点</returns>
        public Vector2 GetPoint( float at ) {
            int left, right;
            float pos;
            if (points.Count * at >= points.Count - 1) {
                left = points.Count - 1;
                right = points.Count - 1;
                pos = 0;
            }
            else {
                left = Mathf.FloorToInt(points.Count * at);
                right = Mathf.FloorToInt(points.Count * at) + 1;
                pos = points.Count * at - left;
            }

            Vector2 OriginalVector = points[right] - points[left];
            Vector2 Origin = OriginalVector * pos + points[left];
            return Origin;
        }

        /// <summary>
        /// 获取法线
        /// </summary>
        /// <param name="at">位置</param>
        /// <returns>法线（key为原点，value为方向）</returns>
        public KeyValuePair<Vector2, Vector2> GetNormal( float at ) {
            int left, right;
            float pos;
            if (points.Count * at >= points.Count - 1) {
                left = points.Count - 1;
                right = points.Count - 1;
                pos = 0;
            }
            else {
                left = Mathf.FloorToInt(points.Count * at);
                right = Mathf.FloorToInt(points.Count * at) + 1;
                pos = points.Count * at - left;
            }

            Vector2 OriginalVector = points[right] - points[left];
            Vector2 Origin = OriginalVector * pos + points[left];
            Vector2 Direction = new Vector2(-OriginalVector.y, OriginalVector.x).normalized;
            return new KeyValuePair<Vector2, Vector2>(Origin, Direction);
        }

        /// <summary>
        /// 取两条曲线的线性插值
        /// </summary>
        /// <param name="start">起始曲线</param>
        /// <param name="end">结束曲线</param>
        /// <param name="t"></param>
        /// <returns>插值</returns>
        public static Curve CurveLerp(Curve start, Curve end, float t ) {
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < PrecisionPerPart; i++)
                points.Add(Vector2.Lerp(start.points[i], end.points[i], t));
            return new Curve(points);
        }
        
        

    }

}
