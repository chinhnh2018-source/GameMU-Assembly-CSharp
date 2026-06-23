using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

namespace HSGameEngine.GameEngine.Logic
{
	public class GChat
	{
		public static bool IsZhengZaiLuYin
		{
			get
			{
				return GChat.ILuYinSecondCout > -1;
			}
		}

		public static int ChatReceiveID
		{
			get
			{
				return GChat.mChatReceiveID++;
			}
			set
			{
				GChat.mChatReceiveID = 0;
			}
		}

		public static void CleanMessage()
		{
			if (GChat.AllChatTextList.Count < 20)
			{
				return;
			}
			int num = 0;
			while (num++ < 30)
			{
				if (GChat.AllChatTextList.Count <= 0)
				{
					break;
				}
				ChatTextItem chatTextItem = GChat.AllChatTextList[0];
				GChat.AllChatTextList.RemoveAt(0);
				try
				{
					GChat.WorldChatTextList.Remove(chatTextItem);
				}
				catch (Exception ex)
				{
					MUDebug.LogException(ex);
				}
				try
				{
					GChat.FactionChatTextList.Remove(chatTextItem);
				}
				catch (Exception ex2)
				{
					MUDebug.LogException(ex2);
				}
				try
				{
					GChat.TeamChatTextList.Remove(chatTextItem);
				}
				catch (Exception ex3)
				{
					MUDebug.LogException(ex3);
				}
				try
				{
					GChat.PrivateChatTextList.Remove(chatTextItem);
				}
				catch (Exception ex4)
				{
					MUDebug.LogException(ex4);
				}
				try
				{
					GChat.SystemChatTextList.Remove(chatTextItem);
				}
				catch (Exception ex5)
				{
					MUDebug.LogException(ex5);
				}
				try
				{
					GChat.ZhenYingChatTextList.Remove(chatTextItem);
				}
				catch (Exception ex6)
				{
					MUDebug.LogException(ex6);
				}
				try
				{
					GChat.FuBenChatTextList.Remove(chatTextItem);
				}
				catch (Exception ex7)
				{
					MUDebug.LogException(ex7);
				}
				try
				{
					GChat.ArmyGroupChatTextList.Remove(chatTextItem);
				}
				catch (Exception ex8)
				{
					MUDebug.LogException(ex8);
				}
				try
				{
					GChat.PlatformChatTextList.Remove(chatTextItem);
				}
				catch (Exception ex9)
				{
					MUDebug.LogException(ex9);
				}
				try
				{
					GChat.ZhanDuiChatTextList.Remove(chatTextItem);
				}
				catch (Exception ex10)
				{
					MUDebug.LogException(ex10);
				}
			}
			while (GChat.SystemChatTextListBak.Count > 100)
			{
				GChat.SystemChatTextListBak.RemoveAt(0);
			}
		}

		public static void CheckAndHandleMessage()
		{
			GChat.ResetChatTextList(GChat.AllChatTextList, 20);
			GChat.ResetChatTextList(GChat.WorldChatTextList, 20);
			GChat.ResetChatTextList(GChat.FactionChatTextList, 20);
			GChat.ResetChatTextList(GChat.TeamChatTextList, 20);
			GChat.ResetChatTextList(GChat.PrivateChatTextList, 20);
			GChat.ResetChatTextList(GChat.SystemChatTextList, 20);
			GChat.ResetChatTextList(GChat.ZhenYingChatTextList, 20);
			GChat.ResetChatTextList(GChat.FuBenChatTextList, 20);
			GChat.ResetChatTextList(GChat.ArmyGroupChatTextList, 20);
			GChat.ResetChatTextList(GChat.PlatformChatTextList, 20);
			GChat.ResetChatTextList(GChat.ZhanDuiChatTextList, 20);
		}

		private static void ResetChatTextList(List<ChatTextItem> textList, int max)
		{
			if (textList.Count > max)
			{
				textList.RemoveRange(0, textList.Count - max);
			}
		}

