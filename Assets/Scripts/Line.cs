using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{

    const float verticalLineGradient = 1e5f;
    float gradiend;
    float y_intercept;
    float gradientPerpendicular;
    Vector2 pointOnLine_1;
    Vector2 pointOnLine_2;
    bool approachSide;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine){
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if(dx == 0){
            gradientPerpendicular = verticalLineGradient;
        }else{
            gradientPerpendicular = dy/dx;
        }

        if(gradientPerpendicular == 0){
            gradiend = verticalLineGradient;
        }else{
            gradiend = -1/gradientPerpendicular;
        }

        y_intercept = pointOnLine.y - gradiend * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2(1, gradiend);
        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }

    bool GetSide(Vector2 p){
        return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }

    public bool HasCrossedLine(Vector2 p){
        return GetSide(p) != approachSide;
    }

    public float DistanceFromPoint(Vector2 p){
        float yInterceptPerpendicular = p.y - gradientPerpendicular * p.x;
        float intersectX = (yInterceptPerpendicular - y_intercept)/(gradiend - gradientPerpendicular);
        float intersectY = gradiend * intersectX + y_intercept;
        return Vector2.Distance(p, new Vector2(intersectX, intersectY));
    }

    public void DrawWithGizmos(float length){
        Vector3 lineDir = new Vector3(1, 0, gradiend).normalized;
        Vector3 lineCenter = new Vector3(pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3.up;
        Gizmos.DrawLine(lineCenter - lineDir * length/2f, lineCenter + lineDir * length/2f);
    }

}
