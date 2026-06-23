namespace GameMU.EventManager.Models;

/// <summary>
/// Cach mot file su kien duoc "bat/tat".
/// </summary>
public enum ToggleStrategy
{
    /// <summary>Khong ho tro bat/tat (chi xem/sua).</summary>
    None,
    /// <summary>Co thuoc tinh co (vd IsOpen, Open) nhan gia tri On/Off.</summary>
    Flag,
    /// <summary>Dieu khien bang khung thoi gian FromDate/ToDate (hoac tuong duong).</summary>
    DateWindow,
    /// <summary>Khong co co san; tat = chuyen record sang file sidecar va xoa khoi XML goc.</summary>
    Park
}

/// <summary>
/// Mo ta mot file XML su kien duoc quan ly.
/// </summary>
public class EventFileDef
{
    public string Key { get; init; } = "";          // dinh danh dung cho URL (khong dau, khong cach)
    public string RelativePath { get; init; } = "";  // duong dan tuong doi tu GameResConfigPath
    public string DisplayName { get; init; } = "";    // ten hien thi tieng Viet
    public string Category { get; init; } = "";       // nhom
    public string Description { get; init; } = "";
    public string ItemElement { get; init; } = "";    // ten the con (vd Activity, System, EventCalendar)
    public string IdAttr { get; init; } = "ID";       // thuoc tinh dinh danh
    public string? NameAttr { get; init; }            // thuoc tinh ten (neu co)
    public string[] ListColumns { get; init; } = Array.Empty<string>(); // cot hien thi trong danh sach

    public ToggleStrategy Toggle { get; init; } = ToggleStrategy.Park;
    // Cho Flag:
    public string? FlagAttr { get; init; }
    public string FlagOn { get; init; } = "1";
    public string FlagOff { get; init; } = "0";
    // Cho DateWindow:
    public string FromAttr { get; init; } = "FromDate";
    public string ToAttr { get; init; } = "ToDate";
}

/// <summary>
/// Mot ban ghi su kien (anh xa thuoc tinh -> gia tri), giu nguyen thu tu thuoc tinh.
/// </summary>
public class EventRecord
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public bool Enabled { get; set; } = true;
    public string Status { get; set; } = "";   // mo ta trang thai (vd "Dang chay", "Het han", "Tat")
    public bool Parked { get; set; }            // nam trong sidecar disabled
    public string? Comment { get; set; }        // chu thich (XML comment) di kem, vd trong SystemParams.xml
    public Dictionary<string, string> Attributes { get; set; } = new();
}