		public static void AddSystemChatMessage(string textMsg, ChatTypeIndexes chatIndex = ChatTypeIndexes.System)
		{
			if (chatIndex < ChatTypeIndexes.System || chatIndex >= ChatTypeIndexes.Max)
			{
				chatIndex = ChatTypeIndexes.System;
			}
			long correctLocalTime = TimeManager.GetCorrectLocalTime();
			ChatTextItem chatTextItem = new ChatTextItem
			{
				FromRoleID = -1,
				FromRoleName = string.Empty,
				Status = 0,
				ToRoleName = string.Empty,
				TextMsg = textMsg,
				Ticks = correctLocalTime,
				ChatIndex = chatIndex,
				occupation = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation),
				PTID = Global.Data.roleData.PTID
			};
			GChat.AllChatTextList.Add(chatTextItem);
			GChat.SystemChatTextList.Add(chatTextItem);
			GChat.SystemChatTextListBak.Add(chatTextItem);
		}

		public static void AddBulletinChatMessage(string textMsg)
		{
			long correctLocalTime = TimeManager.GetCorrectLocalTime();
			ChatTextItem chatTextItem = new ChatTextItem
			{
				FromRoleID = -1,
				FromRoleName = string.Empty,
				Status = 0,
				ToRoleName = string.Empty,
				TextMsg = textMsg,
				Ticks = correctLocalTime,
				ChatIndex = ChatTypeIndexes.Bulletin,
				PTID = 0
			};
			GChat.AllChatTextList.Add(chatTextItem);
			GChat.SystemChatTextList.Add(chatTextItem);
			GChat.SystemChatTextListBak.Add(chatTextItem);
		}

		public static ChatTypeIndexes ParseChatMessage(int fromRoleID, string fromRoleName, int status, string toRoleName, int chatIndex, string textMsg, int chatType, int Occ, int PTID, string speakZoneID = null, string listenZoneID = null)
		{
			if (chatIndex < 0 || chatIndex >= 13)
			{
				return ChatTypeIndexes.Max;
			}
			if (chatIndex == 9 && Global.IsInZhanMengLianSaiCompetetionMap())
			{
				return ChatTypeIndexes.Max;
			}
			GChat.CheckAndHandleMessage();
			if (chatType == 1)
			{
			}
			long correctLocalTime = TimeManager.GetCorrectLocalTime();
			ChatTextItem chatTextItem = new ChatTextItem
			{
				FromRoleID = fromRoleID,
				FromRoleName = fromRoleName,
				Status = status,
				ToRoleName = toRoleName,
				TextMsg = textMsg,
				Ticks = correctLocalTime,
				ChatIndex = (ChatTypeIndexes)chatIndex,
				ChatType = (ChatType)chatType,
				SpeakZoneID = ((speakZoneID == null) ? 0 : Global.SafeConvertToInt32(speakZoneID)),
				ListenZoneID = ((listenZoneID == null) ? 0 : Global.SafeConvertToInt32(listenZoneID)),
				occupation = Global.CalcOriginalOccupationID(Occ),
				PTID = PTID
			};
			List<FriendData> friendDataList = Global.Data.FriendDataList;
			if (friendDataList != null)
			{
				for (int i = 0; i < friendDataList.Count; i++)
				{
					FriendData friendData = friendDataList[i];
					if (friendData.FriendType == 1 && friendData.OtherRoleID == fromRoleID)
					{
						return (ChatTypeIndexes)chatIndex;
					}
				}
			}
			if (chatIndex == 5)
			{
				if (!Global.Data.SysSetting.RefusePrivateChat)
				{
					GChat.AllChatTextList.Add(chatTextItem);
				}
			}
			else
			{
				GChat.AllChatTextList.Add(chatTextItem);
			}
			if (chatIndex == 2)
			{
				GChat.WorldChatTextList.Add(chatTextItem);
			}
			else if (chatIndex == 3)
			{
				GChat.FactionChatTextList.Add(chatTextItem);
			}
			else if (chatIndex == 4)
			{
				GChat.TeamChatTextList.Add(chatTextItem);
			}
			else if (chatIndex == 5)
			{
				if (!Global.Data.SysSetting.RefusePrivateChat)
				{
					GChat.PrivateChatTextList.Add(chatTextItem);
				}
			}
			else if (chatIndex == 0 || chatIndex == 6)
			{
				GChat.SystemChatTextList.Add(chatTextItem);
			}
			else if (chatIndex == 7)
			{
				GChat.ZhenYingChatTextList.Add(chatTextItem);
			}
			else if (chatIndex == 8)
			{
				GChat.FuBenChatTextList.Add(chatTextItem);
			}
			else if (chatIndex == 9)
			{
				if (Global.RoleHaveArmyGroup())
				{
					GChat.ArmyGroupChatTextList.Add(chatTextItem);
				}
			}
			else if (chatIndex == 10)
			{
				GChat.CompGroupChatTextList.Add(chatTextItem);
			}
			else if (!Global.isHaiWai && chatIndex == 11)
			{
				GChat.PlatformChatTextList.Add(chatTextItem);
			}
			else if (chatIndex == 12)
			{
				GChat.ZhanDuiChatTextList.Add(chatTextItem);
			}
			GChat.ShowRoleTalkText(chatTextItem);
			return (ChatTypeIndexes)chatIndex;
		}

