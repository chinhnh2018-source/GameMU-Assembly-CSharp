using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class GBuffTips : UserControl
{
	public void RenderTips(int bufferID)
	{
		this.SetText(bufferID);
		this.SetPropsPanel();
	}

	private void SetText(int bufferID)
	{
		if (bufferID <= 0)
		{
			return;
		}
		BufferData bufferData = Global.GetBufferDataByID(bufferID);
		if (bufferData == null)
		{
			if (2000858 > bufferID || bufferID > 2000862)
			{
				return;
			}
			bufferData = new BufferData();
			bufferData.BufferID = bufferID;
			bufferData.BufferVal = (long)bufferID;
		}
		if (bufferData.BufferID == 81)
		{
			this.txtTitle.Text = Global.GetLang("生命之光");
			this.txtResult.Text = Global.GetLang("提升人物一定比例的生命上限,持续一段时间");
		}
		else if (bufferData.BufferID == 83)
		{
			this.txtTitle.Text = Global.GetLang("守护之魂");
			this.txtResult.Text = Global.GetLang("吸收一定比例的伤害,持续一段时间");
		}
		else if (bufferData.BufferID == 84)
		{
			this.txtTitle.Text = Global.GetLang("战神之力");
			this.txtResult.Text = Global.GetLang("提升人物的攻击防御值,持续一段时间");
		}
		else if (bufferData.BufferID == 102)
		{
			this.txtTitle.Text = Global.GetLang("迅捷之风");
			this.txtResult.Text = Global.GetLang("提升人物的命中和闪避,持续一段时间");
		}
		else if (bufferData.BufferID == 104)
		{
			this.txtTitle.Text = Global.GetLang("伤害反射");
			this.txtResult.Text = Global.GetLang(" 提升人物的伤害反弹,持续一段时间");
		}
		else if (bufferData.BufferID == 31)
		{
			int num = Global.GetBufferGoodsID(bufferData.BufferID, (int)bufferData.BufferVal);
			string text = Global.GetGoodsEquipPropsStringForBufferTips(num);
			this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
			this.txtResult.Text = string.Format(Global.GetLang("您当前的人物成就为{0},可为您带来如下属性加成：\n\n{1}"), this.txtTitle.Text, text);
		}
		else if (bufferData.BufferID == 85 || bufferData.BufferID == 86)
		{
			int num = Global.GetBufferGoodsID(bufferData.BufferID, (int)bufferData.BufferVal);
			string text = Global.GetGoodsEquipPropsStringForBufferTips(num);
			this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
			this.txtResult.Text = string.Format(Global.GetLang("战力鼓舞,可为您带来如下属性加成：\n\n{1}"), this.txtTitle.Text, text);
		}
		else if (bufferData.BufferID != 86)
		{
			if (bufferData.BufferID == 49)
			{
				int num = Global.GetBufferGoodsID(bufferData.BufferID, Global.Data.roleData.VIPLevel);
				string text = Global.GetGoodsEquipPropsStringForBufferTips(num);
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				this.txtResult.Text = string.Format(Global.GetLang("您当前的VIP级别为{0},可为您带来如下属性加成：\n\n{1}"), this.txtTitle.Text, text);
			}
			else if (bufferData.BufferID == 88)
			{
				int num = Global.GetBufferGoodsID(bufferData.BufferID, Convert.ToInt32(bufferData.BufferVal));
				string text = Global.GetGoodsEquipPropsStringForBufferTips(num);
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				this.txtResult.Text = string.Format(Global.GetLang("可为您带来如下属性加成：\n\n{1}"), this.txtTitle.Text, text);
			}
			else if (bufferData.BufferID == 89)
			{
				int num = Global.GetBufferGoodsID(bufferData.BufferID, Convert.ToInt32(bufferData.BufferVal));
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
				string text = goodsXmlNodeByID.Description;
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				this.txtResult.Text = string.Format(Global.GetLang("可为您带来如下属性加成：\n\n{1}"), this.txtTitle.Text, text);
			}
			else if (bufferData.BufferID == 90)
			{
				int num = Global.GetBufferGoodsID(bufferData.BufferID, Convert.ToInt32(bufferData.BufferVal));
				string text = Global.GetGoodsEquipPropsStringForBufferTips(num);
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				this.txtResult.Text = string.Format(Global.GetLang("可为您带来如下属性加成：\n\n{1}"), this.txtTitle.Text, text);
			}
			else if (bufferData.BufferID == 91)
			{
				int num = Global.GetBufferGoodsID(bufferData.BufferID, Convert.ToInt32(bufferData.BufferVal));
				string text = Global.GetGoodsEquipPropsStringForBufferTips(num);
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				this.txtResult.Text = string.Format(Global.GetLang("可为您带来如下属性加成：\n\n{1}"), this.txtTitle.Text, text);
			}
			else if (bufferData.BufferID == 39)
			{
				int num = Global.GetBufferGoodsID(bufferData.BufferID, 0);
				string text = Global.GetGoodsEquipPropsStringForBufferTips(num);
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				this.txtResult.Text = string.Format(Global.GetLang("PK之王,可为您带来如下属性加成：\n\n{1}"), this.txtTitle.Text, text);
			}
			else if (bufferData.BufferID == 87)
			{
				int num = Global.GetBufferGoodsID(bufferData.BufferID, (int)bufferData.BufferVal);
				string text = Global.GetGoodsEquipPropsStringForBufferTips(num);
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				this.txtResult.Text = string.Format(Global.GetLang("您当前的军衔等级为{0},可为您带来如下属性加成：\n\n{1}"), this.txtTitle.Text, text);
			}
			else if (bufferData.BufferID == 92)
			{
				double[] systemParamDoubleArrayByName = ConfigSystemParam.GetSystemParamDoubleArrayByName("HongMingDebuff");
				if (systemParamDoubleArrayByName != null && systemParamDoubleArrayByName.Length == 4)
				{
					this.txtTitle.Text = Global.GetLang("红名处罚BUFF");
					double num2 = (double)(bufferData.BufferVal >> 32 & (long)((ulong)-1));
					this.txtResult.Text = string.Format(Global.GetLang("攻击力下降：{0}%\n防御力下降：{1}%\n生命上限下降：{2}%"), systemParamDoubleArrayByName[0] * num2 * 100.0, systemParamDoubleArrayByName[1] * num2 * 100.0, systemParamDoubleArrayByName[2] * num2 * 100.0);
				}
			}
			else if (bufferData.BufferID == 99)
			{
				string systemParamByName = ConfigSystemParam.GetSystemParamByName("WorldLevelGoodsIDs", true);
				GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(systemParamByName.SafeToInt32(0));
				this.txtTitle.Text = goodsXmlNodeByID2.Title;
				if (goodsXmlNodeByID2 != null)
				{
					this.txtResult.Text = string.Format(Global.GetLang("{0}"), goodsXmlNodeByID2.Description);
					TextBlock textBlock = this.txtResult;
					textBlock.Text = textBlock.Text + bufferData.BufferVal.ToString() + "%";
				}
			}
			else if (bufferData.BufferID == 2080010)
			{
				int num = bufferData.BufferID;
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				GoodVO goodsXmlNodeByID3 = ConfigGoods.GetGoodsXmlNodeByID(num);
				string text;
				if (goodsXmlNodeByID3 == null)
				{
					text = Global.GetLang("无");
				}
				else
				{
					text = goodsXmlNodeByID3.Description;
				}
				this.txtResult.Text = string.Format(Global.GetLang("可为您带来如下属性加成：\n\n{0}"), text);
			}
			else if (bufferData.BufferID == 2080002)
			{
				int num = (int)bufferData.BufferVal;
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				GoodVO goodsXmlNodeByID4 = ConfigGoods.GetGoodsXmlNodeByID(num);
				string text;
				if (goodsXmlNodeByID4 == null)
				{
					text = Global.GetLang("无");
				}
				else
				{
					text = goodsXmlNodeByID4.Description;
				}
				this.txtResult.Text = string.Format(Global.GetLang("可为您带来如下属性加成：\n\n{0}"), text);
			}
			else if (bufferData.BufferID == 2080011)
			{
				int num = bufferData.BufferID;
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				GoodVO goodsXmlNodeByID5 = ConfigGoods.GetGoodsXmlNodeByID(num);
				string text;
				if (goodsXmlNodeByID5 == null)
				{
					text = Global.GetLang("无");
				}
				else
				{
					text = goodsXmlNodeByID5.Description;
				}
				this.txtResult.Text = string.Format(Global.GetLang("可为您带来如下属性加成：\n\n{0}"), text);
			}
			else if (bufferData.BufferID == 115)
			{
				int num = ConfigSystemParam.GetSystemParamIntArrayByName("ManorBuffID", ',')[0];
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				GoodVO goodsXmlNodeByID6 = ConfigGoods.GetGoodsXmlNodeByID(num);
				string text;
				if (goodsXmlNodeByID6 == null)
				{
					text = Global.GetLang("无");
				}
				else
				{
					text = goodsXmlNodeByID6.Description;
				}
				this.txtResult.Text = string.Format(Global.GetLang("可为您带来如下属性加成：\n\n{0}"), text);
			}
			else if (bufferData.BufferID == 100)
			{
				int id = ConfigSystemParam.GetSystemParamIntArrayByName("PingBanBuff", ',')[0];
				GoodVO goodsXmlNodeByID7 = ConfigGoods.GetGoodsXmlNodeByID(id);
				this.txtTitle.Text = goodsXmlNodeByID7.Title;
				if (goodsXmlNodeByID7 != null)
				{
					this.txtResult.Text = string.Format(Global.GetLang("{0}"), goodsXmlNodeByID7.Description);
				}
			}
			else if (bufferData.BufferID == 8999)
			{
				int num = (int)bufferData.BufferVal;
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				GoodVO goodsXmlNodeByID8 = ConfigGoods.GetGoodsXmlNodeByID(num);
				string text;
				if (goodsXmlNodeByID8 == null)
				{
					text = Global.GetLang("无");
				}
				else
				{
					text = goodsXmlNodeByID8.Description;
				}
				this.txtResult.Text = string.Format("{0}", text);
			}
			else if (bufferData.BufferID == 9000 || bufferData.BufferID == 9001 || bufferData.BufferID == 9002)
			{
				int num = (int)bufferData.BufferVal;
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				GoodVO goodsXmlNodeByID9 = ConfigGoods.GetGoodsXmlNodeByID(num);
				string text;
				if (goodsXmlNodeByID9 == null)
				{
					text = Global.GetLang("无");
				}
				else
				{
					text = goodsXmlNodeByID9.Description;
				}
				this.txtResult.Text = string.Format("{0}", text);
			}
			else if (bufferData.BufferID >= 9012 && bufferData.BufferID <= 9017)
			{
				int num = (int)bufferData.BufferVal;
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				GoodVO goodsXmlNodeByID10 = ConfigGoods.GetGoodsXmlNodeByID(num);
				string text;
				if (goodsXmlNodeByID10 == null)
				{
					text = Global.GetLang("无");
				}
				else
				{
					text = goodsXmlNodeByID10.Description;
				}
				this.txtResult.Text = string.Format("{0}", text);
			}
			else if (bufferData.BufferID >= 2000853 && bufferData.BufferID <= 2000862)
			{
				int num = (int)bufferData.BufferVal;
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				GoodVO goodsXmlNodeByID11 = ConfigGoods.GetGoodsXmlNodeByID(num);
				string text;
				if (goodsXmlNodeByID11 == null)
				{
					text = Global.GetLang("无");
				}
				else
				{
					text = goodsXmlNodeByID11.Description;
				}
				this.txtResult.Text = string.Format("{0}", text);
			}
			else if (bufferData.BufferID == 121)
			{
				int num = (int)bufferData.BufferVal;
				this.txtTitle.Text = Global.GetGoodsNameByID(num, false);
				GoodVO goodsXmlNodeByID12 = ConfigGoods.GetGoodsXmlNodeByID(num);
				string text;
				if (goodsXmlNodeByID12 == null)
				{
					text = Global.GetLang("无");
				}
				else
				{
					text = goodsXmlNodeByID12.Description;
				}
				this.txtResult.Text = string.Format("{0}", text);
			}
			else if (bufferData.BufferID == 122)
			{
				int num = (int)(bufferData.BufferVal >> 32);
				GoodVO goodsXmlNodeByID13 = ConfigGoods.GetGoodsXmlNodeByID(num);
				this.txtTitle.Text = goodsXmlNodeByID13.Title;
				this.txtResult.Text = goodsXmlNodeByID13.Description;
			}
			else if (bufferData.BufferID == 2090001)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("EscapeBuffID", ',');
				int num = systemParamIntArrayByName[0];
				GoodVO goodsXmlNodeByID14 = ConfigGoods.GetGoodsXmlNodeByID(num);
				this.txtTitle.Text = goodsXmlNodeByID14.Title;
				this.txtResult.Text = DaTaoShaDataManager.TianShenBuffDes;
			}
			else if (bufferData.BufferID == 2090002)
			{
				int[] systemParamIntArrayByName2 = ConfigSystemParam.GetSystemParamIntArrayByName("EscapeBuffID", ',');
				int num = systemParamIntArrayByName2[1];
				GoodVO goodsXmlNodeByID15 = ConfigGoods.GetGoodsXmlNodeByID(num);
				this.txtTitle.Text = goodsXmlNodeByID15.Title;
				this.txtResult.Text = DaTaoShaDataManager.EMoBuffDes;
			}
			else if (20000 <= bufferID && bufferID <= 29999)
			{
				int num = Global.GetSpecialTitleBuffGoodsID(bufferID);
				GoodVO goodsXmlNodeByID16 = ConfigGoods.GetGoodsXmlNodeByID(num);
				if (goodsXmlNodeByID16 != null)
				{
					this.txtTitle.Text = goodsXmlNodeByID16.Title;
					this.txtResult.Text = goodsXmlNodeByID16.Description;
				}
			}
			else
			{
				int num;
				if (bufferData.BufferID == 46 || bufferData.BufferID == 123)
				{
					int num3 = (int)(bufferData.BufferVal & (long)((ulong)-1));
					num = (int)((double)(bufferData.BufferVal - (long)num3) / Math.Pow(2.0, 32.0));
				}
				else
				{
					num = (int)bufferData.BufferVal;
				}
				GoodVO goodsXmlNodeByID17 = ConfigGoods.GetGoodsXmlNodeByID(num);
				this.txtTitle.Text = goodsXmlNodeByID17.Title;
				this.txtResult.Text = goodsXmlNodeByID17.Description;
			}
		}
		if (bufferData.BufferID == 31)
		{
			this.txtShengyuTime.Text = Global.GetLang("剩余时间：永久");
		}
		else if (bufferData.BufferID == 115)
		{
			this.txtShengyuTime.Text = Global.GetLang(string.Empty);
		}
		else
		{
			double num4 = (double)bufferData.BufferSecs * 1000.0;
			double num5 = (double)Math.Max(Global.GetCorrectLocalTime() - bufferData.StartTime, 0L);
			double num6 = Math.Max((num4 - num5) / 1000.0, 0.0);
			if (num6 == 0.0)
			{
				this.txtShengyuTime.Text = string.Empty;
			}
			else if (num6 < 3600.0 && num6 > 0.0)
			{
				this.txtShengyuTime.Text = string.Format(Global.GetLang("剩余时间：{0}分钟{1}秒"), (int)(num6 / 60.0), (int)(num6 % 60.0));
			}
			else
			{
				double num7 = num6 % 3600.0;
				this.txtShengyuTime.Text = string.Format(Global.GetLang("剩余时间：{0}小时{1}分钟{2}秒"), (int)(num6 / 3600.0), (int)(num7 / 60.0), (int)(num7 % 60.0));
			}
		}
	}

	private void SetPropsPanel()
	{
		int num = 20;
		Vector3 localPosition = this.txtResult.transform.localPosition;
		this.txtShengyuTime.transform.localPosition = new Vector3(localPosition.x, localPosition.y - (float)this.txtResult.ActualHeight - (float)num, 0f);
		this.Bak.transform.localScale = new Vector3(this.Bak.transform.localScale.x, (float)(this.txtResult.ActualHeight + 80.0), 1f);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UISprite Bak;

	public TextBlock txtTitle;

	public TextBlock txtResult;

	public TextBlock txtShengyuTime;
}
