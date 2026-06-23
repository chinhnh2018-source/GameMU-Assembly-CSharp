using System;
using UnityEngine;

public class ScrollGrid : MonoBehaviour
{
	private void Awake()
	{
		this.sView = base.transform.GetComponentInParent<UIDraggablePanel>();
		this.panel = this.sView.transform.GetComponentInParent<UIPanel>();
		this.panelTransfrom = this.panel.gameObject.transform;
	}

	public void SetGrid(int imax, ScrollGrid.ScrollGridSetItem sc)
	{
		if (!this.moveScrollViewLessMaxLine && imax < this.defaultShowMaxLine)
		{
			this.sView.enabled = false;
		}
		if (this.defaultMaxLine > imax)
		{
			this.defaultMaxLine = imax;
		}
		this.initGrid();
		this.indexMax = imax;
		this.scrollGridSetItem = sc;
		if (this.scrollGridSetItem != null)
		{
			this.scrollGridSetItem(this.itemTrans, this.indexStar, this.indexEnd);
		}
		this.SetMessage();
	}

	public void SetGrid(int imax, ScrollGrid.ScrollGridSetItem sc, ScrollGrid.ScrollGridMessage sm)
	{
		this.scrollGridMessage = sm;
		this.SetGrid(imax, sc);
	}

	public void NextIndex()
	{
		int num = this.GetShowStartIndex();
		this.GotoIndex(num + 1);
	}

	public void PreIndex()
	{
		int num = this.GetShowStartIndex();
		this.GotoIndex(num - 1);
	}

	public void GotoIndex(int index)
	{
		if (index < 0)
		{
			return;
		}
		if (index > this.indexMax - this.defaultShowMaxLine)
		{
			return;
		}
		float num = this.GetPanelPositionByIndex(index);
		num = Mathf.Abs(num);
		Vector3 zero = Vector3.zero;
		Vector2 zero2 = Vector2.zero;
		if (this.movement == ScrollGrid.Movement.Horizontal)
		{
			zero.x = this.panel.clipSoftness.x - num;
			zero2.x = num + this.panel.clipSoftness.x;
			if (this.panelTransfrom.localPosition.x == num)
			{
				return;
			}
		}
		else if (this.movement == ScrollGrid.Movement.Vertical)
		{
			zero.y = num - this.panel.clipSoftness.y;
			zero2.y = -num + this.panel.clipSoftness.y;
			if (this.panelTransfrom.localPosition.y == num)
			{
				return;
			}
		}
		SpringPanel.Begin(this.sView.panel.cachedGameObject, zero, this.autoVelocity);
	}

	private void initGrid()
	{
		this.ClearChild();
		this.itemTrans = new Transform[this.defaultMaxLine];
		for (int i = 0; i < this.defaultMaxLine; i++)
		{
			this.itemTrans[i] = this.CreatItemTransfrom(i);
		}
		this.movement = ScrollGrid.Movement.Horizontal;
		if (this.sView.scale.x > 0f)
		{
			this.movement = ScrollGrid.Movement.Horizontal;
		}
		else if (this.sView.scale.y > 0f)
		{
			this.movement = ScrollGrid.Movement.Vertical;
		}
		this.sView.onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
		this.ResetPostion();
		this.indexStar = 0;
		this.indexEnd = this.defaultMaxLine - 1;
		this.indexMax = 0;
		this.panelPos = Vector3.zero;
		this.lastIndex = 0;
		this.showStartIndex = 0;
		if (this.scrollGridSetItem != null)
		{
			this.scrollGridSetItem(this.itemTrans, this.indexStar, this.indexEnd);
		}
		this.SetMessage();
		this.ResetScrollPanelPosition();
	}

