using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class TaskOverPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (this.Btn_OK != null)
		{
			this.Btn_OK.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(null, null);
			};
		}
	}

	private void InitTextInPrefabs()
	{
		this.Btn_OK.Text = Global.GetLang("确 认");
	}

	public string Content
	{
		get
		{
			return this._content;
		}
		set
		{
			this._content = value;
			this.Tex_Content.text = this.Content;
		}
	}

	public void ShowAddContent(int zhangjieID)
	{
		TaskZhangJieVO taskZhangJieVO = ConfigTasks.GetTaskZhangJieVO(zhangjieID);
		if (taskZhangJieVO != null)
		{
			this.ZhangjieTitle.URL = Global.GetGameResTaskZhangJieTitle(taskZhangJieVO.ID);
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(taskZhangJieVO.GlGoodsID);
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(taskZhangJieVO.GlGoodsID);
			this.Content = UIHelper.GetBaseAttributeStr(dummyGoodsData, goodsEquipPropsDoubleList, -1);
		}
	}

	public ShowNetImage ZhangjieTitle;

	public GButton Btn_OK;

	public TextBlock Tex_Content;

	public DPSelectedItemEventHandler DPSelectedItem;

	private string _content = string.Empty;
}
