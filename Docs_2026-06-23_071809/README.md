# Tài liệu phân tích & quản lý Sự kiện GameMU

Bộ tài liệu này tổng hợp kết quả **phân tích hệ thống sự kiện** của GameServer MU,
dựa trên việc đọc trực tiếp các file XML trong `GameRes/GameRes/Config` và mã nguồn
`GameServer/Logic/ActivityNew`. Đây là cơ sở để xây dựng web app C# quản lý sự kiện
(`GameMU.EventManager`).

## Mục lục

| File | Nội dung |
|---|---|
| [01-Tong-quan-he-thong-su-kien.md](01-Tong-quan-he-thong-su-kien.md) | Tổng quan kiến trúc, các loại "sự kiện" trong game |
| [02-Danh-muc-file-XML-su-kien.md](02-Danh-muc-file-XML-su-kien.md) | Danh mục đầy đủ các file XML sự kiện + lược đồ (root/item/thuộc tính) |
| [03-Co-che-bat-tat-su-kien.md](03-Co-che-bat-tat-su-kien.md) | 3 cơ chế BẬT/TẮT sự kiện và cách áp dụng cho từng file |
| [04-Schema-chi-tiet-cac-file-chinh.md](04-Schema-chi-tiet-cac-file-chinh.md) | Giải nghĩa chi tiết thuộc tính các file sự kiện quan trọng |
| [05-Kien-truc-Web-App.md](05-Kien-truc-Web-App.md) | Kiến trúc, cách chạy và mở rộng `GameMU.EventManager` |
| [06-Ban-do-tinh-nang-XML.md](06-Ban-do-tinh-nang-XML.md) | **Bản đồ Tính năng → File XML** (quét từ mã nguồn GameServer, 186 manager) |
| [07-SystemParams.md](07-SystemParams.md) | **Phân tích SystemParams.xml** (từ điển 939 tham số điều khiển nhiều chức năng) |

> Ghi chú: tên gốc trong cấu hình là tiếng Trung (phiên âm Hán-Việt như JieRi = Tiết Nhật/Lễ hội,
> HuoDong = Hoạt động, HeFu = Hợp phục/Gộp server, ZhuanSheng = Chuyển sinh…). Tài liệu dùng
> tên gọi tiếng Việt cho dễ hiểu, đồng thời giữ tên file gốc để tra cứu.
