
/****************************************************
	文件：ServerStart.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/04/24 15:03   	
	功能：服务器入口
*****************************************************/


using System.Threading;

class ServerStart
{
	static void Main(string[] args)
	{
		ServerRoot.Instance.Init();
		//保证进程不会退出
		while (true)
		{
			ServerRoot.Instance.Update();
			Thread.Sleep(20);
		}
	}
}

