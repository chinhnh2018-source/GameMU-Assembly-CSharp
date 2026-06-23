using System;
using System.Collections.Generic;
using Server.Data;
using UnityEngine;

public class JueXingMockData : MonoBehaviour
{
	private void Awake()
	{
		JueXingShiData jueXingShiData = new JueXingShiData();
		jueXingShiData.AttackEquip = this.attackTaoZhuangId;
		jueXingShiData.DefenseEquip = this.defTaoZhuangId;
		jueXingShiData.JueXingJie = this.jieShu;
		jueXingShiData.JueXingJi = this.jieShu;
		TaoZhuangData taoZhuangData = new TaoZhuangData();
		taoZhuangData.ID = 1;
		taoZhuangData.ActiviteList = this.lstAttackJuexingShi;
		TaoZhuangData taoZhuangData2 = new TaoZhuangData();
		taoZhuangData2.ID = 1;
		taoZhuangData2.ActiviteList = this.lstDefJuexingShi;
		jueXingShiData.TaoZhuangList = new List<TaoZhuangData>();
		jueXingShiData.TaoZhuangList.Add(taoZhuangData);
		jueXingShiData.TaoZhuangList.Add(taoZhuangData2);
		JueXingData.SetSelfJueXingData(jueXingShiData);
		JueXingData.jueXingZhiChenNum = this.ownJueXingZhiChen;
		JueXingData.mockMaterialNum = this.materialNum;
	}

	public List<int> lstAttackJuexingShi;

	public List<int> lstDefJuexingShi;

	public int attackTaoZhuangId = 1;

	public int defTaoZhuangId = 2;

	public int ownJueXingZhiChen = 300;

	public int jieShu = 5;

	public int xingShu = 5;

	public int materialNum = 200;
}
