using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField]
    private BallPhysics m_pBallRef = null;

    [SerializeField]
    private TMP_Text KickPowerValue; 

    Slider kickPowerSlider;

    // Use this for initialization
    void Start()
    {
        GameObject slider = GameObject.Find("KickPowerSlider");
        if (slider != null)
        {
            kickPowerSlider = slider.GetComponent<Slider>();
        }
        kickPowerSlider.value = m_pBallRef.getKickPower();
    }

    // Update is called once per frame
    void Update()
    {
        KickPowerValue.SetText("" + kickPowerSlider.value);
        //m_pBallRef.setKickPower(kickPowerSlider.value);
    }
}
