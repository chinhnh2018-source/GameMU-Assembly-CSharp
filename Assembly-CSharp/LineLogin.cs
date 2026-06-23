using System;
using System.Text;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Protocol;
using HSGameEngine.GameEngine.Scene;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class LineLogin : GBasePart
{
	public LineLogin()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitControls();
		this.UserControl_Loaded(null);
	}

	public override void Destroy()
	{
		this.BackgroundCenter.Destroy();
		this.LinePanelImage.Destroy();
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			U3DUtils.AS<LineListItem>(this.ItemCollection[i]).Destroy();
		}
	}

	protected override void InitControls()
	{
		this.Container.Children.Add(this.BackgroundCenter);
		this.Container.Children.Add(this.LinePanelImage);
		this.LinePanelImage.IsHitTestVisible = false;
		Canvas.SetLeft(this.LinePanelImage, 376);
		Canvas.SetTop(this.LinePanelImage, 89);
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 159.0;
		this.listBox.Height = 266.0;
		Canvas.SetLeft(this.listBox, 448);
		Canvas.SetTop(this.listBox, 160);
		this.listBox.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.listBox_MouseLeftButtonUp);
	}

	public override void CleanUpChildWindows()
	{
	}

	private void SocketConnect(object sender, MUSocketConnectEventArgs e)
	{
		switch (e.NetSocketType)
		{
		case 0:
			if (e.Error == string.Empty + 0)
			{
				string text = string.Empty;
				text = StringUtil.substitute("{0}:{1}:{2}", new object[]
				{
					"1345i",
					Global.Data.UserID,
					20140624
				});
				byte[] bytes = new UTF8Encoding().GetBytes(text);
				TCPOutPacket tcpoutPacket = new TCPOutPacket();
				tcpoutPacket.PacketCmdID = 11000;
				tcpoutPacket.FinalWriteData(bytes, 0, bytes.Length);
				this.tcpClient.SendData(tcpoutPacket);
			}
			else
			{
				this.ActiveDisconnect = true;
				Super.HideNetWaiting();
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), StringUtil.substitute(Global.GetLang("连接游戏线路服务器失败"), new object[0]), -1, -1, -1, -1, false);
			}
			break;
		case 1:
			this.ActiveDisconnect = true;
			Super.HideNetWaiting();
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), StringUtil.substitute(Global.GetLang("向游戏线路服务器发送信息失败"), new object[0]), -1, -1, -1, -1, false);
			break;
		case 2:
			break;
		case 3:
			GScene.ServerStopGame();
			if (!this.ActiveDisconnect)
			{
				Super.HideNetWaiting();
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), StringUtil.substitute(Global.GetLang("与游戏线路服务器的连接被断开"), new object[0]), -1, -1, -1, -1, false);
			}
			break;
		case 4:
			this.ActiveDisconnect = true;
			this.tcpClient.Disconnect(2);
			if (e.bytesData != null)
			{
				ServerListData serverListData = DataHelper.BytesToObject<ServerListData>(e.bytesData, 0, e.bytesData.Length);
				if (serverListData == null || serverListData.LineDataList == null || serverListData.LineDataList.Count <= 0)
				{
					Super.HideNetWaiting();
					Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), Global.GetLang("获取到的线路列表为空"), -1, -1, -1, -1, false);
				}
				else if (serverListData.RetCode < 0)
				{
					string message = string.Empty;
					if (serverListData.RetCode == -1)
					{
						message = StringUtil.substitute(Global.GetLang("登陆游戏服务器时失败, 客户端的版本太旧，请清空IE的缓存后再重新登陆!"), new object[0]);
					}
					else
					{
						message = StringUtil.substitute(Global.GetLang("登陆的用户名已经在线，请稍后刷新重新登陆!"), new object[0]);
					}
					Super.HideNetWaiting();
					Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), message, -1, -1, -1, -1, true);
				}
				else
				{
					int oldLineID = -1;
					if (Global.CurrentListData != null)
					{
						oldLineID = Global.CurrentListData.LineID;
						Global.CurrentListData = null;
					}
					Global.LineDataList = serverListData.LineDataList;
					Global.LineDataList.Sort(new Comparison<LineData>(this.ItemsList_Sort));
					Super.HideNetWaiting();
					this.tcpClient.SocketConnect -= this.SocketConnect;
					this.tcpClient.Destroy();
					this.tcpClient = null;
					this.ShowLineListBox(oldLineID);
					if (((serverListData.RolesCount <= 0 && Super.GData.FirstEnterGameServer) || Global.LineDataList.Count <= 1) && !Super.ConnectToGameServerFailed && Global.LineDataList != null && Global.LineDataList.Count > 0)
					{
						Global.CurrentListData = Global.LineDataList[0];
						string xapParamByName = Super.GetXapParamByName("serverip", "127.0.0.1");
						Global.CurrentListData.GameServerIP = xapParamByName;
						if (Global.CurrentListData != null && this.LoginGameToPlay != null)
						{
							this.LoginGameToPlay.Invoke(this, EventArgs.Empty);
						}
					}
				}
			}
			else
			{
				Super.HideNetWaiting();
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), Global.GetLang("获取到的线路列表为空"), -1, -1, -1, -1, false);
			}
			break;
		default:
			throw new Exception(Global.GetLang("错误的Socket操作类型"));
		}
	}

	private int ItemsList_Sort(LineData a, LineData b)
	{
		return a.OnlineCount - b.OnlineCount;
	}

	private void UserControl_Loaded(SpriteSL sender)
	{
		this.BackgroundCenter.URL = "NetImages/LoginRes/Images/Plate/Loading.jpg";
		this.LinePanelImage.URL = "NetImages/LoginRes/Images/Plate/panel_selectLine.jpg";
		this.tcpClient.SocketConnect += this.SocketConnect;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 106.0;
		gicon.Height = 30.0;
		gicon.BodySource = new ImageBrush(Global.GetLoginResImage("Images/Plate/LoadBtn_Normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetLoginResImage("Images/Plate/LoadBtn_Hover.png"));
		gicon.TextSize = 14.0;
		gicon.TextColor = new SolidColorBrush(uint.MaxValue);
		gicon.Text = Global.GetLang("进入");
		Canvas.SetLeft(gicon, 476);
		Canvas.SetTop(gicon, 457);
		this.Container.Children.Add(gicon);
		string loginIP = string.Empty;
		gicon.MouseLeftButtonUp = delegate(object s1, MouseEvent e1)
		{
			if (this.lineListItem == null)
			{
				return;
			}
			Global.CurrentListData = (this.lineListItem.Tag as LineData);
			if (Global.CurrentListData == null)
			{
				return;
			}
			loginIP = Super.GetXapParamByName("serverip", "127.0.0.1");
			Global.CurrentListData.GameServerIP = loginIP;
			if (this.LoginGameToPlay != null)
			{
				this.LoginGameToPlay.Invoke(null, EventArgs.Empty);
			}
		};
		loginIP = Super.GetXapParamByName("serverip", "127.0.0.1");
		this.tcpClient.Connect(loginIP, Global.GetLineServerPort());
		Super.ShowNetWaiting(Global.GetLang("正在连接线路服务器..."));
	}

	private void ShowLineListBox(int oldLineID = -1)
	{
		this.lineListItem = null;
		this.ItemCollection.Clear();
		if (Global.LineDataList == null)
		{
			return;
		}
		int selectedIndex = 0;
		for (int i = 0; i < Global.LineDataList.Count; i++)
		{
			if (Global.LineDataList[i].LineID > 0 && Global.LineDataList[i].LineID < 11)
			{
				LineListItem lineListItem = U3DUtils.NEW<LineListItem>();
				lineListItem.BodyWidth = 159.0;
				lineListItem.BodyHeight = 36.0;
				lineListItem.Tag = Global.LineDataList[i];
				lineListItem.Tip = Global.GetLang("线路依照在线人数排序\n排序越靠后的线路会越拥挤\n我们推荐您进入较为顺畅的线路");
				lineListItem.LineTextBlock.TextColor = Super.GetLineDataBrush(Global.LineDataList[i]);
				lineListItem.LineTextBlock.Text = StringUtil.substitute(Global.GetLang("{0}线（{1}）"), new object[]
				{
					this.LineNames[Global.LineDataList[i].LineID - 1],
					Super.GetLineDataText(Global.LineDataList[i])
				});
				this.ItemCollection.AddNoUpdate(lineListItem);
				if (oldLineID == Global.LineDataList[i].LineID)
				{
					selectedIndex = i;
				}
				lineListItem.addEventListener("doubleClick", new MouseEventHandler(this.listItem_MouseLeftButtonDown));
			}
		}
		this.ItemCollection.DelayUpdate();
		this.listBox.SelectedIndex = selectedIndex;
		this.SelectionChanged();
	}

	private void listItem_MouseLeftButtonDown(MouseEvent e1)
	{
		if (this.lineListItem == null)
		{
			return;
		}
		Global.CurrentListData = (this.lineListItem.Tag as LineData);
		if (Global.CurrentListData == null)
		{
			return;
		}
		string xapParamByName = Super.GetXapParamByName("serverip", "127.0.0.1");
		Global.CurrentListData.GameServerIP = xapParamByName;
		if (this.LoginGameToPlay != null)
		{
			this.LoginGameToPlay.Invoke(this, EventArgs.Empty);
		}
	}

	private void SelectionChanged()
	{
		LineListItem lineListItem = U3DUtils.AS<LineListItem>(this.listBox.SelectedItem);
		if (null == lineListItem)
		{
			return;
		}
		if (this.lineListItem != null)
		{
			this.lineListItem.SelectedState = false;
		}
		this.lineListItem = lineListItem;
		this.lineListItem.SelectedState = true;
	}

	private void listBox_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		Point point = new Point(e.localX, e.localY);
		if (point.X < 0 || point.Y < 0 || (double)point.X >= this.listBox.Width || (double)point.Y >= this.listBox.Height)
		{
			return;
		}
		this.SelectionChanged();
	}

	private URLImage BackgroundCenter = new URLImage();

	private URLImage LinePanelImage = new URLImage();

	private ListBox listBox = new ListBox();

	public ObservableCollection ItemCollection;

	public EventHandler LoginGameToPlay;

	private TCPClient tcpClient = new TCPClient(2);

	private bool ActiveDisconnect;

	private string[] LineNames = new string[]
	{
		Global.GetLang("一"),
		Global.GetLang("二"),
		Global.GetLang("三"),
		Global.GetLang("四"),
		Global.GetLang("五"),
		Global.GetLang("六"),
		Global.GetLang("七"),
		Global.GetLang("八"),
		Global.GetLang("九"),
		Global.GetLang("十")
	};

	private LineListItem lineListItem;
}
