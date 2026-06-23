using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class URLImage : Image
{
	protected override void Awake()
	{
		base.Awake();
		Material material = base.GetComponent<UITexture>().material;
		if (null != material)
		{
			base.GetComponent<UITexture>().material = (SpawnManager.Instantiate(material) as Material);
		}
	}

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(this.ImageURL) && ((null != this.Texture && null == this.Texture.mainTexture) || this.UpdateNow))
		{
			base.StartCoroutine<bool>(this.InitImage(), new TTMonoBehaviour.CoroutineExceptionHandler(this.CoroutineException));
			this.UpdateNow = false;
		}
	}

	public string URL
	{
		get
		{
			return this.ImageURL;
		}
		set
		{
			this.ImageURL = value;
			if (null != this.Texture)
			{
				if (!string.IsNullOrEmpty(this.ImageURL))
				{
					this.ShowImage(this.ImageURL);
				}
				else
				{
					this.SetTexture(null);
				}
			}
		}
	}

	public void ShowImage(string uriImage)
	{
		this.ImageURL = uriImage;
		if (this.ImageURL == null)
		{
			return;
		}
		if (!U3DUtils.ComponentIsEnabled(this))
		{
			this.UpdateNow = true;
			return;
		}
		base.Source = this._DefaultSource;
		if (!string.IsNullOrEmpty(this.ImageURL))
		{
			base.StartCoroutine<bool>(this.InitImage(), new TTMonoBehaviour.CoroutineExceptionHandler(this.CoroutineException));
		}
		else
		{
			this.SetTexture(null);
		}
	}

	public void ForceShow()
	{
		if (this.ImageURL == null || !U3DUtils.ComponentIsEnabled(this))
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.ImageURL))
		{
			base.StartCoroutine<bool>(this.InitImage(), new TTMonoBehaviour.CoroutineExceptionHandler(this.CoroutineException));
		}
		else
		{
			this.SetTexture(null);
		}
	}

	public ImageBrush DefaultSource
	{
		set
		{
			this._DefaultSource = value;
		}
	}

	private void CoroutineException()
	{
		if (this.ImageDownloadedErr != null)
		{
			this.ImageDownloadedErr(null);
		}
	}

	private string AddQJSuffix(string name)
	{
		if (name.EndsWith(".qj"))
		{
			return name;
		}
		return name + ".qj";
	}

	private IEnumerator InitImage()
	{
		WWW www = new WWW(PathUtils.WebPath(this.AddQJSuffix(this.ImageURL)));
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			www = new WWW(PathUtils.WebPath(this.ImageURL));
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				if (!Global.IsTuiGuangFenBao)
				{
					if (this.ImageDownloadedErr != null)
					{
						this.ImageDownloadedErr(null);
					}
					yield break;
				}
				WaitForCDNAsset cdnRequestImg = new WaitForCDNAsset(this.AddQJSuffix(this.ImageURL));
				yield return base.StartCoroutine(cdnRequestImg);
				if (!string.IsNullOrEmpty(cdnRequestImg.error) || cdnRequestImg.www == null || cdnRequestImg.www.textureNonReadable == null)
				{
					cdnRequestImg = new WaitForCDNAsset(this.ImageURL);
					yield return base.StartCoroutine(cdnRequestImg);
					if (!string.IsNullOrEmpty(cdnRequestImg.error) || cdnRequestImg.www == null || cdnRequestImg.www.textureNonReadable == null)
					{
						MUDebug.LogError<string>(new string[]
						{
							"NetImage 下载出错 " + cdnRequestImg.error + "  URL " + this.ImageURL
						});
						if (this.ImageDownloadedErr != null)
						{
							this.ImageDownloadedErr(null);
						}
						yield break;
					}
				}
				this.isCDNDownload = true;
				if (null == cdnRequestImg.www.assetBundle)
				{
					this.SetTexture(cdnRequestImg.www.textureNonReadable);
				}
				else
				{
					this.SetTexture(cdnRequestImg.www.assetBundle.mainAsset as Texture2D);
				}
			}
		}
		if (!this.isCDNDownload)
		{
			if (null == www.assetBundle)
			{
				this.SetTexture(www.textureNonReadable);
			}
			else
			{
				this.SetTexture(www.assetBundle.mainAsset as Texture2D);
			}
		}
		else
		{
			this.isCDNDownload = false;
		}
		if (this.ImageDownloaded != null)
		{
			this.ImageDownloaded(null);
		}
		www.Dispose();
		www = null;
		yield break;
	}

	public bool ToGrayBitmap
	{
		get
		{
			return this._ToGrayBitmap;
		}
		set
		{
			if (this._ToGrayBitmap != value)
			{
				this._ToGrayBitmap = value;
				this.SetTexture(this.Texture.mainTexture);
			}
		}
	}

	public void SetTexture(Texture texture)
	{
		if (texture && this.AutoWrapModeClamp)
		{
			texture.wrapMode = 1;
		}
		if (this._ToGrayBitmap)
		{
			if (null == this.SourceTexture && null != texture)
			{
				this.SourceTexture = texture;
				Material material = new Material(Shader.Find("Unlit/Gray"));
				material.mainTexture = texture;
				this.Texture.material = material;
			}
			else if (null == texture)
			{
				this.Texture.mainTexture = null;
				this.SourceTexture = null;
			}
			else
			{
				this.Texture.mainTexture = texture;
			}
		}
		else if (null != this.SourceTexture)
		{
			Material material2 = new Material(Shader.Find("Unlit/Transparent Colored"));
			material2.mainTexture = texture;
			this.SourceTexture = null;
			this.Texture.material = material2;
		}
		else if (null == texture)
		{
			this.Texture.mainTexture = null;
			this.SourceTexture = null;
		}
		else
		{
			this.Texture.mainTexture = texture;
		}
		if (this.AutoResize && null != this.Texture.mainTexture)
		{
			base.transform.localScale = new Vector3((float)this.Texture.mainTexture.width, (float)this.Texture.mainTexture.height, 1f);
		}
		if (this.AutoPerfect && null != this.Texture.mainTexture)
		{
			if (this.Texture.mainTexture.width % 2 != 0)
			{
				base.transform.localPosition = new Vector3(0.5f, base.transform.localPosition.y, base.transform.localPosition.z);
			}
			if (this.Texture.mainTexture.height % 2 != 0)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, -0.5f, base.transform.localPosition.z);
			}
		}
	}

	public int ItsSizeWidth
	{
		get
		{
			return this.Texture.mainTexture.width;
		}
	}

	public int ItsSizeHeight
	{
		get
		{
			return this.Texture.mainTexture.height;
		}
	}

	public float ActualHeight
	{
		get
		{
			return this._ActualHeight;
		}
		set
		{
			this._ActualHeight = value;
		}
	}

	public float ActualWidth
	{
		get
		{
			return this._ActualWidth;
		}
		set
		{
			this._ActualWidth = value;
		}
	}

	public string ImageURL;

	public bool AutoResize;

	public bool AutoPerfect;

	public bool AutoWrapModeClamp;

	private bool UpdateNow;

	protected ImageBrush _DefaultSource;

	private bool isCDNDownload;

	private Texture SourceTexture;

	private bool _ToGrayBitmap;

	private float _ActualHeight;

	private float _ActualWidth;

	public URLImage.ImageDownloadedEventHandler ImageDownloaded;

	public URLImage.ImageDownloadedEventHandler ImageDownloadedErr;

	public delegate void ImageDownloadedEventHandler(object obj);
}