		private static void ShowRoleTalkText(ChatTextItem chatTextItem)
		{
			if (Global.Data.SysSetting.HideChatPopupWin)
			{
				return;
			}
			if (chatTextItem.ChatIndex != ChatTypeIndexes.Map)
			{
				return;
			}
			Global.Data.GameScene.ShowRoleTalkText(chatTextItem.FromRoleID, chatTextItem.TextMsg);
		}

		private static string GetFormatedTextContent(string value, int offset, int roleID, bool isChat = false)
		{
			string text = value;
			string empty = string.Empty;
			if (offset > 0)
			{
			}
			int num = text.IndexOf("<$goods$>");
			if (num != -1)
			{
				text = text.Substring(0, num);
			}
			if (!isChat)
			{
				return empty;
			}
			if (num != -1)
			{
				string goods = value.Substring(num + "<$goods$>".Length);
				return GChat.GetFormatedGoodsText(empty, goods, text, offset, roleID);
			}
			return value;
		}

		public static string GetFormatedTextContentEx(string value, bool showSymbol)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = "｛-｝";
			string text2 = string.Empty;
			if (value.IndexOf(text) != -1)
			{
				int num = 0;
				List<string> list = new List<string>();
				while (value.IndexOf(text, num) != -1)
				{
					int num2 = value.IndexOf(text, num);
					list.Add(value.Substring(num, num2 - num));
					num = num2 + 3;
				}
				if (num < value.Length - 1)
				{
					list.Add(value.Substring(num, value.Length - num));
				}
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					string text3 = list[i];
					if (text3 != null || !(text3 == string.Empty))
					{
						int length = text3.Length;
						num = text3.IndexOf('｛');
						if (num != -1 && text3.IndexOf('｝') != -1)
						{
							if (num != 0)
							{
								if (showSymbol)
								{
									text2 = ChatSymbolConfig.Singleton.FormatToHtml(text3.Substring(0, num));
								}
								else
								{
									text2 = text3.Substring(0, num);
								}
								stringBuilder.Append(text2);
							}
							stringBuilder.Append(GChat.GetFormatGoodsStr(text3));
						}
						else
						{
							if (showSymbol)
							{
								text2 = ChatSymbolConfig.Singleton.FormatToHtml(text3);
							}
							else
							{
								text2 = text3;
							}
							stringBuilder.Append(text2);
						}
					}
				}
			}
			else
			{
				if (showSymbol)
				{
					text2 = ChatSymbolConfig.Singleton.FormatToHtml(value);
				}
				else
				{
					text2 = value;
				}
				stringBuilder.Append(text2);
			}
			return stringBuilder.ToString();
		}

		public static bool CheckGoodFormatedContent(string value)
		{
			string text = "｛-｝";
			string strItem = string.Empty;
			bool result = true;
			if (value.IndexOf(text) != -1)
			{
				int num = 0;
				List<string> list = new List<string>();
				while (value.IndexOf(text, num) != -1)
				{
					int num2 = value.IndexOf(text, num);
					list.Add(value.Substring(num, num2 - num));
					num = num2 + 3;
				}
				if (num < value.Length - 1)
				{
					list.Add(value.Substring(num, value.Length - num));
				}
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					string text2 = list[i];
					if (text2 != null || !(text2 == string.Empty))
					{
						int length = text2.Length;
						num = text2.IndexOf('｛');
						if (num != -1 && text2.IndexOf('｝') != -1)
						{
							strItem = text2.Substring(num);
							bool flag = true;
							GChat.CheckGoodItemString(strItem, out flag);
							if (!flag)
							{
								result = false;
								break;
							}
						}
					}
				}
			}
			return result;
		}

		private static void CheckGoodItemString(string strItem, out bool bRight)
		{
			bRight = true;
			int num = strItem.IndexOf('｛');
			int num2 = strItem.IndexOf('｝');
			if (num < 0 || num2 < 0 || num >= num2)
			{
				return;
			}
			string text = strItem.Substring(num + 1, num2 - (num + 1));
			string[] array = text.Split(new char[]
			{
				'_'
			});
			if (array == null || array.Length < 3)
			{
				return;
			}
			string text2 = array[0];
			string text3 = array[1];
			string text4 = array[2];
			string text5 = string.Concat(new string[]
			{
				text2,
				"_",
				text3,
				"_",
				text4
			});
			if (strItem.Length < num2 + 1 + (strItem.Length - (num2 + 1)))
			{
				return;
			}
			string text6 = strItem.Substring(num2 + 1, strItem.Length - (num2 + 1));
			if (text6.Length < 7 || text6.Length < 8 + Math.Max(0, text6.Length - 11))
			{
				return;
			}
			GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(ConvertExt.SafeConvertToInt32(text3), null);
			if (goodsDataByDbID == null)
			{
				return;
			}
			string text7 = ChatBoxBagContainer.GetTextColoeByGoodsIDHTML(goodsDataByDbID);
			if (string.IsNullOrEmpty(text7) || text7.Length < 3)
			{
				return;
			}
			text7 = text7.Substring(0, text7.Length - 3);
			if (!StringUtil.isEqualIgnoreCase(strItem, text7))
			{
				bRight = false;
			}
			if (strItem.Contains("***"))
			{
				bRight = true;
			}
		}

		private static string GetFormatGoodsStr(string strItem)
		{
			int num = strItem.IndexOf('｛');
			int num2 = strItem.IndexOf('｝');
			if (num < 0 || num2 < 0 || num >= num2)
			{
				return strItem;
			}
			string text = strItem.Substring(num + 1, num2 - (num + 1));
			string[] array = text.Split(new char[]
			{
				'_'
			});
			if (array == null || array.Length < 3)
			{
				return strItem;
			}
			string text2 = array[0];
			string text3 = array[1];
			string text4 = array[2];
			string text5 = string.Concat(new string[]
			{
				text2,
				"_",
				text3,
				"_",
				text4
			});
			if (strItem.Length < num2 + 1 + (strItem.Length - (num2 + 1)))
			{
				return strItem;
			}
			string text6 = strItem.Substring(num2 + 1, strItem.Length - (num2 + 1));
			if (text6.Length < 7 || text6.Length < 8 + Math.Max(0, text6.Length - 11))
			{
				return strItem;
			}
			string color = text6.Substring(1, 6);
			string text7 = text6.Substring(8, Math.Max(0, text6.Length - 11));
			string formatedColorStr = GChat.GetFormatedColorStr(text7 + "\u00a0", color);
			return string.Format("<a href='{0}' id='{1}'>{2}</a>", text5, text5, formatedColorStr);
		}

		public static string GetFormatedColorStr(string value, string color)
		{
			if (color.StartsWith("0x") || color.StartsWith("0X"))
			{
				color = color.Substring(2);
			}
			return string.Format("<font color=#{0}>{1}</font>", color, value);
		}

		public static string GetFormatedRolename(string value, int roleID, bool FormMini = false)
		{
			GTextData gtextData = new GTextData();
			gtextData.startIndex = 0;
			gtextData.endIndex = 0;
			gtextData.RoleID = roleID;
			gtextData.type = 0;
			gtextData.key = string.Format("{0}_{1}{2}", gtextData.type, gtextData.startIndex, gtextData.endIndex);
			string text = "RoleID_" + roleID;
			string text2 = string.Format("<a href='{0}' id='{1}'>{2}</a>", text, text, GChat.GetFormatedColorStr(value, (!FormMini) ? "f7d66b" : "ffffff"));
			return text2 + "\u00a0";
		}

		private static string GetFormatedGoodsText(string textStr, string goods, string msg, int offset, int roleID)
		{
			string[] array = goods.Split(StringMark.CHAT_SUB_MARK1);
			if (array.Length > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				foreach (string text in array)
				{
					string[] array3 = text.Split(StringMark.CHAT_SUB_MARK2);
					GTextData gtextData = new GTextData();
					gtextData.startIndex = Convert.ToInt32(array3[0]) + offset;
					gtextData.endIndex = Convert.ToInt32(array3[1]) + offset;
					gtextData.goodsId = Convert.ToInt32(array3[2]);
					gtextData.RoleID = roleID;
					gtextData.type = 2;
					gtextData.Quality = Convert.ToInt32(array3[3]);
					gtextData.key = string.Format("{0}_{1}{2}", gtextData.type, gtextData.startIndex, gtextData.endIndex);
					if (gtextData.endIndex <= textStr.Length && gtextData.startIndex >= 0)
					{
						string text2 = textStr.Substring(gtextData.startIndex, gtextData.endIndex - gtextData.startIndex);
						string[] array4 = text2.Split(new char[]
						{
							'+'
						});
						int num2 = ConfigGoods.FindGoodsIDByName(array4[0]);
						if (num2 >= 0)
						{
							gtextData.color = Global.ParseStringColor(Global.GetGoodsColorString(num2)).Color;
							GChat.FormatGoodsStrAndPreStr(stringBuilder, msg, gtextData, ref num);
						}
					}
				}
				return stringBuilder.ToString();
			}
			return msg;
		}

		private static void FormatGoodsStrAndPreStr(StringBuilder strFinal, string strMsg, GTextData data, ref int index)
		{
			string text = strMsg.Substring(index, data.startIndex - index - 1);
			strFinal.Append(text);
			text = strMsg.Substring(data.startIndex, data.endIndex);
			strFinal.Append(string.Format("{0}{1}{2}{3}{4}", new object[]
			{
				"｛" + data.key + "｝",
				"{" + data.color.ToString() + "}",
				text,
				"{-}",
				"｛-｝"
			}));
			index = data.endIndex + 1;
		}

		public static string FormatChatText(ChatTextItem chatTextItem, bool showSymbol = true, bool FormMini = false)
		{
			string text = "<p align=left valign=top>";
			return text + GChat.GetChatUserNameText(chatTextItem, FormMini) + GChat.GetChatContentText(chatTextItem, showSymbol, FormMini) + "</p>";
		}

		public static string FormatChatUserText(ChatTextItem chatTextItem, bool beSelfLeft = false, bool FormMini = false)
		{
			string text = string.Empty;
			if (chatTextItem.FromRoleID == Global.Data.roleData.RoleID && !beSelfLeft)
			{
				text = "<p align=right valign=top>";
			}
			else
			{
				text = "<p align=left valign=top>";
			}
			return text + GChat.GetChatUserNameText(chatTextItem, FormMini) + "</p>";
		}

		public static string FormatChatContentText(ChatTextItem chatTextItem, int maxLineWidth, bool showSymbol = true)
		{
			string chatContentText = GChat.GetChatContentText(chatTextItem, showSymbol, false);
			string text = "<p align=left valign=top>" + chatContentText + "</p>";
			if (chatTextItem.FromRoleID == Global.Data.roleData.RoleID)
			{
				text = "<p align=right valign=top>" + chatContentText + "</p>";
				if (NGUIHTML.GetLineCount(text, maxLineWidth) > 1)
				{
					text = "<p align=left valign=top>" + chatContentText + "</p>";
				}
			}
			return text;
		}

		public static string GetChatContentText(ChatTextItem chatTextItem, bool showSymbol = true, bool FormMini = false)
		{
			string text = chatTextItem.TextMsg;
			chatTextItem.FromRoleName = Global.ChangeChatboxRoleName(chatTextItem.FromRoleName, chatTextItem.SpeakZoneID);
			chatTextItem.ToRoleName = Global.ChangeChatboxRoleName(chatTextItem.ToRoleName, chatTextItem.ListenZoneID);
			string text2 = text.Substring(0, Math.Min(5, text.Length));
			if (text2 == "[VIP]")
			{
				text2 = GChat.GetFormatedColorStr(text2, U3DUtils.ConvertToNguiColor(4294944000U));
				text = text.Substring(5, text.Length - 5);
			}
			else
			{
				text2 = string.Empty;
			}
			string color = "0xffffff";
			if (!FormMini)
			{
				color = "0xdac7ae";
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.System)
			{
				color = GChat.TabListColor[5];
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.Bulletin)
			{
				color = GChat.TabListColor[0];
			}
			else if (chatTextItem.ChatIndex != ChatTypeIndexes.Map)
			{
				if (chatTextItem.ChatIndex == ChatTypeIndexes.World)
				{
					color = GChat.TabListColor[1];
				}
				else if (chatTextItem.ChatIndex == ChatTypeIndexes.Faction)
				{
					color = GChat.TabListColor[2];
				}
				else if (chatTextItem.ChatIndex == ChatTypeIndexes.Team)
				{
					color = GChat.TabListColor[3];
				}
				else if (chatTextItem.ChatIndex == ChatTypeIndexes.Private)
				{
					if (chatTextItem.Status == 0)
					{
						color = GChat.TabListColor[4];
					}
				}
				else if (chatTextItem.ChatIndex == ChatTypeIndexes.ZhenYing)
				{
					color = GChat.TabListColor[5];
				}
				else if (chatTextItem.ChatIndex == ChatTypeIndexes.FuBen)
				{
					color = GChat.TabListColor[5];
				}
				else if (chatTextItem.ChatIndex == ChatTypeIndexes.ArmyGroup)
				{
					color = GChat.TabListColor[5];
				}
				else if (chatTextItem.ChatIndex == ChatTypeIndexes.Comp)
				{
					color = GChat.TabListColor[5];
				}
				else if (!Global.isHaiWai && chatTextItem.ChatIndex == ChatTypeIndexes.Platform)
				{
					color = GChat.TabListColor[1];
				}
			}
			text = GChat.GetFormatedTextContentEx(text, showSymbol);
			return GChat.GetFormatedColorStr(text, color);
		}

		public static string GetChatUserNameText(ChatTextItem chatTextItem, bool FormMini = false)
		{
			string text = chatTextItem.TextMsg;
			string value = string.Empty;
			chatTextItem.FromRoleName = Global.ChangeChatboxRoleName(chatTextItem.FromRoleName, chatTextItem.SpeakZoneID);
			chatTextItem.ToRoleName = Global.ChangeChatboxRoleName(chatTextItem.ToRoleName, chatTextItem.ListenZoneID);
			string text2 = text.Substring(0, Math.Min(5, text.Length));
			if (text2 == "[VIP]")
			{
				text2 = GChat.GetFormatedColorStr(text2, U3DUtils.ConvertToNguiColor(4294944000U));
				text = text.Substring(5, text.Length - 5);
			}
			else
			{
				text2 = string.Empty;
			}
			string color = "0xffffff";
			if (chatTextItem.ChatIndex == ChatTypeIndexes.System)
			{
				color = GChat.TabListColor[5];
				value = Global.GetLang("[系统]：");
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.Bulletin)
			{
				color = GChat.TabListColor[0];
				value = Global.GetLang("[公告]：");
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.Map)
			{
				color = "0xffffff";
				value = (FormMini ? Global.GetLang("[附近]") : string.Empty) + StringUtil.substitute("{0}{1}：", new object[]
				{
					text2,
					GChat.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID, FormMini)
				});
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.World)
			{
				color = GChat.TabListColor[1];
				value = (FormMini ? Global.GetLang("[世界]") : string.Empty) + StringUtil.substitute("{0}{1}：", new object[]
				{
					text2,
					GChat.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID, FormMini)
				});
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.Faction)
			{
				color = GChat.TabListColor[2];
				value = (FormMini ? Global.GetLang("[战盟]") : string.Empty) + StringUtil.substitute("{0}{1}：", new object[]
				{
					text2,
					GChat.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID, FormMini)
				});
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.Team)
			{
				color = GChat.TabListColor[3];
				value = (FormMini ? Global.GetLang("[队伍]") : string.Empty) + StringUtil.substitute("{0}{1}：", new object[]
				{
					text2,
					GChat.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID, FormMini)
				});
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.Private)
			{
				if (chatTextItem.Status == 0)
				{
					color = GChat.TabListColor[4];
					if (chatTextItem.FromRoleID == Global.Data.roleData.RoleID)
					{
						value = (FormMini ? Global.GetLang("[私聊]") : string.Empty) + StringUtil.substitute(Global.GetLang("{0}："), new object[]
						{
							GChat.GetFormatedRolename(Global.GetLang("你"), chatTextItem.FromRoleID, FormMini)
						});
					}
					else
					{
						value = (FormMini ? Global.GetLang("[私聊]") : string.Empty) + StringUtil.substitute(Global.GetLang("{0}{1}："), new object[]
						{
							GChat.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID, FormMini),
							text2
						});
					}
				}
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.ZhenYing)
			{
				color = GChat.TabListColor[5];
				value = (FormMini ? Global.GetLang("[阵营]") : string.Empty) + StringUtil.substitute("{0}{1}：", new object[]
				{
					text2,
					GChat.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID, FormMini)
				});
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.FuBen)
			{
				color = GChat.TabListColor[5];
				if (Global.IsInKuaFuPlunderBattleMap())
				{
					chatTextItem.ChatIndex = ChatTypeIndexes.Map;
					value = (FormMini ? Global.GetLang("[附近]") : string.Empty) + StringUtil.substitute("{0}{1}：", new object[]
					{
						text2,
						GChat.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID, FormMini)
					});
				}
				else
				{
					value = (FormMini ? Global.GetLang("[副本]") : string.Empty) + StringUtil.substitute("{0}{1}：", new object[]
					{
						text2,
						GChat.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID, FormMini)
					});
				}
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.ArmyGroup)
			{
				color = GChat.TabListColor[5];
				value = (FormMini ? Global.GetLang("[军团]") : string.Empty) + StringUtil.substitute("{0}{1}：", new object[]
				{
					text2,
					GChat.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID, FormMini)
				});
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.Comp)
			{
				color = GChat.TabListColor[5];
				value = (FormMini ? Global.GetLang("[势力]") : string.Empty) + StringUtil.substitute("{0}{1}：", new object[]
				{
					text2,
					GChat.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID, FormMini)
				});
			}
			else if (!Global.isHaiWai && chatTextItem.ChatIndex == ChatTypeIndexes.Platform)
			{
				color = GChat.TabListColor[5];
				value = (FormMini ? Global.GetLang("[平台]") : string.Empty) + StringUtil.substitute("{0}{1}：", new object[]
				{
					text2,
					GChat.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID, FormMini)
				});
			}
			else if (chatTextItem.ChatIndex == ChatTypeIndexes.ZhanDui)
			{
				color = GChat.TabListColor[5];
				value = (FormMini ? Global.GetLang("[战队]") : string.Empty) + StringUtil.substitute("{0}{1}：", new object[]
				{
					text2,
					GChat.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID, FormMini)
				});
			}
			if (!FormMini)
			{
				color = "0xdac7ae";
			}
			return GChat.GetFormatedColorStr(value, color);
		}

		public static string FormatChatVoiceText(ChatTextItem chatTextItem, bool beSelfLeft = true, bool FormMini = false)
		{
			return GChat.FormatChatUserText(chatTextItem, beSelfLeft, FormMini);
		}

		public static void ClearAllVoiceState()
		{
			GChat.ILuYinSecondCout = -1;
			GChat.IsInLuYinAndSending = false;
			GChat.IsInvokeSendOver = false;
			GChat.IsSendOver = false;
		}

		public const string TongYongNameColor = "f7d66b";

		public const string OXTongYongColor = "0xdac7ae";

		public const string MapPreWord = "/s ";

		public const string WorldPreWord = "/w ";

		public const string FactionPreWord = "/g ";

		public const string TeamPreWord = "/t ";

		public const string PrivatePreWord = "/";

		public const string ZhenyingWord = "/z ";

		public const string FuBenWord = "/f ";

		public const string ArmyGroupWord = "/j ";

		public const string CompGroupWord = "/c ";

		public const string PlatformWord = "/pt ";

		public const string ZhanDuiWord = "/d ";

		public static int ILuYinSecondCout = -1;

		public static bool IsInLuYinAndSending = false;

		public static bool IsInvokeSendOver = false;

		public static bool IsSendOver = false;

		private static int mChatReceiveID = 0;

		public static List<ChatTextItem> AllChatTextList = new List<ChatTextItem>(20);

		public static List<ChatTextItem> WorldChatTextList = new List<ChatTextItem>(20);

		public static List<ChatTextItem> FactionChatTextList = new List<ChatTextItem>(20);

		public static List<ChatTextItem> TeamChatTextList = new List<ChatTextItem>(20);

		public static List<ChatTextItem> PrivateChatTextList = new List<ChatTextItem>(20);

		public static List<ChatTextItem> SystemChatTextList = new List<ChatTextItem>(20);

		public static List<ChatTextItem> SystemChatTextListBak = new List<ChatTextItem>(20);

		public static List<ChatTextItem> ZhenYingChatTextList = new List<ChatTextItem>(20);

		public static List<ChatTextItem> FuBenChatTextList = new List<ChatTextItem>(20);

		public static List<ChatTextItem> ArmyGroupChatTextList = new List<ChatTextItem>(20);

		public static List<ChatTextItem> CompGroupChatTextList = new List<ChatTextItem>(20);

		public static List<ChatTextItem> PlatformChatTextList = new List<ChatTextItem>(20);

		public static List<ChatTextItem> ZhanDuiChatTextList = new List<ChatTextItem>(20);

		public static string[] TabListColor = new string[]
		{
			"0xdec69c",
			"0xf9f702",
			"0x99ccff",
			"0x00ff00",
			"0xe000fa",
			"0xff9d08"
		};
	}
}
