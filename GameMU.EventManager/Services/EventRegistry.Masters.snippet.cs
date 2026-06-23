// =====================================================================================
//  CÁC MASTER (TỪ ĐIỂN ID GỐC) NGOÀI Goods.xml  —  dán vào EventRegistry.Files
//  Đã đối soát định lượng với data thật (xem % khớp ở mỗi ForeignKey).
//  Lưu ý: Map dùng IdAttr="Code" (KHÔNG phải "ID"); FK trỏ tới map phải set TargetIdAttr="Code".
// =====================================================================================

        // ===== Vật phẩm (master lớn nhất) =====
        // LƯU Ý HIỆU NĂNG: Goods.xml ~15MB / 9.720 Item. Khi đăng ký def này KÈM ForeignKeys,
        // back-reference engine (GetBackReferences) sẽ LoadRecords(goods) -> parse 15MB mỗi lần quét.
        // Nếu thấy chậm, cân nhắc cache LoadRecords theo file, hoặc bỏ ForeignKeys ở đây và chỉ
        // giữ def để làm TargetKey cho các file khác trỏ tới.
        new() {
            Key="goods", RelativePath="Goods.xml", DisplayName="Vật phẩm (Goods)",
            Category="Từ điển gốc", ItemElement="Item", IdAttr="ID", NameAttr="Title",
            Description="Từ điển ~9.720 vật phẩm — master được hầu hết file Config tham chiếu (GoodsList, Award, NeedGoods...).",
            ListColumns=new[]{"ID","Title","Categoriy","PriceOne","JinJie","XiLian"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="JinJie", TargetKey="goods",  TargetIdAttr="ID", Description="Thăng cấp/tiến giai -> Item.ID kế tiếp (self-ref). Khớp 100% (453/453).", MultiValue=false },
                new() { Field="XiLian", TargetKey="xilian", TargetIdAttr="ID", Description="Trỏ bảng tẩy luyện XiLianShuXing.xml. Khớp 100% (261/261). LƯU Ý: KHÔNG trỏ Goods (chỉ 3%).", MultiValue=false }
            }
        },

        // ===== Quái / Boss =====
        new() {
            Key="monster", RelativePath="Monsters.xml", DisplayName="Quái & Boss (Monsters)",
            Category="Từ điển gốc", ItemElement="Monster", IdAttr="ID", NameAttr="SName",
            Description="Từ điển ~2.064 quái/boss. Tham chiếu kỹ năng (SkillIDs->Magic), bản đồ (MapCode->Map), bảng rơi đồ (FallID).",
            ListColumns=new[]{"ID","SName","Level","MapCode","FallID","SkillIDs"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="SkillIDs", TargetKey="magic",   TargetIdAttr="ID",   Description="Kỹ năng của quái (->Magics.xml). Khớp 100% (187/187).", MultiValue=true, MultiSeparator=',' },
                new() { Field="MapCode",  TargetKey="map",     TargetIdAttr="Code", Description="Bản đồ quái xuất hiện (->MapConfig.xml). Khớp 99% (379/382, miss là map sự kiện).", MultiValue=true, MultiSeparator=',' },
                new() { Field="FallID",   TargetKey="monster", TargetIdAttr="ID",   Description="Chuỗi rơi đồ: trỏ tới một Monster khác (self-ref). Khớp 92% (392/427, miss là boss đặc biệt/liên server).", MultiValue=true, MultiSeparator=',' }
            }
        },

        // ===== Kỹ năng =====
        new() {
            Key="magic", RelativePath="Magics.xml", DisplayName="Kỹ năng (Magics)",
            Category="Từ điển gốc", ItemElement="Magic", IdAttr="ID", NameAttr="Name",
            Description="Từ điển ~448 kỹ năng. Được Monster.SkillIDs, vật phẩm học kỹ năng... tham chiếu.",
            ListColumns=new[]{"ID","Name","SkillType","ToOcuupation","MagicType"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="NextMagicID", TargetKey="magic", TargetIdAttr="ID", Description="Kỹ năng cấp kế tiếp (tự trỏ trong Magics.xml).", MultiValue=false }
            }
        },

        // ===== Bản đồ =====
        new() {
            Key="map", RelativePath="MapConfig.xml", DisplayName="Bản đồ (MapConfig)",
            Category="Từ điển gốc", ItemElement="Map", IdAttr="Code", NameAttr=null,
            Description="Từ điển bản đồ (khóa = Code). Được Monster.MapCode, NPC.MapCode, FuBen.MapCode tham chiếu.",
            ListColumns=new[]{"Code","BirthPosX","BirthPosY","BirthRadius"},
            Toggle=ToggleStrategy.None
        },

        // ===== NPC =====
        new() {
            Key="npc", RelativePath="npcs.xml", DisplayName="NPC",
            Category="Từ điển gốc", ItemElement="NPC", IdAttr="ID", NameAttr="SName",
            Description="Từ điển ~365 NPC. Tham chiếu bản đồ (MapCode->Map) và nhiệm vụ (Tasks->Task). Lưu ý: SaleID là ID cấu hình cửa hàng, KHÔNG phải Goods.ID.",
            ListColumns=new[]{"ID","SName","Function","MapCode","Tasks","SaleID"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="MapCode", TargetKey="map",  TargetIdAttr="Code", Description="Bản đồ NPC đứng (->MapConfig.xml). Khớp 98% (43/44).", MultiValue=true, MultiSeparator=',' },
                new() { Field="Tasks",   TargetKey="task", TargetIdAttr="ID",   Description="Nhiệm vụ gắn với NPC (->EraTask/LegionTasks). Khớp 100% (4/4).", MultiValue=true, MultiSeparator=',' }
                // KHÔNG map SaleID -> goods: đối soát cho 0% khớp (SaleID là ID bảng bán hàng riêng).
            }
        },

        // ===== Phụ bản (Dungeon) =====
        new() {
            Key="dungeon", RelativePath="FuBen.xml", DisplayName="Phụ bản (FuBen)",
            Category="Từ điển gốc", ItemElement="Copy", IdAttr="ID", NameAttr="CopyName",
            Description="Từ điển ~290 phụ bản. Tham chiếu bản đồ (MapCode->Map), boss (BossID->Monster), phần thưởng (RewardGoods->Goods).",
            ListColumns=new[]{"ID","CopyName","MapCode","BossID","MinLevel","RewardGoods"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="MapCode",     TargetKey="map",     TargetIdAttr="Code", Description="Bản đồ phụ bản (->MapConfig.xml). Khớp 100% (290/290).", MultiValue=false },
                new() { Field="BossID",      TargetKey="monster", TargetIdAttr="ID",   Description="Boss của phụ bản (->Monsters.xml). Khớp 98% (59/60, miss là boss liên server).", MultiValue=false },
                new() { Field="RewardGoods", TargetKey="",        Description="Vật phẩm thưởng (->Goods.xml), dạng 'id|id|id'. Khớp 100% (29/29).", MultiValue=true, MultiSeparator='|' }
            }
        },

        // ===== Nhiệm vụ =====
        new() {
            Key="task", RelativePath="EraTask.xml", DisplayName="Nhiệm vụ - Kỷ nguyên (EraTask)",
            Category="Từ điển gốc", ItemElement="EraTask", IdAttr="ID", NameAttr="TaskName",
            Description="Nhiệm vụ theo kỷ nguyên. Được NPC.Tasks tham chiếu.",
            ListColumns=new[]{"ID","EraID","EraStage","TaskName","Description"},
            Toggle=ToggleStrategy.None
        },
        new() {
            Key="task-legion", RelativePath="LegionTasks.xml", DisplayName="Nhiệm vụ - Liên minh (LegionTasks)",
            Category="Từ điển gốc", ItemElement="LegionTasks", IdAttr="ID", NameAttr="Name",
            Description="Nhiệm vụ liên minh/quân đoàn (13 mục). Phần thưởng/kiểu hoàn thành theo TypeID.",
            ListColumns=new[]{"ID","Name","CompleteType","Exp","Describtion"},
            Toggle=ToggleStrategy.None
        },

        // ===== Tẩy luyện (XiLian) — master mới, được Goods.XiLian trỏ tới =====
        new() {
            Key="xilian", RelativePath="XiLianShuXing.xml", DisplayName="Thuộc tính tẩy luyện (XiLianShuXing)",
            Category="Từ điển gốc", ItemElement="XiLian", IdAttr="ID", NameAttr="Name",
            Description="Bảng thuộc tính tẩy luyện cho trang bị. Được Goods.XiLian trỏ tới (khớp 100% - 261/261). NeedGoods->Goods.ID.",
            ListColumns=new[]{"ID","Name","NeedGoods","NeedJinBi","NeedZuanShi"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="NeedGoods", TargetKey="", Description="Vật phẩm cần để tẩy luyện (->Goods.xml), dạng 'id,sl'.", ParseRewardList=true }
            }
        },

        // =================================================================================
        //  GHI CHÚ: def "goods" (master vật phẩm) đã được đăng ký ở ĐẦU block này, kèm 2 FK
        //  tự-trỏ JinJie (->goods) và XiLian (->xilian). Khi dán, đảm bảo def "goods" và
        //  "xilian" đều có mặt để forward-link/back-reference resolve đúng.
        // =================================================================================
