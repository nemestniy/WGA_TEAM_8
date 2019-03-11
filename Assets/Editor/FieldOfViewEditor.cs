using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor {
    
    private void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView) target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.up, 360, fow._currentFarViewRadius); //draw far view radius 
//        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.up, 360, fow._closeViewRadius); //draw close view radius 
        Vector3 viewAngleA = fow.DirFromAngle(-fow._currentViewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow._currentViewAngle / 2, false);
        
        //draw view angles
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow._currentFarViewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow._currentFarViewRadius);
    }
}
