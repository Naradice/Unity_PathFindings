using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    const float minPathUpdateTime = 0.2f;
    const float pathUpdateMoveThreshold = 0.5f;
    public Transform target;
    public float speed = 10;
    public float trunDist = 5;
    public float turnSpeed = 3;
    public float stoppingDst = 10;

    Road path;

    void Start(){
        StartCoroutine("UpdatePath");
    }

    void OnPathFound(Vector3[] waypoints, bool pathSuccessful){
        if(pathSuccessful){
            path = new Road(waypoints, transform.position, trunDist, stoppingDst);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath(){

        if(Time.timeSinceLevelLoad < 0.3f){
            yield return new WaitForSeconds(0.3f);
        }
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;
        
        while(true){
            yield return new WaitForSeconds(minPathUpdateTime);
            if((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold){
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                targetPosOld = target.position;
            }
        }
    }

    IEnumerator FollowPath() {
        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.lookPoints[0]);

        float sppedPercent = 1;

        while(followingPath){
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while(path.turnBoundaries[pathIndex].HasCrossedLine(pos2D)){
                if(pathIndex == path.finishLineIndex){
                    followingPath = false;
                    break;
                }else{
                    pathIndex++;
                }
            }

            if(followingPath){

                if(pathIndex >= path.slowDownIndex){
                    sppedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                    if(sppedPercent < 0.01f){
                        followingPath = false;
                    }
                }
                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                //transform.rotation = targetRotation;
                transform.Translate(Vector3.forward * Time.deltaTime * speed * sppedPercent, Space.Self);
            }
            yield return null;
        }
    }

    public void OnDrawGizmos(){
        if(path != null){
            path.DrowWithGizmos();
        }
    }
}
