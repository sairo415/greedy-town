using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMaker : _ObjectsMakeBase
{
    public float m_startDelay;
    public int m_makeCount;
    public float m_makeDelay;
    public Vector3 m_randomPos;
    public Vector3 m_randomRot;
    public Vector3 m_randomScale;
    public bool isObjectAttachToParent = true;

    float m_Time;
    float m_Time2;
    float m_delayTime;
    float m_count;
    float m_scalefactor;


    void Start()
    {
        m_Time = m_Time2 = Time.time;
        m_scalefactor = VariousEffectsScene.m_gaph_scenesizefactor; //transform.parent.localScale.x; 
    }

    void OnEnable()
    {
        m_Time = m_Time2 = Time.time;
        m_count = 0;
        m_scalefactor = VariousEffectsScene.m_gaph_scenesizefactor; //transform.parent.localScale.x; 
    }



    void Update()
    {

        if (Time.time > m_Time + m_startDelay)
        {
            if (Time.time > m_Time2 + m_makeDelay && m_count < m_makeCount)
            {
                Vector3 m_pos = transform.position + GetRandomVector(m_randomPos)* m_scalefactor; 
                Quaternion m_rot = transform.rotation * Quaternion.Euler(GetRandomVector(m_randomRot));
                

                for (int i = 0; i < m_makeObjs.Length; i++)
                {
                    if (m_makeObjs[i] == null)
                        continue;

                    GameObject m_obj = Instantiate(m_makeObjs[i], m_pos, m_rot);

                    Vector3 m_scale = (m_makeObjs[i].transform.localScale + GetRandomVector2(m_randomScale));
                    if (isObjectAttachToParent)
                    {
                        m_obj.transform.parent = this.transform;
                    }
                    Transform parent = transform.parent;
                    while (!parent.TryGetComponent(out Weapon wea))
                    {
                        parent = parent.parent;
                    }
                    Weapon pweapon = parent.GetComponent<Weapon>();
                    Particle[] particles = m_obj.GetComponentsInChildren<Particle>();
                    foreach (Particle particle in particles)
                    {
                        particle.weapon = pweapon;
                    }
                    Hammer[] hammers = m_obj.GetComponentsInChildren<Hammer>();
                    foreach (Hammer hammer in hammers)
                    {
                        hammer.Init(pweapon.damage, -1);
                    }

                    m_obj.transform.localScale = m_scale;
                }

                m_Time2 = Time.time;
                m_count++;
            }
        }
    }
}
