using GameMU.EventManager.Models;
using GameMU.EventManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameMU.EventManager;

/// <summary>
/// Mở rộng IEndpointRouteBuilder để đăng ký tất cả /api/* endpoints.
/// Gọi từ Program.cs: app.MapApiEndpoints();
/// </summary>
public static class ApiEndpoints
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api")
            .WithOpenApi()
            .WithTags("GameMU Event API");

        // ─── Files catalog ─────────────────────────────────────────────────
        api.MapGet("/files", (XmlEventService svc) =>
        {
            var result = EventRegistry.Files.Select(def => new
            {
                def.Key,
                def.DisplayName,
                def.Category,
                def.Description,
                def.RelativePath,
                def.Toggle,
                Exists = svc.FileExists(def),
                Count = svc.CountRecords(def)
            });
            return Results.Ok(result);
        })
        .WithSummary("Liệt kê tất cả XML files được quản lý");

        // ─── List records trong 1 file ──────────────────────────────────────
        api.MapGet("/events/{key}", (string key, XmlEventService svc) =>
        {
            var def = FindDef(key);
            if (def == null) return Results.NotFound(new { error = $"Không tìm thấy key: {key}" });
            if (!svc.FileExists(def)) return Results.NotFound(new { error = $"File chưa tồn tại: {def.RelativePath}" });

            try
            {
                var records = svc.LoadRecords(def);
                return Results.Ok(new
                {
                    def.Key,
                    def.DisplayName,
                    def.Category,
                    def.RelativePath,
                    def.Toggle,
                    def.ItemElement,
                    def.IdAttr,
                    def.ListColumns,
                    Count = records.Count,
                    Records = records
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, title: "Lỗi đọc file");
            }
        })
        .WithSummary("Lấy danh sách records của một file XML");

        // ─── Get 1 record ──────────────────────────────────────────────────
        api.MapGet("/events/{key}/{id}", (string key, string id, XmlEventService svc) =>
        {
            var def = FindDef(key);
            if (def == null) return Results.NotFound(new { error = $"Key không tồn tại: {key}" });

            try
            {
                var records = svc.LoadRecords(def);
                var record = records.FirstOrDefault(r => r.Id == id);
                if (record == null) return Results.NotFound(new { error = $"ID không tìm thấy: {id}" });
                return Results.Ok(record);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .WithSummary("Lấy một record theo ID");

        // ─── Update record ──────────────────────────────────────────────────
        api.MapPut("/events/{key}/{id}", async (
            string key, string id,
            [FromBody] Dictionary<string, string> attrs,
            XmlEventService svc) =>
        {
            var def = FindDef(key);
            if (def == null) return Results.NotFound(new { error = $"Key không tồn tại: {key}" });

            try
            {
                svc.UpdateRecord(def, id, attrs);
                return Results.Ok(new { success = true, key, id, message = "Đã lưu (backup tự động)." });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, title: "Lỗi lưu");
            }
        })
        .WithSummary("Cập nhật thuộc tính của một record");

        // ─── Add record ────────────────────────────────────────────────────
        api.MapPost("/events/{key}", async (
            string key,
            [FromBody] Dictionary<string, string> attrs,
            XmlEventService svc) =>
        {
            var def = FindDef(key);
            if (def == null) return Results.NotFound(new { error = $"Key không tồn tại: {key}" });

            try
            {
                svc.AddRecord(def, attrs);
                var newId = attrs.GetValueOrDefault(def.IdAttr, "?");
                return Results.Created($"/api/events/{key}/{newId}",
                    new { success = true, key, id = newId });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, title: "Lỗi thêm record");
            }
        })
        .WithSummary("Thêm record mới vào file XML");

        // ─── Delete record ──────────────────────────────────────────────────
        api.MapDelete("/events/{key}/{id}", (string key, string id, XmlEventService svc) =>
        {
            var def = FindDef(key);
            if (def == null) return Results.NotFound(new { error = $"Key không tồn tại: {key}" });

            try
            {
                svc.DeleteRecord(def, id);
                return Results.Ok(new { success = true, key, id, message = "Đã xóa (backup tự động)." });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, title: "Lỗi xóa");
            }
        })
        .WithSummary("Xóa một record");

        // ─── Toggle record ──────────────────────────────────────────────────
        api.MapPost("/events/{key}/{id}/toggle", (
            string key, string id,
            [FromQuery] bool enable,
            XmlEventService svc) =>
        {
            var def = FindDef(key);
            if (def == null) return Results.NotFound(new { error = $"Key không tồn tại: {key}" });
            if (def.Toggle == ToggleStrategy.None)
                return Results.BadRequest(new { error = $"File '{key}' không hỗ trợ bật/tắt." });

            try
            {
                svc.Toggle(def, id, enable);
                return Results.Ok(new
                {
                    success = true, key, id,
                    enabled = enable,
                    message = enable ? "Đã bật." : "Đã tắt."
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, title: "Lỗi toggle");
            }
        })
        .WithSummary("Bật hoặc tắt một record");

        // ─── Backup ────────────────────────────────────────────────────────
        api.MapPost("/events/{key}/backup", (string key, XmlEventService svc) =>
        {
            var def = FindDef(key);
            if (def == null) return Results.NotFound(new { error = $"Key không tồn tại: {key}" });

            try
            {
                svc.TriggerBackup(def);
                return Results.Ok(new { success = true, message = "Backup thành công." });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, title: "Lỗi backup");
            }
        })
        .WithSummary("Tạo backup thủ công cho file");

        api.MapGet("/backups", (XmlEventService svc) =>
        {
            var list = svc.ListBackups();
            return Results.Ok(new { count = list.Count, files = list });
        })
        .WithSummary("Liệt kê danh sách backup gần đây");

        // ─── Reload GameServer (trigger hot-reload) ─────────────────────────
        api.MapPost("/reload/{key}", async (
            string key,
            [FromServices] IConfiguration config,
            XmlEventService svc) =>
        {
            var def = FindDef(key);
            if (def == null) return Results.NotFound(new { error = $"Key không tồn tại: {key}" });

            // Gọi GameServer management endpoint để reload XML
            // Cấu hình trong appsettings.json: "GameServer:ManageUrl"
            var manageUrl = config["GameServer:ManageUrl"];
            if (string.IsNullOrEmpty(manageUrl))
            {
                return Results.Ok(new
                {
                    success = false,
                    message = "GameServer:ManageUrl chưa được cấu hình. Vui lòng thêm vào appsettings.json.",
                    hint = "Thêm: \"GameServer\": { \"ManageUrl\": \"http://localhost:9000\" }"
                });
            }

            try
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                // GameServer nhận reload command qua HTTP management port
                var reloadPath = def.RelativePath.Replace('\\', '/').ToLower();
                var res = await http.PostAsync(
                    $"{manageUrl.TrimEnd('/')}/manage/reloadxml?file={Uri.EscapeDataString(reloadPath)}",
                    null);
                if (res.IsSuccessStatusCode)
                    return Results.Ok(new { success = true, message = $"Reload '{reloadPath}' thành công." });
                return Results.Ok(new { success = false, status = (int)res.StatusCode,
                    message = "GameServer trả về lỗi." });
            }
            catch (Exception ex)
            {
                return Results.Ok(new { success = false, message = ex.Message,
                    hint = "Kiểm tra GameServer có đang chạy không." });
            }
        })
        .WithSummary("Trigger GameServer hot-reload XML sau khi save");

        // ─── SystemParams ───────────────────────────────────────────────────
        api.MapGet("/params/{name}", (string name, XmlEventService svc) =>
        {
            var def = EventRegistry.Files.FirstOrDefault(f => f.Key == "system-params");
            if (def == null) return Results.NotFound();

            try
            {
                var records = svc.LoadRecords(def);
                var param = records.FirstOrDefault(r =>
                    string.Equals(r.Id, name, StringComparison.OrdinalIgnoreCase));
                if (param == null) return Results.NotFound(new { error = $"Param không tồn tại: {name}" });
                return Results.Ok(new
                {
                    name = param.Id,
                    value = param.Attributes.GetValueOrDefault("Value", ""),
                    comment = param.Comment
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .WithSummary("Lấy giá trị một SystemParam");

        api.MapPut("/params/{name}", async (
            string name,
            [FromBody] ParamUpdateRequest req,
            XmlEventService svc) =>
        {
            var def = EventRegistry.Files.FirstOrDefault(f => f.Key == "system-params");
            if (def == null) return Results.NotFound();

            try
            {
                svc.UpdateRecord(def, name, new Dictionary<string, string>
                {
                    ["Value"] = req.Value
                });
                return Results.Ok(new { success = true, name, value = req.Value,
                    note = "Cần restart GameServer để có hiệu lực." });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .WithSummary("Cập nhật giá trị SystemParam (cần restart server)");
    }

    // ─── Helper ──────────────────────────────────────────────────────────────
    private static EventFileDef? FindDef(string key) =>
        EventRegistry.Files.FirstOrDefault(f =>
            string.Equals(f.Key, key, StringComparison.OrdinalIgnoreCase));
}

/// Request body cho PUT /api/params/{name}
public record ParamUpdateRequest(string Value);

        // ─── Export ZIP ──────────────────────────────────────────────
        api.MapGet("/export", (
            [FromQuery] string? keys,
            [FromServices] ZipService zip) =>
        {
            var filterKeys = string.IsNullOrEmpty(keys)
                ? null
                : keys.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

            try
            {
                var data = zip.ExportAll(filterKeys);
                var filename = $"GameMU_Config_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
                return Results.File(data, "application/zip", filename);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, title: "Lỗi export");
            }
        })
        .WithSummary("Xuất XML files thành ZIP. ?keys=k1,k2 để chọn file (mặc định: tất cả)");

        // ─── Import ZIP ───────────────────────────────────────────────
        api.MapPost("/import", async (
            HttpRequest request,
            [FromServices] ZipService zip) =>
        {
            if (!request.HasFormContentType)
                return Results.BadRequest(new { error = "Upload form với file .zip." });

            var file = request.Form.Files.FirstOrDefault();
            if (file == null || file.Length == 0)
                return Results.BadRequest(new { error = "Không có file được upload." });

            if (!file.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                return Results.BadRequest(new { error = "Chỉ chấp nhận file .zip." });

            using var stream = file.OpenReadStream();
            var (imported, skipped, errors) = zip.ImportZip(stream);
            return Results.Ok(new
            {
                success = errors.Count == 0,
                imported, skipped, errors,
                message = $"Đã import {imported} file. Bỏ qua {skipped}. Lỗi: {errors.Count}."
            });
        })
        .WithSummary("Nhập ZIP bundle — ghi đè XML files (tự backup trước)");

    }
}