/****************************************************
    文件：PlayerController.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/1 17:41:36
	功能：玩家表现实体控制器
*****************************************************/

using UnityEngine;

public class PlayerController : Controller
{
    private Vector3 camOffset;    
    public GameObject skill1FX;
    public GameObject skill2FX;
    public GameObject skill3FX;
    public GameObject normalAtk1FX;
    public GameObject normalAtk2FX;
    public GameObject normalAtk3FX;
    public GameObject normalAtk4FX;
    public GameObject normalAtk5FX;
    private float targetBlend;
    private float currentBlend;

    public override void Init()
    {
        base.Init();
        camTrans = Camera.main.transform;
        camOffset = transform.position - camTrans.position;
        #region 初始化技能特效
        if (skill1FX != null)
        {
            fxDict.Add(skill1FX.name,skill1FX);
        }
        if (skill2FX != null)
        {
            fxDict.Add(skill2FX.name, skill2FX);
        }
        if (skill3FX != null)
        {
            fxDict.Add(skill3FX.name, skill3FX);
        }
        if (normalAtk1FX!= null)
        {
            fxDict.Add(normalAtk1FX.name, normalAtk1FX);
        }
        if (normalAtk2FX != null)
        {
            fxDict.Add(normalAtk2FX.name, normalAtk2FX);
        }
        if (normalAtk3FX != null)
        {
            fxDict.Add(normalAtk3FX.name, normalAtk3FX);
        }
        if (normalAtk4FX != null)
        {
            fxDict.Add(normalAtk4FX.name, normalAtk4FX);
        }
        if (normalAtk5FX != null)
        {
            fxDict.Add(normalAtk5FX.name, normalAtk5FX);
        }
        #endregion

    }

    private void Update()
    {    
        if (currentBlend != targetBlend)
        {
            UpdateMixBlend();
        }

        if (isMove)
        {
            ani.applyRootMotion = true;
            //设置方向
            SetDir();
            //产生移动
            SetMove();
            //相机跟随
            SetCam();            
        }
        if (isAct)
        {
            ani.applyRootMotion = true;
        }
        if (skillMove)
        {
            ani.applyRootMotion = true;
            SetSkillMove();
            SetCam();
        }

    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1)) +camTrans.eulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    private void SetMove()
    {
        ctrl.Move(transform.forward * Time.deltaTime * Constants.PlayerMoveSpeed);
    }
    private void SetSkillMove()
    {      
        ctrl.Move(transform.forward * Time.deltaTime * skillMoveSpeed);
    }

    public void SetCam()
    {
        if (camTrans != null)
        {
            camTrans.position = transform.position - camOffset;
        }
    }
    private void UpdateMixBlend()
    {
        if (Mathf.Abs(currentBlend - targetBlend) < Constants.AccelerSpeed * Time.deltaTime)
        {
            currentBlend = targetBlend;
        }
        else if (currentBlend > targetBlend)
        {
            currentBlend -= Constants.AccelerSpeed * Time.deltaTime;
        }
        else
        {
            currentBlend += Constants.AccelerSpeed * Time.deltaTime;
        }
        ani.SetFloat("Blend", currentBlend);
    }


    public override void SetBlend(float blend)
    {
        targetBlend = blend;
    }

    public override void SetFX(string name, float destroy)
    {
        GameObject go;
        if(fxDict.TryGetValue(name,out go))
        {
            go.SetActive(true);         
            timerSvc.AddTimeTask((int tid) =>
            {
                go.SetActive(false);
            },destroy);
        }
    }


}