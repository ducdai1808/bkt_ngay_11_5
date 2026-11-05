using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace RepairServiceTracker;

public class Form1 : Form
{
    private readonly BindingList<DonSuaChua> _dsDon = new();
    private readonly BindingSource _binding = new();
    private readonly ErrorProvider _errors = new();
    private readonly TextBox txtMa = new(), txtTen = new(), txtSDT = new(), txtMoTa = new(), txtChiPhi = new(), txtTimKiem = new();
    private readonly ComboBox cboThietBi = new(), cboTrangThai = new(), cboLoc = new();
    private readonly DateTimePicker dtpNgay = new();
    private readonly PictureBox picAnh = new();
    private readonly DataGridView dgv = new();
    private readonly Label lblThongKe = new();
    private string _anhDangChon = "";
    private bool _dangNap = false;

    private static readonly string[] TrangThais = { "Mới tiếp nhận", "Đang sửa", "Chờ linh kiện", "Hoàn tất", "Đã hủy" };

    public Form1()
    {
        Text = "Repair Service Tracker - Quản lý đơn sửa chữa";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1100, 700);
        Size = new Size(1350, 850);
        Font = new Font("Segoe UI", 9F);
        BuildMenu();
        BuildInterface();
        ConfigureGrid();
        _binding.DataSource = _dsDon;
        dgv.DataSource = _binding;
        _dsDon.ListChanged += (_, _) => UpdateStatus();
        AddSampleData();
        UpdateStatus();
    }

    private void BuildMenu()
    {
        var menu = new MenuStrip();
        var file = new ToolStripMenuItem("Tệp");
        file.DropDownItems.Add("Xuất danh sách đang hiển thị ra CSV", null, (_, _) => ExportCsv());
        file.DropDownItems.Add(new ToolStripSeparator());
        file.DropDownItems.Add("Thoát", null, (_, _) => Close());
        menu.Items.Add(file);
        MainMenuStrip = menu;
        Controls.Add(menu);
    }

    private void BuildInterface()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10) };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 285));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        Controls.Add(root);

        var info = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 5, Padding = new Padding(8), CellBorderStyle = TableLayoutPanelCellBorderStyle.Single };
        info.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
        info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        info.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
        info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66));
        for (int i = 0; i < 5; i++) info.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
        root.Controls.Add(info, 0, 0);

        AddField(info, "Mã đơn:", txtMa, 0, 0);
        AddField(info, "Khách hàng:", txtTen, 2, 0);
        AddField(info, "Số điện thoại:", txtSDT, 0, 1);
        cboThietBi.DropDownStyle = ComboBoxStyle.DropDownList;
        cboThietBi.Items.AddRange(new object[] { "Điện thoại", "Laptop", "Máy tính bàn", "Máy tính bảng", "Tivi", "Thiết bị khác" });
        cboThietBi.SelectedIndex = 0;
        AddField(info, "Loại thiết bị:", cboThietBi, 2, 1);
        AddField(info, "Mô tả lỗi:", txtMoTa, 0, 2);
        txtMoTa.Multiline = true;
        txtMoTa.ScrollBars = ScrollBars.Vertical;
        dtpNgay.Format = DateTimePickerFormat.Custom;
        dtpNgay.CustomFormat = "dd/MM/yyyy";
        AddField(info, "Ngày tiếp nhận:", dtpNgay, 2, 2);
        AddField(info, "Chi phí (VNĐ):", txtChiPhi, 0, 3);
        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.Items.AddRange(TrangThais);
        cboTrangThai.SelectedIndex = 0;
        AddField(info, "Trạng thái:", cboTrangThai, 2, 3);

        var imagePanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        var btnChonAnh = new Button { Text = "Chọn ảnh thiết bị...", AutoSize = true, Margin = new Padding(3, 8, 12, 3) };
        btnChonAnh.Click += (_, _) => ChooseImage();
        var btnXoaAnh = new Button { Text = "Bỏ ảnh", AutoSize = true, Margin = new Padding(3, 8, 12, 3) };
        btnXoaAnh.Click += (_, _) => ClearImage();
        picAnh.Size = new Size(105, 65);
        picAnh.BorderStyle = BorderStyle.FixedSingle;
        picAnh.SizeMode = PictureBoxSizeMode.Zoom;
        imagePanel.Controls.AddRange(new Control[] { btnChonAnh, btnXoaAnh, picAnh });
        info.Controls.Add(new Label { Text = "Ảnh thiết bị:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 4);
        info.Controls.Add(imagePanel, 1, 4);
        info.SetColumnSpan(imagePanel, 3);

        var listPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        listPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        listPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.Controls.Add(listPanel, 0, 1);

        var tools = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true, Padding = new Padding(0, 4, 0, 2) };
        tools.Controls.Add(new Label { Text = "Tìm kiếm:", AutoSize = true, Margin = new Padding(3, 8, 3, 0) });
        txtTimKiem.Width = 220;
        txtTimKiem.PlaceholderText = "Mã đơn / tên / số điện thoại";
        txtTimKiem.TextChanged += (_, _) => ApplyFilter();
        tools.Controls.Add(txtTimKiem);
        tools.Controls.Add(new Label { Text = "Lọc trạng thái:", AutoSize = true, Margin = new Padding(12, 8, 3, 0) });
        cboLoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLoc.Items.Add("Tất cả");
        cboLoc.Items.AddRange(TrangThais);
        cboLoc.SelectedIndex = 0;
        cboLoc.SelectedIndexChanged += (_, _) => ApplyFilter();
        cboLoc.Width = 150;
        tools.Controls.Add(cboLoc);
        AddActionButton(tools, "Thêm", (_, _) => AddOrder());
        AddActionButton(tools, "Cập nhật", (_, _) => UpdateOrder());
        AddActionButton(tools, "Xóa", (_, _) => DeleteOrder());
        AddActionButton(tools, "Làm mới", (_, _) => ClearForm());
        listPanel.Controls.Add(tools, 0, 0);

        dgv.Dock = DockStyle.Fill;
        dgv.ReadOnly = true;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect = false;
        dgv.RowHeadersVisible = false;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.SelectionChanged += (_, _) => LoadSelectedOrder();
        listPanel.Controls.Add(dgv, 0, 1);

        var status = new StatusStrip();
        lblThongKe.Text = "Sẵn sàng";
        status.Items.Add(lblThongKe);
        root.Controls.Add(status, 0, 2);
    }

    private void AddField(TableLayoutPanel panel, string label, Control control, int col, int row)
    {
        panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, col, row);
        control.Dock = DockStyle.Fill;
        if (control is TextBox tb) tb.Margin = new Padding(5, 5, 8, 5);
        panel.Controls.Add(control, col + 1, row);
    }

    private void AddActionButton(FlowLayoutPanel panel, string text, EventHandler click)
    {
        var b = new Button { Text = text, AutoSize = true, Height = 29, Margin = new Padding(8, 2, 2, 2) };
        b.Click += click;
        panel.Controls.Add(b);
    }

    private void ConfigureGrid()
    {
        dgv.AutoGenerateColumns = false;
        AddColumn("Mã đơn", nameof(DonSuaChua.MaDon));
        AddColumn("Khách hàng", nameof(DonSuaChua.TenKhachHang));
        AddColumn("Số điện thoại", nameof(DonSuaChua.SoDienThoai));
        AddColumn("Thiết bị", nameof(DonSuaChua.LoaiThietBi));
        AddColumn("Ngày nhận", nameof(DonSuaChua.NgayTiepNhan), "dd/MM/yyyy");
        AddColumn("Chi phí dự kiến", nameof(DonSuaChua.ChiPhiDuKien), "N0");
        AddColumn("Trạng thái", nameof(DonSuaChua.TrangThai));
        AddColumn("Mô tả lỗi", nameof(DonSuaChua.MoTaLoi));
    }

    private void AddColumn(string header, string property, string? format = null)
    {
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = header, DataPropertyName = property, Name = property,
            DefaultCellStyle = { Format = format ?? "" }
        });
    }

    private void AddSampleData()
    {
        _dsDon.Add(new DonSuaChua { MaDon = "SC001", TenKhachHang = "Nguyễn Văn An", SoDienThoai = "0912345678", LoaiThietBi = "Laptop", MoTaLoi = "Máy không lên nguồn", NgayTiepNhan = DateTime.Today, ChiPhiDuKien = 850000, TrangThai = "Đang sửa" });
        _dsDon.Add(new DonSuaChua { MaDon = "SC002", TenKhachHang = "Trần Thu Hà", SoDienThoai = "0987654321", LoaiThietBi = "Điện thoại", MoTaLoi = "Thay màn hình bị vỡ", NgayTiepNhan = DateTime.Today.AddDays(-1), ChiPhiDuKien = 1200000, TrangThai = "Chờ linh kiện" });
        _dsDon.Add(new DonSuaChua { MaDon = "SC003", TenKhachHang = "Lê Minh Đức", SoDienThoai = "0901234567", LoaiThietBi = "Tivi", MoTaLoi = "Mất tín hiệu", NgayTiepNhan = DateTime.Today.AddDays(-2), ChiPhiDuKien = 500000, TrangThai = "Hoàn tất" });
    }

    private void ChooseImage()
    {
        using var dialog = new OpenFileDialog { Title = "Chọn ảnh thiết bị", Filter = "Tệp ảnh|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Tất cả tệp|*.*" };
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _anhDangChon = dialog.FileName;
            ShowImage(_anhDangChon);
        }
    }

    private void ShowImage(string path)
    {
        picAnh.Image?.Dispose();
        picAnh.Image = null;
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var temp = Image.FromStream(stream);
            picAnh.Image = new Bitmap(temp);
        }
    }

    private void ClearImage()
    {
        _anhDangChon = "";
        picAnh.Image?.Dispose();
        picAnh.Image = null;
    }

    private DonSuaChua? SelectedOrder => dgv.CurrentRow?.DataBoundItem as DonSuaChua;

    private void LoadSelectedOrder()
    {
        if (_dangNap || SelectedOrder is not DonSuaChua item) return;
        _dangNap = true;
        txtMa.Text = item.MaDon;
        txtTen.Text = item.TenKhachHang;
        txtSDT.Text = item.SoDienThoai;
        cboThietBi.SelectedItem = item.LoaiThietBi;
        txtMoTa.Text = item.MoTaLoi;
        dtpNgay.Value = item.NgayTiepNhan;
        txtChiPhi.Text = item.ChiPhiDuKien.ToString("0.##", CultureInfo.InvariantCulture);
        cboTrangThai.SelectedItem = item.TrangThai;
        _anhDangChon = item.DuongDanAnh;
        ShowImage(_anhDangChon);
        _dangNap = false;
    }

    private bool ValidateInput(bool isNew, DonSuaChua? current = null)
    {
        _errors.Clear();
        bool valid = true;
        void Error(Control c, string message) { _errors.SetError(c, message); valid = false; }

        string ma = txtMa.Text.Trim();
        if (ma == "") Error(txtMa, "Mã đơn không được để trống.");
        else if (_dsDon.Any(x => x.MaDon.Equals(ma, StringComparison.OrdinalIgnoreCase) && !ReferenceEquals(x, current)))
            Error(txtMa, "Mã đơn đã tồn tại.");

        if (string.IsNullOrWhiteSpace(txtTen.Text)) Error(txtTen, "Tên khách hàng không được để trống.");
        if (string.IsNullOrWhiteSpace(txtSDT.Text)) Error(txtSDT, "Số điện thoại không được để trống.");
        if (string.IsNullOrWhiteSpace(txtMoTa.Text)) Error(txtMoTa, "Mô tả lỗi là bắt buộc.");

        if (!decimal.TryParse(txtChiPhi.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out decimal cost) ||
            !decimal.TryParse(txtChiPhi.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out cost))
        {
            // Accept current-culture or invariant decimal input.
            if (!decimal.TryParse(txtChiPhi.Text.Trim(), out cost)) Error(txtChiPhi, "Chi phí phải là số hợp lệ.");
        }
        if (decimal.TryParse(txtChiPhi.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out cost) && cost < 0)
            Error(txtChiPhi, "Chi phí dự kiến không được âm.");
        else if (decimal.TryParse(txtChiPhi.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out cost) && cost < 0)
            Error(txtChiPhi, "Chi phí dự kiến không được âm.");

        return valid;
    }

    private DonSuaChua ReadForm()
    {
        decimal.TryParse(txtChiPhi.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var cost);
        if (cost == 0) decimal.TryParse(txtChiPhi.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out cost);
        return new DonSuaChua
        {
            MaDon = txtMa.Text.Trim(), TenKhachHang = txtTen.Text.Trim(), SoDienThoai = txtSDT.Text.Trim(),
            LoaiThietBi = cboThietBi.Text, MoTaLoi = txtMoTa.Text.Trim(), NgayTiepNhan = dtpNgay.Value.Date,
            ChiPhiDuKien = cost, TrangThai = cboTrangThai.Text, DuongDanAnh = _anhDangChon
        };
    }

    private void AddOrder()
    {
        if (!ValidateInput(true)) { MessageBox.Show("Vui lòng kiểm tra dữ liệu không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        _dsDon.Add(ReadForm());
        ApplyFilter();
        ClearForm();
    }

    private void UpdateOrder()
    {
        var selected = SelectedOrder;
        if (selected == null) { MessageBox.Show("Hãy chọn đơn cần cập nhật."); return; }
        if (!ValidateInput(false, selected)) { MessageBox.Show("Vui lòng kiểm tra dữ liệu không hợp lệ."); return; }
        var data = ReadForm();
        selected.MaDon = data.MaDon; selected.TenKhachHang = data.TenKhachHang; selected.SoDienThoai = data.SoDienThoai;
        selected.LoaiThietBi = data.LoaiThietBi; selected.MoTaLoi = data.MoTaLoi; selected.NgayTiepNhan = data.NgayTiepNhan;
        selected.ChiPhiDuKien = data.ChiPhiDuKien; selected.TrangThai = data.TrangThai; selected.DuongDanAnh = data.DuongDanAnh;
        ApplyFilter();
        UpdateStatus();
    }

    private void DeleteOrder()
    {
        var selected = SelectedOrder;
        if (selected == null) { MessageBox.Show("Hãy chọn đơn cần xóa."); return; }
        if (MessageBox.Show($"Bạn có chắc muốn xóa đơn {selected.MaDon}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            _dsDon.Remove(selected);
        ClearForm();
    }

    private void ClearForm()
    {
        _dangNap = true;
        txtMa.Clear(); txtTen.Clear(); txtSDT.Clear(); txtMoTa.Clear(); txtChiPhi.Clear();
        cboThietBi.SelectedIndex = 0; cboTrangThai.SelectedIndex = 0; dtpNgay.Value = DateTime.Today;
        ClearImage(); _errors.Clear(); dgv.ClearSelection();
        _dangNap = false;
        txtMa.Focus();
    }

    private void ApplyFilter()
    {
        string keyword = txtTimKiem.Text.Trim();
        string status = cboLoc.SelectedItem?.ToString() ?? "Tất cả";
        var rows = _dsDon.Where(x =>
            (status == "Tất cả" || x.TrangThai == status) &&
            (keyword == "" || x.MaDon.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
             x.TenKhachHang.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
             x.SoDienThoai.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToList();
        _binding.DataSource = new BindingList<DonSuaChua>(rows);
        dgv.DataSource = _binding;
        UpdateStatus(rows.Count);
    }

    private void UpdateStatus(int? visibleCount = null)
    {
        int count = visibleCount ?? _dsDon.Count(x =>
            (cboLoc.SelectedItem?.ToString() is not string s || s == "Tất cả" || x.TrangThai == s) &&
            (txtTimKiem.Text.Trim() == "" || x.MaDon.Contains(txtTimKiem.Text.Trim(), StringComparison.OrdinalIgnoreCase) ||
             x.TenKhachHang.Contains(txtTimKiem.Text.Trim(), StringComparison.OrdinalIgnoreCase) ||
             x.SoDienThoai.Contains(txtTimKiem.Text.Trim(), StringComparison.OrdinalIgnoreCase)));
        decimal totalUnfinished = _dsDon.Where(x => x.TrangThai != "Hoàn tất" && x.TrangThai != "Đã hủy").Sum(x => x.ChiPhiDuKien);
        lblThongKe.Text = $"Số đơn đang hiển thị: {count}    |    Tổng chi phí dự kiến chưa hoàn tất: {totalUnfinished:N0} VNĐ";
    }

    private void ExportCsv()
    {
        using var dialog = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "DanhSachDonSuaChua.csv", Title = "Xuất danh sách đơn sửa chữa" };
        if (dialog.ShowDialog() != DialogResult.OK) return;
        var rows = dgv.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow)
            .Select(r => r.DataBoundItem as DonSuaChua).Where(x => x != null).Cast<DonSuaChua>().ToList();
        var sb = new StringBuilder();
        sb.AppendLine("MaDon,TenKhachHang,SoDienThoai,LoaiThietBi,MoTaLoi,NgayTiepNhan,ChiPhiDuKien,TrangThai,DuongDanAnh");
        foreach (var x in rows)
        {
            string[] values = { x.MaDon, x.TenKhachHang, x.SoDienThoai, x.LoaiThietBi, x.MoTaLoi,
                x.NgayTiepNhan.ToString("dd/MM/yyyy"), x.ChiPhiDuKien.ToString("0", CultureInfo.InvariantCulture), x.TrangThai, x.DuongDanAnh };
            sb.AppendLine(string.Join(",", values.Select(CsvEscape)));
        }
        File.WriteAllText(dialog.FileName, "\uFEFF" + sb.ToString(), Encoding.UTF8);
        MessageBox.Show("Xuất CSV thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static string CsvEscape(string value) => "\"" + (value ?? "").Replace("\"", "\"\"") + "\"";
}
