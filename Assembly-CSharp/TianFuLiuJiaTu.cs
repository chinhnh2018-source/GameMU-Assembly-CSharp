using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class TianFuLiuJiaTu : UserControl
{
	protected override void InitializeComponent()
	{
		if (this.LiuJiaoXing != null)
		{
			this.meshfilter = (MeshFilter)this.LiuJiaoXing.GetComponent(typeof(MeshFilter));
		}
		this.mesh = this.meshfilter.mesh;
		this.vertices = new Vector3[7];
		this.triangles = new int[18];
		this.vertices[0] = new Vector3(0f, 0f, 0f);
		this.vertices[1] = new Vector3(0f, 2f, 0f);
		this.vertices[2] = new Vector3(1.6f, 1f, 0f);
		this.vertices[3] = new Vector3(1.6f, -1f, 0f);
		this.vertices[4] = new Vector3(0f, -2f, 0f);
		this.vertices[5] = new Vector3(-1.6f, -1f, 0f);
		this.vertices[6] = new Vector3(-1.6f, 1f, 0f);
		this.triangles[0] = 0;
		this.triangles[1] = 1;
		this.triangles[2] = 2;
		this.triangles[3] = 3;
		this.triangles[4] = 0;
		this.triangles[5] = 2;
		this.triangles[6] = 4;
		this.triangles[7] = 0;
		this.triangles[8] = 3;
		this.triangles[9] = 0;
		this.triangles[10] = 4;
		this.triangles[11] = 5;
		this.triangles[12] = 0;
		this.triangles[13] = 5;
		this.triangles[14] = 6;
		this.triangles[15] = 0;
		this.triangles[16] = 6;
		this.triangles[17] = 1;
		this.mesh.vertices = this.vertices;
		this.mesh.triangles = this.triangles;
		this.InitStr();
	}

	private void InitStr()
	{
		this.LengXue.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("冷血一击")
		});
		this.WuQing.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("无情一击")
		});
		this.YeMan.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("野蛮一击")
		});
		this.DiKangLengXue.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("抵抗冷血")
		});
		this.DiKangWuQing.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("抵抗无情")
		});
		this.DiKangYeMan.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("抵抗野蛮")
		});
	}

	private new void Start()
	{
	}

	public void setLiuJiaoXing(Item item)
	{
		this.smooth = 0.05f;
		this.SetVertices(item);
		this.Isbool = true;
	}

	public void SetVertices(Item item)
	{
		this.vertices1[0] = new Vector3(0f, 0f, 0f);
		this.vertices1[1] = new Vector3(this.vertices[1].x + this.vertices[1].x * item.ShuXing_1, this.vertices[1].y + this.vertices[1].y * item.ShuXing_1, 0f);
		this.vertices1[2] = new Vector3(this.vertices[2].x + this.vertices[2].x * item.ShuXing_2, this.vertices[2].y + this.vertices[2].y * item.ShuXing_2, 0f);
		this.vertices1[3] = new Vector3(this.vertices[3].x + this.vertices[3].x * item.ShuXing_3, this.vertices[3].y + this.vertices[3].y * item.ShuXing_3, 0f);
		this.vertices1[4] = new Vector3(this.vertices[4].x + this.vertices[4].x * item.ShuXing_4, this.vertices[4].y + this.vertices[4].y * item.ShuXing_4, 0f);
		this.vertices1[5] = new Vector3(this.vertices[5].x + this.vertices[5].x * item.ShuXing_5, this.vertices[5].y + this.vertices[5].y * item.ShuXing_5, 0f);
		this.vertices1[6] = new Vector3(this.vertices[6].x + this.vertices[6].x * item.ShuXing_6, this.vertices[6].y + this.vertices[6].y * item.ShuXing_6, 0f);
	}

	private new void Update()
	{
		if (this.Isbool)
		{
			if (this.smooth > 1f)
			{
				this.Isbool = false;
			}
			this.smooth += 0.05f;
			Vector3[] array = new Vector3[]
			{
				new Vector3(0f, 0f, 0f),
				Vector3.Lerp(this.vertices[1], this.vertices1[1], this.smooth),
				Vector3.Lerp(this.vertices[2], this.vertices1[2], this.smooth),
				Vector3.Lerp(this.vertices[3], this.vertices1[3], this.smooth),
				Vector3.Lerp(this.vertices[4], this.vertices1[4], this.smooth),
				Vector3.Lerp(this.vertices[5], this.vertices1[5], this.smooth),
				Vector3.Lerp(this.vertices[6], this.vertices1[6], this.smooth)
			};
			this.mesh.vertices = array;
		}
	}

	private MeshFilter meshfilter;

	private Mesh mesh;

	private Vector3[] vertices;

	private int[] triangles;

	private Vector3[] vertices1 = new Vector3[7];

	private float smooth = 0.05f;

	public bool Isbool;

	public UILabel LengXue;

	public UILabel WuQing;

	public UILabel YeMan;

	public UILabel DiKangLengXue;

	public UILabel DiKangWuQing;

	public UILabel DiKangYeMan;

	public GameObject LiuJiaoXing;
}
