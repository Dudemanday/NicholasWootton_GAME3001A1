using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    private Vector3 m_vPlayerPos;
    [SerializeField]
    private Vector3 m_vTargetPos;
    private Vector3 m_vInitialVel;

    [SerializeField]
    private float m_fKickPower;

    [SerializeField]
    private float m_fPlayerKickMaxDistance; //player needs to be less that this units, to be able to kick the ball

    [SerializeField]
    private bool m_bDubugKickBall = false;

    [SerializeField]
    private bool m_resetBall = false;

    [SerializeField]
    private bool m_debugMode = false;

    [SerializeField]
    private GameObject m_PlayerRef = null;

    [SerializeField]
    private GameObject m_CenterFieldRef = null;

    private Rigidbody m_rb = null;
    private GameObject m_targetDisplay = null;

    [SerializeField]
    private bool m_bIsGrounded = true;

    private float m_fDistanceToTargetYZ = 0.0f;
    private float m_fDistanceToTargetXY = 0.0f;

    private Vector3 vDebugHeading;

    private float m_fTimeInAir = 0;

    // Start is called before the first frame update
    void Start()
    {

        m_rb = GetComponent<Rigidbody>();
        Assert.IsNotNull(m_rb, "projectileComp is null");

        if (m_debugMode)
        {
            CreateLandingDisplay();
        }

        m_fDistanceToTargetYZ = (m_targetDisplay.transform.position - transform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        m_vPlayerPos = m_PlayerRef.transform.position;
        if (m_resetBall)
        {
            Reset();
            m_resetBall = !m_resetBall;
        }
        if (m_fTimeInAir == 20 && transform.position.y <= 0.6f)
        {
            m_bIsGrounded = true;
            m_targetDisplay.transform.position = new Vector3(0.0f, -1.0f, 0.0f);
        }
        //reset ball if it goes out of bounds
        if (transform.position.y <= 0)
        {
            Reset();
        }
        //kick ball using debug bool
        if (m_bDubugKickBall && m_bIsGrounded)
        {
            m_bDubugKickBall = false;
            OnKickBall();
        }
        //sets target offscreen if debug mode is off
        if (!m_debugMode)
        {
            m_targetDisplay.transform.position = new Vector3(0.0f, -1.0f, 0.0f);
        }
        //update m_fTimeInAir
        if (m_fTimeInAir < 20) m_fTimeInAir++;
    }

    private void CreateLandingDisplay()
    {
        m_targetDisplay = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_targetDisplay.transform.position = new Vector3(0.0f, -1.0f, 0.0f);
        m_targetDisplay.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        m_targetDisplay.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        m_targetDisplay.GetComponent<Renderer>().material.color = Color.red;
        m_targetDisplay.GetComponent<Collider>().enabled = false;
    }

    public void resetBall()
    {
        Reset();
    }

    public float getKickPower()
    {
        return m_fKickPower;
    }

    public void setKickPower(float kickPower)
    {
        m_fKickPower = kickPower;
    }

    public void toggleDebugMode()
    {
        m_debugMode = !m_debugMode;
    }

    public bool getDubug()
    {
        return m_debugMode;
    }

    private void Reset()
    {
        transform.position = m_CenterFieldRef.transform.position;
        m_rb.velocity = Vector3.zero;
    }

    public void OnKickBall()
    {
        if ((transform.position - m_vPlayerPos).magnitude < m_fPlayerKickMaxDistance
            && m_bIsGrounded)
        {
            m_bIsGrounded = false;
            m_fTimeInAir = 0;

            //Setup target position and debug heading line
            m_vTargetPos = transform.position + (transform.position - m_vPlayerPos) * m_fKickPower;
            m_vTargetPos.y = m_fKickPower;

            m_targetDisplay.transform.position = m_vTargetPos;
            vDebugHeading = m_vTargetPos - transform.position;


            //get velocity from distance

            //max height
            float fMaxHeight = m_targetDisplay.transform.position.y;

            //For Y-Axis and Z-Axis
            Vector3 distance = m_targetDisplay.transform.position - transform.position;
            distance.x = 0.0f;
            float m_fDistanceToTargetYZ = (distance).magnitude;

            float fRange = (m_fDistanceToTargetYZ * 2);
            float fTheta = Mathf.Atan((4 * fMaxHeight) / fRange);
            float fInitVelMagYZ = Mathf.Sqrt((2 * Mathf.Abs(Physics.gravity.y) * fMaxHeight)) / Mathf.Sin(fTheta);

            m_vInitialVel.y = fInitVelMagYZ * Mathf.Sin(fTheta);
            m_vInitialVel.z = fInitVelMagYZ * Mathf.Cos(fTheta);

            if (transform.position.z < m_vPlayerPos.z)
            {
                m_vInitialVel.z = -m_vInitialVel.z;
            }

            //For X-Axis
            distance = m_targetDisplay.transform.position - transform.position;
            distance.z = 0.0f;
            m_fDistanceToTargetXY = (distance).magnitude;

            float fRangeXY = (m_fDistanceToTargetXY * 2);
            float fThetaXY = Mathf.Atan((4 * fMaxHeight) / fRangeXY);
            float fInitVelMagXY = Mathf.Sqrt((2 * Mathf.Abs(Physics.gravity.y) * fMaxHeight)) / Mathf.Sin(fThetaXY);

            m_vInitialVel.x = fInitVelMagXY * Mathf.Cos(fThetaXY);

            if (transform.position.x < m_vPlayerPos.x)
            {
                m_vInitialVel.x = -m_vInitialVel.x;
            }

            m_rb.velocity = m_vInitialVel;

            //angle = Mathf.Atan((transform.position.y - m_vPlayerPos.y) / (transform.position.x - m_vPlayerPos.y)) * 1000;
            //m_targetDisplay.transform.rotation = Quaternion.Euler(90f, angle, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + vDebugHeading, transform.position);
    }
}


/// OLD V FROM PHYS PROF LAB
//calc distance
//m_fDistanceToTarget = (m_targetDisplay.transform.position - transform.position).magnitude;

//fMaxHeight = m_targetDisplay.transform.position.y;
//fRange = (m_fDistanceToTarget * 2);
//fTheta = Mathf.Atan((4 * fMaxHeight) / fRange);

//fInitVelMag = Mathf.Sqrt((2 * Mathf.Abs(Physics.gravity.y) * fMaxHeight)) / Mathf.Sin(fTheta);

//m_vInitialVel.y = fInitVelMag * Mathf.Sin(fTheta);
//m_vInitialVel.z = fInitVelMag * Mathf.Cos(fTheta);




//// MY ATTEMPT VERSION
//Vector3 m_vDisplacementPlayerBall = -(transform.position - m_vPlayerPos);
//float zDistance = Mathf.Abs(m_vTargetPos.z - transform.position.z);
//float zFactor = zDistance / m_vDisplacementPlayerBall.z;
//float endX = zFactor * m_vDisplacementPlayerBall.x + transform.position.x;
//m_vInitialVel.x = endX;