	private void ResetPostion()
	{
		float num = 0f;
		bool flag = false;
		Vector3 zero = Vector3.zero;
		float num2 = 0f;
		float num3 = 0f;
		if (this.movement == ScrollGrid.Movement.Horizontal)
		{
			num2 = this.vSize.x;
			num3 = this.size.x;
		}
		else if (this.movement == ScrollGrid.Movement.Vertical)
		{
			num2 = this.vSize.y;
			num3 = this.size.y;
		}
		for (int i = 0; i < this.itemTrans.Length; i++)
		{
			float num4;
			if (!flag)
			{
				num = num2 / 2f - num3 / 2f;
				if (num > num2 / 2f)
				{
					num = num2 / 2f;
				}
				flag = true;
				num4 = num;
			}
			else
			{
				num4 = num - (num3 + (float)this.space) * (float)i;
			}
			if (this.movement == ScrollGrid.Movement.Horizontal)
			{
				zero.x = -num4;
			}
			else if (this.movement == ScrollGrid.Movement.Vertical)
			{
				zero.y = num4;
			}
			this.itemTrans[i].localPosition = zero;
		}
	}

	private void SwitchItem(int id)
	{
		this.indexStar = id;
		if (this.movement == ScrollGrid.Movement.Horizontal)
		{
			if (this.indexStar > 0)
			{
				this.indexStar = 0;
				return;
			}
			if (this.indexStar < -(this.indexMax - this.defaultMaxLine))
			{
				this.indexStar = this.indexMax - this.defaultMaxLine;
				return;
			}
			this.indexStar = Mathf.Abs(this.indexStar);
		}
		else if (this.movement == ScrollGrid.Movement.Vertical)
		{
			if (this.indexStar < 0)
			{
				this.indexStar = 0;
				return;
			}
			if (this.indexStar > this.indexMax - this.defaultMaxLine)
			{
				this.indexStar = this.indexMax - this.defaultMaxLine;
				return;
			}
		}
		if (this.lastIndex != this.indexStar)
		{
			if (this.lastIndex < this.indexStar)
			{
				Transform transform = this.itemTrans[0];
				for (int i = 0; i < this.itemTrans.Length - 1; i++)
				{
					this.itemTrans[i] = this.itemTrans[i + 1];
				}
				Vector3 localPosition = transform.localPosition;
				if (this.movement == ScrollGrid.Movement.Horizontal)
				{
					float num = this.itemTrans[this.itemTrans.Length - 2].localPosition.x + this.size.x + (float)this.space;
					localPosition.x = num;
				}
				else if (this.movement == ScrollGrid.Movement.Vertical)
				{
					float num = this.itemTrans[this.itemTrans.Length - 2].localPosition.y - this.size.y - (float)this.space;
					localPosition.y = num;
				}
				transform.localPosition = localPosition;
				this.itemTrans[this.itemTrans.Length - 1] = transform;
			}
			else
			{
				Transform transform2 = this.itemTrans[this.itemTrans.Length - 1];
				for (int j = this.itemTrans.Length - 1; j > 0; j--)
				{
					this.itemTrans[j] = this.itemTrans[j - 1];
				}
				Vector3 localPosition2 = transform2.localPosition;
				if (this.movement == ScrollGrid.Movement.Horizontal)
				{
					float num2 = this.itemTrans[1].localPosition.x - this.size.x - (float)this.space;
					localPosition2.x = num2;
				}
				else if (this.movement == ScrollGrid.Movement.Vertical)
				{
					float num2 = this.itemTrans[1].localPosition.y + this.size.y + (float)this.space;
					localPosition2.y = num2;
				}
				transform2.localPosition = localPosition2;
				this.itemTrans[0] = transform2;
			}
			this.lastIndex = this.indexStar;
			if (this.scrollGridSetItem != null)
			{
				this.scrollGridSetItem(this.itemTrans, this.indexStar, this.indexEnd);
			}
			this.sView.ResetPosition();
		}
	}

