using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LabEquipmentCheckout;

public class Form1 : Form
{
    private readonly BindingList<PhieuMuon> danhSach = new();
    private readonly BindingSource bindingSource = new();
    private readonly ErrorProvider errorProvider = new();

    private readonly TextBox txtMaPhieu = new();
    private readonly TextBox txtMaSV = new();
    private readonly TextBox txtHoTen = new();
    private readonly ComboBox cboThietBi = new();
    private readonly NumericUpDown nudSoLuong = new();
    private readonly DateTimePicker dtpNgayMuon = new();
    private readonly DateTimePicker dtpHanTra = new();
    private readonly DateTimePicker dtpNgayTra = new();
    private readonly CheckBox chkDaTra = new();

    private readonly TextBox txtTimKiem = new();
    private readonly ComboBox cboLocTrangThai = new();
    private readonly DataGridView dgvPhieuMuon = new();

    private readonly ToolStripStatusLabel lblTongPhieu = new();
    private readonly ToolStripStatusLabel lblQuaHan = new();

    private PhieuMuon? phieuDangChon;

    public Form1()
    {
        Text = "Lab Equipment Checkout";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(1250, 780);
        MinimumSize = new Size(1000, 680);
        BackColor = Color.FromArgb(245, 247, 250);
        Font = new Font("Segoe UI", 9.5F);

        errorProvider.ContainerControl = this;

        TaoGiaoDien();
        KhoiTaoDuLieu();
        HienThiDanhSach();
    }

