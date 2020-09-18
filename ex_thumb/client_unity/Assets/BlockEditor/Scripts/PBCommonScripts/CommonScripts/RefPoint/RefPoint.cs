using UnityEngine;

public enum RefPointType
{
    male = 0,
    female,
    
    male71,
    female71,
 
    male50,
    female50,
    hole50,
    cross50,
    axis50,
    claw50,
   
    male34,
    female34,
    claw34,
    axis34,
}

[ExecuteInEditMode]
[SelectionBase]
public class RefPoint : MonoBehaviour
{
    public PEBlockAlign blockAlign;
    public RefPointType refPointType;

    private Transform mTrans;

    private Vector3 initPos;
    private Vector3 initAngle;

    private void OnEnable()
    {
        mTrans = transform;
        initPos = mTrans.localPosition;
        initAngle = mTrans.localEulerAngles;
    }

    void Update()
    {
        mTrans.localPosition = initPos;
        mTrans.localEulerAngles = initAngle;
    }
}