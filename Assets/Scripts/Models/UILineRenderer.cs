using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    public List<Vector2> _points;
    
    private float unitWidth;
    private float unitHeight;
    public float thickness = 10f;

    public void ResetLines()
    {
        _points = new List<Vector2>();
    }
    

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        unitWidth = rectTransform.rect.width;
        unitHeight = rectTransform.rect.height;

        if (_points.Count < 2)
        {
            return;;
        }

        float angle = 0;

        for (int i = 0; i < _points.Count; i++)
        {
            Vector2 point = _points[i];
            if (i < _points.Count - 1)
            {
                angle = GetAngle(_points[i], _points[i + 1]) + 45f;
            }
            DrawVerticesForPoint(point,vh,angle);
        }

        for (int i = 0; i < _points.Count-1; i++)
        {
            int index = i * 2;
            vh.AddTriangle(index+0,index+1,index+3);
            vh.AddTriangle(index+3,index+2,index+0);
        }
    }

    float GetAngle(Vector2 me, Vector2 target)
    {
        return (float)(Mathf.Atan2(target.y - me.y, target.x - me.x) * (180 / Mathf.PI));
    }

    void DrawVerticesForPoint(Vector2 _point, VertexHelper vh,float angle)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = Quaternion.Euler(0,0,angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * _point.x, unitHeight * _point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0,0,angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * _point.x, unitHeight * _point.y);
        vh.AddVert(vertex);

    }
}