    private void TaoGiaoDien()
    {
        var menu = new MenuStrip();
        var mnuFile = new ToolStripMenuItem("Tệp");
        var mnuExport = new ToolStripMenuItem("Xuất danh sách đang lọc ra CSV");
        var mnuExit = new ToolStripMenuItem("Thoát");
        mnuExport.Click += (_, _) => XuatCSV();
        mnuExit.Click += (_, _) => Close();
        mnuFile.DropDownItems.AddRange(new ToolStripItem[]
        {
            mnuExport, new ToolStripSeparator(), mnuExit
        });
        menu.Items.Add(mnuFile);
        MainMenuStrip = menu;
        Controls.Add(menu);

        var status = new StatusStrip();
        lblTongPhieu.Text = "Tổng số phiếu: 0";
        lblQuaHan.Text = "Số phiếu quá hạn: 0";
        status.Items.Add(lblTongPhieu);
        status.Items.Add(new ToolStripStatusLabel { Spring = true });
        status.Items.Add(lblQuaHan);
        Controls.Add(status);

        var main = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(12)
        };
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 270));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(main);
        main.BringToFront();

        var grpInput = new GroupBox
        {
            Text = "THÔNG TIN PHIẾU MƯỢN",
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        main.Controls.Add(grpInput, 0, 0);

        var input = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 5,
            Padding = new Padding(5),
            Font = new Font("Segoe UI", 9.5F)
        };
        input.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
        input.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        input.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
        input.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 5; i++)
            input.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
        grpInput.Controls.Add(input);

        CauHinhInput();
        ThemTruong(input, "Mã phiếu:", txtMaPhieu, 0, 0);
        ThemTruong(input, "Mã sinh viên:", txtMaSV, 0, 2);
        ThemTruong(input, "Họ tên sinh viên:", txtHoTen, 1, 0);
        ThemTruong(input, "Thiết bị:", cboThietBi, 1, 2);
        ThemTruong(input, "Số lượng:", nudSoLuong, 2, 0);
        ThemTruong(input, "Ngày mượn:", dtpNgayMuon, 2, 2);
        ThemTruong(input, "Hạn trả:", dtpHanTra, 3, 0);

        var returnPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight
        };
        returnPanel.Controls.Add(chkDaTra);
        returnPanel.Controls.Add(dtpNgayTra);
        input.Controls.Add(TaoLabel("Ngày trả thực tế:"), 2, 3);
        input.Controls.Add(returnPanel, 3, 3);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 2, 0, 0)
        };
        var btnThem = TaoButton("Thêm", Color.SeaGreen);
        var btnSua = TaoButton("Cập nhật", Color.RoyalBlue);
        var btnXoa = TaoButton("Xóa", Color.IndianRed);
        var btnTra = TaoButton("Đánh dấu đã trả", Color.Teal, 135);
        var btnMoi = TaoButton("Làm mới", Color.DimGray);
        btnThem.Click += (_, _) => ThemPhieu();
        btnSua.Click += (_, _) => CapNhatPhieu();
        btnXoa.Click += (_, _) => XoaPhieu();
        btnTra.Click += (_, _) => DanhDauDaTra();
        btnMoi.Click += (_, _) => LamMoi();
        buttons.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnTra, btnMoi });
        input.Controls.Add(buttons, 0, 4);
        input.SetColumnSpan(buttons, 4);

        var filter = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            Padding = new Padding(2, 10, 2, 2),
            FlowDirection = FlowDirection.LeftToRight
        };
        main.Controls.Add(filter, 0, 1);

        txtTimKiem.Width = 280;
        txtTimKiem.PlaceholderText = "Tìm theo mã sinh viên hoặc tên thiết bị...";
        cboLocTrangThai.Width = 180;
        cboLocTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLocTrangThai.Items.AddRange(new object[]
        {
            "Tất cả phiếu", "Đang mượn", "Quá hạn", "Đã trả"
        });
        cboLocTrangThai.SelectedIndex = 0;
        filter.Controls.Add(TaoLabelInline("Tìm kiếm:"));
        filter.Controls.Add(txtTimKiem);
        filter.Controls.Add(TaoLabelInline("Trạng thái:"));
        filter.Controls.Add(cboLocTrangThai);

        var grpList = new GroupBox
        {
            Text = "DANH SÁCH PHIẾU MƯỢN / TRẢ",
            Dock = DockStyle.Fill,
            Padding = new Padding(8),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        main.Controls.Add(grpList, 0, 2);

        dgvPhieuMuon.Dock = DockStyle.Fill;
        dgvPhieuMuon.AutoGenerateColumns = false;
        dgvPhieuMuon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvPhieuMuon.MultiSelect = false;
        dgvPhieuMuon.ReadOnly = true;
        dgvPhieuMuon.AllowUserToAddRows = false;
        dgvPhieuMuon.AllowUserToDeleteRows = false;
        dgvPhieuMuon.RowHeadersVisible = false;
        dgvPhieuMuon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvPhieuMuon.BackgroundColor = Color.White;
        dgvPhieuMuon.BorderStyle = BorderStyle.FixedSingle;
        dgvPhieuMuon.EnableHeadersVisualStyles = false;
        dgvPhieuMuon.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 76, 115);
        dgvPhieuMuon.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvPhieuMuon.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        dgvPhieuMuon.RowTemplate.Height = 30;
        CauHinhCot();
        grpList.Controls.Add(dgvPhieuMuon);

        txtTimKiem.TextChanged += (_, _) => HienThiDanhSach();
        cboLocTrangThai.SelectedIndexChanged += (_, _) => HienThiDanhSach();
        dgvPhieuMuon.SelectionChanged += (_, _) => NapDuLieuLenForm();
        chkDaTra.CheckedChanged += (_, _) => dtpNgayTra.Enabled = chkDaTra.Checked;
    }

    private void CauHinhInput()
    {
        foreach (var txt in new[] { txtMaPhieu, txtMaSV, txtHoTen })
            txt.Dock = DockStyle.Fill;

        cboThietBi.Dock = DockStyle.Fill;
        cboThietBi.DropDownStyle = ComboBoxStyle.DropDownList;
        cboThietBi.Items.AddRange(new object[]
        {
            "Laptop", "Máy chiếu", "Máy tính để bàn",
            "Bộ Arduino", "Kit Raspberry Pi", "Thiết bị mạng",
            "Bộ thực hành điện", "Máy đo đa năng"
        });
        cboThietBi.SelectedIndex = 0;

        nudSoLuong.Dock = DockStyle.Left;
        nudSoLuong.Minimum = 1;
        nudSoLuong.Maximum = 1000;
        nudSoLuong.Value = 1;
        nudSoLuong.Width = 120;

        CauHinhDatePicker(dtpNgayMuon);
        CauHinhDatePicker(dtpHanTra);
        CauHinhDatePicker(dtpNgayTra);
        dtpNgayMuon.Value = DateTime.Today;
        dtpHanTra.Value = DateTime.Today.AddDays(7);
        dtpNgayTra.Value = DateTime.Today;
        dtpNgayTra.Enabled = false;

        chkDaTra.Text = "Đã trả";
        chkDaTra.AutoSize = true;
        chkDaTra.Padding = new Padding(0, 7, 8, 0);
    }

    private static void CauHinhDatePicker(DateTimePicker picker)
    {
        picker.Dock = DockStyle.Fill;
        picker.Format = DateTimePickerFormat.Custom;
        picker.CustomFormat = "dd/MM/yyyy";
    }

    private void CauHinhCot()
    {
        dgvPhieuMuon.Columns.Clear();
        ThemCot(nameof(PhieuMuon.MaPhieu), "Mã phiếu", 10);
        ThemCot(nameof(PhieuMuon.MaSinhVien), "Mã sinh viên", 11);
        ThemCot(nameof(PhieuMuon.HoTenSinhVien), "Họ tên sinh viên", 15);
        ThemCot(nameof(PhieuMuon.ThietBi), "Thiết bị", 15);
        ThemCot(nameof(PhieuMuon.SoLuong), "Số lượng", 7);
        ThemCot(nameof(PhieuMuon.NgayMuon), "Ngày mượn", 10);
        ThemCot(nameof(PhieuMuon.HanTra), "Hạn trả", 10);
        ThemCot(nameof(PhieuMuon.NgayTraThucTe), "Ngày trả thực tế", 12);
        ThemCot(nameof(PhieuMuon.TrangThai), "Tình trạng", 10);
        dgvPhieuMuon.Columns[nameof(PhieuMuon.NgayMuon)]!.DefaultCellStyle.Format = "dd/MM/yyyy";
        dgvPhieuMuon.Columns[nameof(PhieuMuon.HanTra)]!.DefaultCellStyle.Format = "dd/MM/yyyy";
        dgvPhieuMuon.Columns[nameof(PhieuMuon.NgayTraThucTe)]!.DefaultCellStyle.Format = "dd/MM/yyyy";
    }

    private void ThemCot(string property, string header, float weight)
    {
        dgvPhieuMuon.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = property,
            DataPropertyName = property,
            HeaderText = header,
            FillWeight = weight,
            SortMode = DataGridViewColumnSortMode.Automatic,
            DefaultCellStyle = property == nameof(PhieuMuon.NgayTraThucTe)
                ? new DataGridViewCellStyle { NullValue = "—" }
                : new DataGridViewCellStyle()
        });
    }

    private void KhoiTaoDuLieu()
    {
        danhSach.Add(new PhieuMuon
        {
            MaPhieu = "PM001", MaSinhVien = "D19CNPM001",
            HoTenSinhVien = "Nguyễn Văn Nam", ThietBi = "Laptop",
            SoLuong = 1, NgayMuon = DateTime.Today.AddDays(-2),
            HanTra = DateTime.Today.AddDays(5)
        });
        danhSach.Add(new PhieuMuon
        {
            MaPhieu = "PM002", MaSinhVien = "D19CNPM002",
            HoTenSinhVien = "Trần Thị Lan", ThietBi = "Máy chiếu",
            SoLuong = 1, NgayMuon = DateTime.Today.AddDays(-10),
            HanTra = DateTime.Today.AddDays(-2)
        });
        danhSach.Add(new PhieuMuon
        {
            MaPhieu = "PM003", MaSinhVien = "D19CNPM003",
            HoTenSinhVien = "Lê Minh Đức", ThietBi = "Bộ Arduino",
            SoLuong = 2, NgayMuon = DateTime.Today.AddDays(-7),
            HanTra = DateTime.Today.AddDays(-1),
            NgayTraThucTe = DateTime.Today
        });
    }

    private void HienThiDanhSach()
    {
        IEnumerable<PhieuMuon> ketQua = danhSach;
        string tuKhoa = txtTimKiem.Text.Trim();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            ketQua = ketQua.Where(x =>
                x.MaSinhVien.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ||
                x.ThietBi.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase));
        }

        string trangThai = cboLocTrangThai.Text;
        if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả phiếu")
            ketQua = ketQua.Where(x => x.TrangThai == trangThai);

        var ketQuaList = ketQua.ToList();
        bindingSource.DataSource = new BindingList<PhieuMuon>(ketQuaList);
        dgvPhieuMuon.DataSource = bindingSource;

        lblTongPhieu.Text = $"Tổng số phiếu: {danhSach.Count}";
        lblQuaHan.Text = $"Số phiếu quá hạn: {danhSach.Count(x => x.TrangThai == "Quá hạn")}";
    }

    private bool KiemTraDuLieu()
    {
        errorProvider.Clear();
        bool hopLe = true;
        string ma = txtMaPhieu.Text.Trim();

        if (string.IsNullOrWhiteSpace(ma))
        {
            errorProvider.SetError(txtMaPhieu, "Mã phiếu không được để trống.");
            hopLe = false;
        }
        else if (danhSach.Any(x =>
            x.MaPhieu.Equals(ma, StringComparison.OrdinalIgnoreCase) &&
            !ReferenceEquals(x, phieuDangChon)))
        {
            errorProvider.SetError(txtMaPhieu, "Mã phiếu đã tồn tại.");
            hopLe = false;
        }

        if (string.IsNullOrWhiteSpace(txtMaSV.Text))
        {
            errorProvider.SetError(txtMaSV, "Mã sinh viên không được để trống.");
            hopLe = false;
        }

        if (string.IsNullOrWhiteSpace(txtHoTen.Text))
        {
            errorProvider.SetError(txtHoTen, "Họ tên sinh viên không được để trống.");
            hopLe = false;
        }

        if (string.IsNullOrWhiteSpace(cboThietBi.Text))
        {
            errorProvider.SetError(cboThietBi, "Vui lòng chọn thiết bị.");
            hopLe = false;
        }

        if (nudSoLuong.Value <= 0)
        {
            errorProvider.SetError(nudSoLuong, "Số lượng phải lớn hơn 0.");
            hopLe = false;
        }

        if (dtpHanTra.Value.Date < dtpNgayMuon.Value.Date)
        {
            errorProvider.SetError(dtpHanTra, "Hạn trả không được trước ngày mượn.");
            hopLe = false;
        }

        if (chkDaTra.Checked && dtpNgayTra.Value.Date < dtpNgayMuon.Value.Date)
        {
            errorProvider.SetError(dtpNgayTra, "Ngày trả thực tế không được trước ngày mượn.");
            hopLe = false;
        }

        return hopLe;
    }

    private PhieuMuon LayDuLieu() => new()
    {
        MaPhieu = txtMaPhieu.Text.Trim(),
        MaSinhVien = txtMaSV.Text.Trim(),
        HoTenSinhVien = txtHoTen.Text.Trim(),
        ThietBi = cboThietBi.Text,
        SoLuong = (int)nudSoLuong.Value,
        NgayMuon = dtpNgayMuon.Value.Date,
        HanTra = dtpHanTra.Value.Date,
        NgayTraThucTe = chkDaTra.Checked ? dtpNgayTra.Value.Date : null
    };

    private void ThemPhieu()
    {
        phieuDangChon = null;
        if (!KiemTraDuLieu()) return;

        danhSach.Add(LayDuLieu());
        LamMoi();
        HienThiDanhSach();
        MessageBox.Show("Thêm phiếu mượn thành công!", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void CapNhatPhieu()
    {
        if (phieuDangChon == null)
        {
            MessageBox.Show("Vui lòng chọn phiếu cần cập nhật.");
            return;
        }

        if (!KiemTraDuLieu()) return;

        PhieuMuon data = LayDuLieu();
        phieuDangChon.MaPhieu = data.MaPhieu;
        phieuDangChon.MaSinhVien = data.MaSinhVien;
        phieuDangChon.HoTenSinhVien = data.HoTenSinhVien;
        phieuDangChon.ThietBi = data.ThietBi;
        phieuDangChon.SoLuong = data.SoLuong;
        phieuDangChon.NgayMuon = data.NgayMuon;
        phieuDangChon.HanTra = data.HanTra;
        phieuDangChon.NgayTraThucTe = data.NgayTraThucTe;

        LamMoi();
        HienThiDanhSach();
        MessageBox.Show("Cập nhật phiếu mượn thành công!", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void XoaPhieu()
    {
        if (phieuDangChon == null)
        {
            MessageBox.Show("Vui lòng chọn phiếu cần xóa.");
            return;
        }

        var result = MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa phiếu {phieuDangChon.MaPhieu}?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            danhSach.Remove(phieuDangChon);
            LamMoi();
            HienThiDanhSach();
            MessageBox.Show("Đã xóa phiếu mượn.");
        }
    }

    private void DanhDauDaTra()
    {
        if (phieuDangChon == null)
        {
            MessageBox.Show("Vui lòng chọn phiếu cần đánh dấu đã trả.");
            return;
        }

        if (phieuDangChon.NgayTraThucTe.HasValue)
        {
            MessageBox.Show("Phiếu này đã được trả trước đó.");
            return;
        }

        DateTime ngayTra = DateTime.Today;
        if (ngayTra < phieuDangChon.NgayMuon.Date)
        {
            MessageBox.Show("Ngày trả thực tế không được trước ngày mượn.");
            return;
        }

        phieuDangChon.NgayTraThucTe = ngayTra;
        HienThiDanhSach();
        MessageBox.Show("Đã cập nhật ngày trả và trạng thái phiếu.", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void NapDuLieuLenForm()
    {
        if (dgvPhieuMuon.CurrentRow?.DataBoundItem is not PhieuMuon item)
            return;

        phieuDangChon = item;
        txtMaPhieu.Text = item.MaPhieu;
        txtMaSV.Text = item.MaSinhVien;
        txtHoTen.Text = item.HoTenSinhVien;
        cboThietBi.SelectedItem = item.ThietBi;
        nudSoLuong.Value = Math.Clamp(item.SoLuong, (int)nudSoLuong.Minimum, (int)nudSoLuong.Maximum);
        dtpNgayMuon.Value = item.NgayMuon;
        dtpHanTra.Value = item.HanTra;
        chkDaTra.Checked = item.NgayTraThucTe.HasValue;
        dtpNgayTra.Value = item.NgayTraThucTe ?? DateTime.Today;
        dtpNgayTra.Enabled = chkDaTra.Checked;
        errorProvider.Clear();
    }

    private void LamMoi()
    {
        phieuDangChon = null;
        txtMaPhieu.Clear();
        txtMaSV.Clear();
        txtHoTen.Clear();
        cboThietBi.SelectedIndex = 0;
        nudSoLuong.Value = 1;
        dtpNgayMuon.Value = DateTime.Today;
        dtpHanTra.Value = DateTime.Today.AddDays(7);
        dtpNgayTra.Value = DateTime.Today;
        chkDaTra.Checked = false;
        dtpNgayTra.Enabled = false;
        errorProvider.Clear();
        dgvPhieuMuon.ClearSelection();
    }

    private void XuatCSV()
    {
        using var save = new SaveFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv",
            DefaultExt = "csv",
            AddExtension = true,
            FileName = "DanhSachPhieuMuon.csv",
            Title = "Xuất danh sách phiếu mượn"
        };

        if (save.ShowDialog() != DialogResult.OK) return;

        try
        {
            using var writer = new StreamWriter(save.FileName, false, new UTF8Encoding(true));
            writer.WriteLine("MaPhieu,MaSinhVien,HoTenSinhVien,ThietBi,SoLuong,NgayMuon,HanTra,NgayTraThucTe,TrangThai");

            foreach (DataGridViewRow row in dgvPhieuMuon.Rows)
            {
                if (row.DataBoundItem is not PhieuMuon item) continue;

                string[] values =
                {
                    item.MaPhieu,
                    item.MaSinhVien,
                    item.HoTenSinhVien,
                    item.ThietBi,
                    item.SoLuong.ToString(CultureInfo.InvariantCulture),
                    item.NgayMuon.ToString("dd/MM/yyyy"),
                    item.HanTra.ToString("dd/MM/yyyy"),
                    item.NgayTraThucTe?.ToString("dd/MM/yyyy") ?? "",
                    item.TrangThai
                };
                writer.WriteLine(string.Join(",", values.Select(EscapeCsv)));
            }

            MessageBox.Show("Xuất CSV thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Không thể xuất file: " + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string EscapeCsv(string value) =>
        "\"" + value.Replace("\"", "\"\"") + "\"";
}
