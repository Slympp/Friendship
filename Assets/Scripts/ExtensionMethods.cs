using UnityEngine;

public static class ExtensionMethods {

    public static float Normalize(this float v, float a, float b, float min, float max) {
        return (b - a) * ((v - min) / (max - min)) + a;
    }
    
    public static Vector2 NearestPointOnLine(Vector2 linePnt, Vector2 lineDir, Vector2 pnt) {
        lineDir.Normalize();
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }
    
    public static Vector2 NearestPointOnFiniteLine(this Vector2 pnt, Vector2 start, Vector2 end) {
        var line = end - start;
        var len = line.magnitude;
        line.Normalize();
   
        var v = pnt - start;
        var d = Vector2.Dot(v, line);
        d = Mathf.Clamp(d, 0f, len);
        return start + line * d;
    }
}