/****************************************************
    文件：MonsterController.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/14 19:38:3
	功能：g怪物实体角色控制类
*****************************************************/

using UnityEngine;

public class MonsterController : Controller 
{
    private void Update()
    {
        if (isMove)
        {          
            SetDir();
            SetMove();
        }
    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
    private void SetMove()
    {
        ctrl.Move(transform.forward * Time.deltaTime * Constants.PlayerMoveSpeed);
        //解决资源错误
        ctrl.Move(Vector3.down * Time.deltaTime * Constants.PlayerMoveSpeed);
    }
}