	private void ResetScrollPanelPosition()
	{
		if (this.movement == ScrollGrid.Movement.Horizontal)
		{
			if (this.indexStar == 0)
			{
				this.ResetPostion();
				Vector3 zero = Vector3.zero;
				zero.x = this.panel.clipSoftness.x;
				Vector2 zero2 = Vector2.zero;
				zero2.x = -this.panel.clipSoftness.x;
				this.panelTransfrom.localPosition = zero;
				Vector4 clipRange = this.panel.clipRange;
				Vector4 clipRange2;
				clipRange2..ctor(zero2.x, zero2.y, clipRange.z, clipRange.w);
				this.panel.clipRange = clipRange2;
			}
		}
		else if (this.movement == ScrollGrid.Movement.Vertical && this.indexStar == 0)
		{
			this.ResetPostion();
			Vector3 zero3 = Vector3.zero;
			zero3.y = -this.panel.clipSoftness.y;
			Vector2 zero4 = Vector2.zero;
			zero4.y = this.panel.clipSoftness.y;
			this.panelTransfrom.localPosition = zero3;
			Vector4 clipRange3 = this.panel.clipRange;
			Vector4 clipRange4;
			clipRange4..ctor(zero4.x, zero4.y, clipRange3.z, clipRange3.w);
			this.panel.clipRange = clipRange4;
		}
	}

	private void SetItemPostion()
	{
		float positionByIndex = this.GetPositionByIndex(this.indexStar);
		Vector3 zero = Vector3.zero;
		float num = 0f;
		if (this.movement == ScrollGrid.Movement.Horizontal)
		{
			num = this.size.x;
		}
		else if (this.movement == ScrollGrid.Movement.Vertical)
		{
			num = this.size.y;
		}
		for (int i = 0; i < this.itemTrans.Length; i++)
		{
			float num2 = positionByIndex - (num + (float)this.space) * (float)i;
			if (this.movement == ScrollGrid.Movement.Horizontal)
			{
				zero.x = -num2;
			}
			else if (this.movement == ScrollGrid.Movement.Vertical)
			{
				zero.y = num2;
			}
			this.itemTrans[i].localPosition = zero;
		}
	}

	public float GetPositionByIndex(int index)
	{
		Vector3 zero = Vector3.zero;
		float num = 0f;
		float num2 = 0f;
		if (this.movement == ScrollGrid.Movement.Horizontal)
		{
			num = this.vSize.x;
			num2 = this.size.x;
		}
		else if (this.movement == ScrollGrid.Movement.Vertical)
		{
			num = this.vSize.y;
			num2 = this.size.y;
		}
		float num3 = num / 2f - num2 / 2f;
		if (num3 > num / 2f)
		{
			num3 = num / 2f;
		}
		return num3 - (num2 + (float)this.space) * (float)index;
	}

	public float GetPanelPositionByIndex(int index)
	{
		if (this.movement == ScrollGrid.Movement.Horizontal)
		{
			return (this.size.x + (float)this.space) * (float)index;
		}
		if (this.movement == ScrollGrid.Movement.Vertical)
		{
			return (this.size.y + (float)this.space) * (float)index;
		}
		return 0f;
	}

	private int GetShowStartIndex()
	{
		Vector3 localPosition = this.panelTransfrom.localPosition;
		if (this.movement == ScrollGrid.Movement.Horizontal)
		{
			if (this.indexStar > 0)
			{
				int num = (int)((localPosition.x - this.panel.clipSoftness.x) / (this.size.x + (float)this.space));
				return Mathf.Abs(num);
			}
		}
		else if (this.movement == ScrollGrid.Movement.Vertical && this.indexStar > 0)
		{
			return (int)((localPosition.y + this.panel.clipSoftness.y) / (this.size.y + (float)this.space));
		}
		return this.indexStar;
	}

