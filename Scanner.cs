using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit[] targets;
    public Transform nearestTarget;

    void FixedUpdate()
    {
        targets = Physics.SphereCastAll(transform.position, scanRange, Vector3.up, 0, targetLayer);
        nearestTarget = GetNearest();
    }

    Transform GetNearest()
    {
        Transform result = null;
        float diff = 100;

        foreach (RaycastHit target in targets)
        {
            if (target.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (!enemy.isLive)
                    continue;

                Vector3 myPos = transform.position;
                Vector3 targetPos = target.transform.position;

                float curDiff = Vector3.Distance(myPos, targetPos);
                if (curDiff < diff)
                {
                    diff = curDiff;
                    result = target.transform;
                }
            }
            else if(target.transform.TryGetComponent<VamsuBoss>(out VamsuBoss boss))
            {
                if (!boss.isLive)
                    continue;

                Vector3 myPos = transform.position;
                Vector3 targetPos = target.transform.position;

                float curDiff = Vector3.Distance(myPos, targetPos);
                if (curDiff < diff)
                {
                    diff = curDiff;
                    result = target.transform;
                }
            }
        }

        return result;
    }

}
