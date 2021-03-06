﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkerPathPreview : MonoBehaviour {

    [SerializeField]
    private LineRenderer lr = null;
    [SerializeField]
    private PathfindingAgent moveHelper = null;
    [SerializeField]
    private float zValue = -1;

    private void Update() {
        if(!Pause.isPaused()) {
            if(this.moveHelper != null && this.moveHelper.hasPath()) {
                this.lr.enabled = true;

                NavPath path = this.moveHelper.path;

                List<Vector3> points = new List<Vector3>();
                points.Add(this.transform.position - Vector3.forward);

                for(int i = path.targetIndex; i < path.pointCount; i++) {
                    points.Add((Vector3)path.getPoint(i).worldPos + Vector3.forward * this.zValue);
                }

                this.lr.positionCount = points.Count;
                Vector3[] a = points.ToArray();
                Array.Reverse(a);
                this.lr.SetPositions(a);

            } else {
                this.lr.enabled = false;
            }
        }
    }
}