	private Transform CreatItemTransfrom(int index)
	{
		GameObject gameObject = Object.Instantiate(this.ItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		if (gameObject != null)
		{
			gameObject.transform.parent = base.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.name = "ScrollGrid Item " + index.ToString();
			return gameObject.transform;
		}
		Debug.LogError("scrollGrid creat item prefab ");
		return null;
	}

	public void ClearChild()
	{
		if (base.transform.childCount < 1)
		{
			return;
		}
		Transform child = base.transform.GetChild(0);
		child.parent = null;
		Object.Destroy(child.gameObject);
		this.ClearChild();
	}

	private void onDragStarted()
	{
	}

	private void onDragFinished()
	{
		this.SetItemPostion();
		if (this.centerScrollViewOnChild)
		{
			this.GotoIndex(this.GetShowStartIndex());
		}
	}

	private void onMomentumMove()
	{
	}

	private void onStoppedMoving()
	{
		this.ResetScrollPanelPosition();
	}

	private void LateUpdate()
	{
		this.panelPos = this.panel.gameObject.transform.localPosition;
		if (this.movement == ScrollGrid.Movement.Horizontal)
		{
			this.stepIndex = (int)(this.panelPos.x / (this.size.x + (float)this.space));
		}
		else if (this.movement == ScrollGrid.Movement.Vertical)
		{
			this.stepIndex = (int)(this.panelPos.y / (this.size.y + (float)this.space));
		}
		this.SwitchItem(this.stepIndex);
		this.SetMessage();
	}

	private void SetMessage()
	{
		if (this.scrollGridMessage != null)
		{
			int num = this.GetShowStartIndex();
			if (num == 0)
			{
				if (this.indexMax <= this.defaultShowMaxLine)
				{
					this.scrollGridMessage(0);
				}
				else
				{
					this.scrollGridMessage(-1);
				}
			}
			else if (this.indexMax - this.defaultShowMaxLine > num)
			{
				if (this.movement == ScrollGrid.Movement.Horizontal)
				{
					if (num + 1 == this.indexMax - this.defaultShowMaxLine)
					{
						if (Mathf.Abs(this.itemTrans[this.itemTrans.Length - 1].localPosition.x) == Mathf.Abs(this.GetPositionByIndex(this.indexMax - 1)))
						{
							this.scrollGridMessage(1);
						}
						else
						{
							this.scrollGridMessage(2);
						}
					}
					else
					{
						this.scrollGridMessage(2);
					}
				}
				else if (this.movement == ScrollGrid.Movement.Vertical)
				{
					if (num + 1 == this.indexMax - this.defaultShowMaxLine)
					{
						if (Mathf.Abs(this.itemTrans[this.itemTrans.Length - 1].localPosition.y) == Mathf.Abs(this.GetPositionByIndex(this.indexMax - 1)))
						{
							this.scrollGridMessage(1);
						}
						else
						{
							this.scrollGridMessage(2);
						}
					}
					else
					{
						this.scrollGridMessage(2);
					}
				}
			}
			else
			{
				this.scrollGridMessage(1);
			}
		}
	}

	private ScrollGrid.ScrollGridSetItem scrollGridSetItem;

	private ScrollGrid.ScrollGridMessage scrollGridMessage;

	public int defaultShowMaxLine;

	public int defaultMaxLine;

	public int space;

	public Vector2 size;

	public float autoVelocity = 8f;

	public bool moveScrollViewLessMaxLine = true;

	public bool centerScrollViewOnChild;

	private int indexStar;

	private int indexEnd;

	private int indexMax;

	private Vector3 panelPos = Vector3.zero;

	private int lastIndex;

	private int showStartIndex;

	private UIPanel panel;

	private UIDraggablePanel sView;

	public Vector2 vSize;

	private ScrollGrid.Movement movement;

	private Transform panelTransfrom;

	public GameObject ItemPrefab;

	private Transform[] itemTrans;

	private int stepIndex;

	public enum Movement
	{
		Horizontal,
		Vertical
	}

	public delegate void ScrollGridSetItem(Transform[] trans, int start, int end);

	public delegate void ScrollGridMessage(int index);
}
