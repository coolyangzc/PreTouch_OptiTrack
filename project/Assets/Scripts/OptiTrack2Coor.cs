using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptiTrack2Coor : MonoBehaviour {


    public DebugInfo debugInfo;
    
    Vector3 O, X, Y, Z;
    Vector3[] coor = new Vector3[3];
    
    float[][] gauss = new float[3][], a = new float[3][];
    bool[] inMin = new bool[3], inMax = new bool[3];
    int minI, minJ, maxI, maxJ;
    const int n = 3;
    const float eps = 1e-8f;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < 3; ++i) {
            a[i] = new float[4];
            gauss[i] = new float[3];
        }
    }
    
    public Vector3 ToCoor(Vector3 opti) {
        opti -= O;

        for(int i=0; i<3; ++i) {
            for(int j=0; j<3; ++j) 
                a[i][j] = gauss[i][j];
            a[i][3] = opti[i];
        }
        for(int i=0; i<n; ++i) 
        {
            int k = 0;
            for(int j=0; j<n; ++j) 
                if (Mathf.Abs(a[i][j]) > Mathf.Abs(a[i][k])) k = j;
            float w = a[i][k];
            for(int j=0; j<n+1; ++j) 
                a[i][j] /= w;
            for(int ii=0; ii<n; ++ii) 
                if (ii != i)
                {
                    w = a[ii][k];
                    if (Mathf.Abs(w) > eps)
                        for(int j=0; j<n+1; ++j) 
                                a[ii][j] -= a[i][j] * w;
                }
        }

        for(int i=0; i<n; ++i) 
            for(int j=0; i<n; ++j) 
                if (Mathf.Abs(a[i][j]) > eps)
                {
                    opti[j] = a[i][n];
                    break;
                }
        return opti;
    }
    
    public void UpdateAxis(List<Vector3> rb) {
        if (rb.Count < 3)
            return;
        float minDist = 1e10f;
        float maxDist = 0f;
        
        
        for (int i = 0; i < 3; ++i) {
            inMin[i] = inMax[i] = false;
            for (int j = i+1; j < 3; ++j) {
                float dist = Vector3.Distance(rb [i], rb [j]);
                if (dist < minDist) {
                    minDist = dist;
                    minI = i;
                    minJ = j;
                }
                if (dist > maxDist) {
                    maxDist = dist;
                    maxI = i;
                    maxJ = j;
                }
            }
        }
        inMin[minI] = true;
        inMin[minJ] = true;
        inMax[maxI] = true;
        inMax[maxJ] = true;
        for (int i = 0; i < 3; ++i)
            if (inMin[i] && inMax[i])
                coor[0] = rb[i];
            else if (inMin[i] && !inMax[i])
                coor[1] = rb[i];
            else if (!inMin[i] && inMax[i])
                coor[2] = rb[i];
        X = coor[1] - coor[0];
        Y = coor[2] - coor[0];
        Z = Vector3.Cross(Y, X);
        Y = Vector3.Cross(X, Z);
        X.Normalize();
        Y.Normalize();
        Z.Normalize();
        O = coor[0];
        
        for (int i = 0; i < 3; ++i) {
            gauss[i][0] = X[i];
            gauss[i][1] = Y[i];
            gauss[i][2] = Z[i];
        }
        for (int i = 0; i < 3; ++i) {
            coor[i] = ToCoor(coor[i]);
            debugInfo.Log(i.ToString(), coor[i].x.ToString("f2") + ',' + coor[i].y.ToString("f2") + ',' + coor[i].z.ToString("f2"));
        }
    }
}
