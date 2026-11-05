using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ClinicAppointmentManager;

public class Form1 : Form
{
    private readonly BindingList<LichHen> danhSach = new();
    private readonly BindingSource bindingSource = new();
    private readonly ErrorProvider errorProvider = new();

    private readonly TextBox txtMa = new();
    private readonly TextBox txtHoTen = new();
    private readonly TextBox txtSDT = new();
    private readonly TextBox txtLyDo = new();
    private readonly DateTimePicker dtpNgayHen = new();
    private readonly ComboBox cboBacSi = new();
    private readonly ComboBox cboTrangThai = new();

    private readonly TextBox txtTimKiem = new();
    private readonly ComboBox cboLocTrangThai = new();
    private readonly DateTimePicker dtpLocNgay = new();
    private readonly CheckBox chkLocNgay = new();

    private readonly DataGridView dgvLichHen = new();
    private readonly ToolStripStatusLabel lblSoLuong = new();
    private LichHen? lichHenDangChon;

    public Form1()
    {
        Text = "Clinic Appointment Manager";
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
        var mnuExport = new ToolStripMenuItem("Xuất danh sách đang hiển thị ra CSV");
        var mnuExit = new ToolStripMenuItem("Thoát");
        mnuExport.Click += (_, _) => XuatCSV();
        mnuExit.Click += (_, _) => Close();
        mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuExport, new ToolStripSeparator(), mnuExit });
        menu.Items.Add(mnuFile);
        MainMenuStrip = menu;
        Controls.Add(menu);

        var status = new StatusStrip();
        lblSoLuong.Text = "Số lịch hẹn đang hiển thị: 0";
        status.Items.Add(lblSoLuong);
        Controls.Add(status);

        var main = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(12),
            BackColor = BackColor
        };
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 245));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(main);
        main.BringToFront();

        var grpInput = new GroupBox
        {
            Text = "THÔNG TIN LỊCH HẸN",
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
        ThemTruong(input, "Mã lịch hẹn:", txtMa, 0, 0);
        ThemTruong(input, "Họ tên bệnh nhân:", txtHoTen, 0, 2);
        ThemTruong(input, "Số điện thoại:", txtSDT, 1, 0);
        ThemTruong(input, "Ngày giờ hẹn:", dtpNgayHen, 1, 2);
        ThemTruong(input, "Bác sĩ:", cboBacSi, 2, 0);
        ThemTruong(input, "Trạng thái:", cboTrangThai, 2, 2);
        input.Controls.Add(TaoLabel("Lý do khám:"), 0, 3);
        input.Controls.Add(txtLyDo, 1, 3);
        input.SetColumnSpan(txtLyDo, 3);

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
        var btnMoi = TaoButton("Làm mới", Color.DimGray);
        btnThem.Click += (_, _) => ThemLichHen();
        btnSua.Click += (_, _) => CapNhatLichHen();
        btnXoa.Click += (_, _) => XoaLichHen();
        btnMoi.Click += (_, _) => LamMoi();
        buttons.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnMoi });
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
        txtTimKiem.Width = 245;
        txtTimKiem.PlaceholderText = "Tìm theo tên hoặc số điện thoại...";
        cboLocTrangThai.Width = 155;
        cboLocTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLocTrangThai.Items.AddRange(new object[] { "Tất cả trạng thái", "Chờ khám", "Đang khám", "Hoàn tất", "Đã hủy" });
        cboLocTrangThai.SelectedIndex = 0;
        chkLocNgay.Text = "Lọc theo ngày";
        chkLocNgay.AutoSize = true;
        chkLocNgay.Padding = new Padding(4, 7, 0, 0);
        dtpLocNgay.Width = 130;
        dtpLocNgay.Format = DateTimePickerFormat.Short;
        dtpLocNgay.Enabled = false;
        var btnLoc = TaoButton("Lọc", Color.Teal);
        filter.Controls.AddRange(new Control[]
        {
            TaoLabelInline("Tìm kiếm:"), txtTimKiem, chkLocNgay, dtpLocNgay,
            cboLocTrangThai, btnLoc
        });

        var grpList = new GroupBox
        {
            Text = "DANH SÁCH LỊCH HẸN",
            Dock = DockStyle.Fill,
            Padding = new Padding(8),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        main.Controls.Add(grpList, 0, 2);

        dgvLichHen.Dock = DockStyle.Fill;
        dgvLichHen.AutoGenerateColumns = false;
        dgvLichHen.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvLichHen.MultiSelect = false;
        dgvLichHen.ReadOnly = true;
        dgvLichHen.AllowUserToAddRows = false;
        dgvLichHen.AllowUserToDeleteRows = false;
        dgvLichHen.RowHeadersVisible = false;
        dgvLichHen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvLichHen.BackgroundColor = Color.White;
        dgvLichHen.BorderStyle = BorderStyle.FixedSingle;
        dgvLichHen.EnableHeadersVisualStyles = false;
        dgvLichHen.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 76, 115);
        dgvLichHen.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvLichHen.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        dgvLichHen.RowTemplate.Height = 30;
        CauHinhCot();
        grpList.Controls.Add(dgvLichHen);

        txtTimKiem.TextChanged += (_, _) => HienThiDanhSach();
        cboLocTrangThai.SelectedIndexChanged += (_, _) => HienThiDanhSach();
        chkLocNgay.CheckedChanged += (_, _) =>
        {
            dtpLocNgay.Enabled = chkLocNgay.Checked;
            HienThiDanhSach();
        };
        dtpLocNgay.ValueChanged += (_, _) => HienThiDanhSach();
        btnLoc.Click += (_, _) => HienThiDanhSach();
        dgvLichHen.SelectionChanged += (_, _) => NapDuLieuLenForm();
    }

    private void CauHinhInput()
    {
        foreach (var txt in new[] { txtMa, txtHoTen, txtSDT, txtLyDo })
            txt.Dock = DockStyle.Fill;

        txtLyDo.Multiline = true;
        txtLyDo.ScrollBars = ScrollBars.Vertical;

        dtpNgayHen.Dock = DockStyle.Fill;
        dtpNgayHen.Format = DateTimePickerFormat.Custom;
        dtpNgayHen.CustomFormat = "dd/MM/yyyy HH:mm";
        dtpNgayHen.ShowUpDown = true;
        dtpNgayHen.Value = DateTime.Now.AddHours(1);

        cboBacSi.Dock = DockStyle.Fill;
        cboBacSi.DropDownStyle = ComboBoxStyle.DropDownList;
        cboBacSi.Items.AddRange(new object[]
        {
            "BS. Nguyễn Văn An", "BS. Trần Thị Hoa",
            "BS. Lê Minh Tuấn", "BS. Phạm Thu Hà"
        });
        cboBacSi.SelectedIndex = 0;

        cboTrangThai.Dock = DockStyle.Fill;
        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.Items.AddRange(new object[]
        {
            "Chờ khám", "Đang khám", "Hoàn tất", "Đã hủy"
        });
        cboTrangThai.SelectedIndex = 0;
    }

    private static Label TaoLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleLeft,
        Font = new Font("Segoe UI", 9, FontStyle.Bold)
    };

    private static Label TaoLabelInline(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Padding = new Padding(0, 8, 0, 0),
        Font = new Font("Segoe UI", 9, FontStyle.Bold)
    };

    private static Button TaoButton(string text, Color color) => new()
    {
        Text = text,
        Width = 105,
        Height = 32,
        BackColor = color,
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat,
        Margin = new Padding(4, 2, 4, 2),
        Cursor = Cursors.Hand
    };

    private static void ThemTruong(TableLayoutPanel table, string ten, Control control, int row, int col)
    {
        table.Controls.Add(TaoLabel(ten), col, row);
        table.Controls.Add(control, col + 1, row);
    }

    private void CauHinhCot()
    {
        dgvLichHen.Columns.Clear();
        ThemCot("MaLichHen", "Mã lịch hẹn", 12);
        ThemCot("HoTen", "Họ tên bệnh nhân", 18);
        ThemCot("SoDienThoai", "Số điện thoại", 14);
        ThemCot("NgayGioHen", "Ngày giờ hẹn", 16);
        ThemCot("BacSi", "Bác sĩ", 15);
        ThemCot("LyDoKham", "Lý do khám", 15);
        ThemCot("TrangThai", "Trạng thái", 12);
        dgvLichHen.Columns[nameof(LichHen.NgayGioHen)]!.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
    }

    private void ThemCot(string property, string header, float fillWeight)
    {
        dgvLichHen.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = property,
            DataPropertyName = property,
            HeaderText = header,
            FillWeight = fillWeight,
            SortMode = DataGridViewColumnSortMode.Automatic
        });
    }

    private void KhoiTaoDuLieu()
    {
        danhSach.Add(new LichHen
        {
            MaLichHen = "LH001", HoTen = "Nguyễn Văn Nam",
            SoDienThoai = "0912345678", NgayGioHen = DateTime.Now.AddDays(1),
            BacSi = "BS. Nguyễn Văn An", LyDoKham = "Khám tổng quát", TrangThai = "Chờ khám"
        });
        danhSach.Add(new LichHen
        {
            MaLichHen = "LH002", HoTen = "Trần Thị Lan",
            SoDienThoai = "0987654321", NgayGioHen = DateTime.Now.AddDays(2),
            BacSi = "BS. Trần Thị Hoa", LyDoKham = "Đau đầu", TrangThai = "Đang khám"
        });
        danhSach.Add(new LichHen
        {
            MaLichHen = "LH003", HoTen = "Lê Minh Đức",
            SoDienThoai = "0901234567", NgayGioHen = DateTime.Now.AddDays(3),
            BacSi = "BS. Lê Minh Tuấn", LyDoKham = "Kiểm tra sức khỏe", TrangThai = "Hoàn tất"
        });
    }

    private void HienThiDanhSach()
    {
        IEnumerable<LichHen> ketQua = danhSach;

        string tuKhoa = txtTimKiem.Text.Trim();
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            ketQua = ketQua.Where(x =>
                x.HoTen.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ||
                x.SoDienThoai.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase));
        }

        if (chkLocNgay.Checked)
        {
            DateTime ngay = dtpLocNgay.Value.Date;
            ketQua = ketQua.Where(x => x.NgayGioHen.Date == ngay);
        }

        string trangThai = cboLocTrangThai.Text;
        if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả trạng thái")
            ketQua = ketQua.Where(x => x.TrangThai == trangThai);

        var ketQuaList = ketQua.ToList();
        bindingSource.DataSource = new BindingList<LichHen>(ketQuaList);
        dgvLichHen.DataSource = bindingSource;
        lblSoLuong.Text = $"Số lịch hẹn đang hiển thị: {ketQuaList.Count}";
    }

    private bool KiemTraDuLieu()
    {
        errorProvider.Clear();
        bool hopLe = true;

        string ma = txtMa.Text.Trim();
        if (string.IsNullOrWhiteSpace(ma))
        {
            errorProvider.SetError(txtMa, "Mã lịch hẹn không được để trống.");
            hopLe = false;
        }
        else if (danhSach.Any(x =>
            x.MaLichHen.Equals(ma, StringComparison.OrdinalIgnoreCase) &&
            !ReferenceEquals(x, lichHenDangChon)))
        {
            errorProvider.SetError(txtMa, "Mã lịch hẹn đã tồn tại.");
            hopLe = false;
        }

        if (string.IsNullOrWhiteSpace(txtHoTen.Text))
        {
            errorProvider.SetError(txtHoTen, "Họ tên không được để trống.");
            hopLe = false;
        }

        if (!Regex.IsMatch(txtSDT.Text.Trim(), @"^0\d{9}$"))
        {
            errorProvider.SetError(txtSDT, "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 0.");
            hopLe = false;
        }

        if (string.IsNullOrWhiteSpace(txtLyDo.Text))
        {
            errorProvider.SetError(txtLyDo, "Lý do khám không được để trống.");
            hopLe = false;
        }

        if (dtpNgayHen.Value < DateTime.Now)
        {
            errorProvider.SetError(dtpNgayHen, "Ngày giờ hẹn không được nằm trong quá khứ.");
            hopLe = false;
        }

        return hopLe;
    }

    private LichHen LayDuLieu() => new()
    {
        MaLichHen = txtMa.Text.Trim(),
        HoTen = txtHoTen.Text.Trim(),
        SoDienThoai = txtSDT.Text.Trim(),
        NgayGioHen = dtpNgayHen.Value,
        BacSi = cboBacSi.Text,
        LyDoKham = txtLyDo.Text.Trim(),
        TrangThai = cboTrangThai.Text
    };

    private void ThemLichHen()
    {
        lichHenDangChon = null;
        if (!KiemTraDuLieu()) return;

        danhSach.Add(LayDuLieu());
        MessageBox.Show("Thêm lịch hẹn thành công!", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        LamMoi();
        HienThiDanhSach();
    }

    private void CapNhatLichHen()
    {
        if (lichHenDangChon == null)
        {
            MessageBox.Show("Vui lòng chọn lịch hẹn cần cập nhật.");
            return;
        }

        if (!KiemTraDuLieu()) return;

        LichHen duLieu = LayDuLieu();
        lichHenDangChon.MaLichHen = duLieu.MaLichHen;
        lichHenDangChon.HoTen = duLieu.HoTen;
        lichHenDangChon.SoDienThoai = duLieu.SoDienThoai;
        lichHenDangChon.NgayGioHen = duLieu.NgayGioHen;
        lichHenDangChon.BacSi = duLieu.BacSi;
        lichHenDangChon.LyDoKham = duLieu.LyDoKham;
        lichHenDangChon.TrangThai = duLieu.TrangThai;

        MessageBox.Show("Cập nhật lịch hẹn thành công!", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        LamMoi();
        HienThiDanhSach();
    }

    private void XoaLichHen()
    {
        if (lichHenDangChon == null)
        {
            MessageBox.Show("Vui lòng chọn lịch hẹn cần xóa.");
            return;
        }

        var xacNhan = MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa lịch hẹn {lichHenDangChon.MaLichHen}?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (xacNhan == DialogResult.Yes)
        {
            danhSach.Remove(lichHenDangChon);
            LamMoi();
            HienThiDanhSach();
            MessageBox.Show("Đã xóa lịch hẹn.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void NapDuLieuLenForm()
    {
        if (dgvLichHen.CurrentRow?.DataBoundItem is not LichHen item)
            return;

        lichHenDangChon = item;
        txtMa.Text = item.MaLichHen;
        txtHoTen.Text = item.HoTen;
        txtSDT.Text = item.SoDienThoai;
        dtpNgayHen.Value = item.NgayGioHen;
        cboBacSi.SelectedItem = item.BacSi;
        cboTrangThai.SelectedItem = item.TrangThai;
        txtLyDo.Text = item.LyDoKham;
        errorProvider.Clear();
    }

    private void LamMoi()
    {
        lichHenDangChon = null;
        txtMa.Clear();
        txtHoTen.Clear();
        txtSDT.Clear();
        txtLyDo.Clear();
        dtpNgayHen.Value = DateTime.Now.AddHours(1);
        cboBacSi.SelectedIndex = 0;
        cboTrangThai.SelectedIndex = 0;
        errorProvider.Clear();
        dgvLichHen.ClearSelection();
    }

    private void XuatCSV()
    {
        using var save = new SaveFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv",
            DefaultExt = "csv",
            AddExtension = true,
            FileName = "DanhSachLichHen.csv",
            Title = "Xuất danh sách lịch hẹn"
        };

        if (save.ShowDialog() != DialogResult.OK) return;

        try
        {
            using var writer = new StreamWriter(save.FileName, false, new UTF8Encoding(true));
            writer.WriteLine("MaLichHen,HoTen,SoDienThoai,NgayGioHen,BacSi,LyDoKham,TrangThai");

            foreach (DataGridViewRow row in dgvLichHen.Rows)
            {
                if (row.DataBoundItem is not LichHen item) continue;

                string[] values =
                {
                    item.MaLichHen,
                    item.HoTen,
                    item.SoDienThoai,
                    item.NgayGioHen.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    item.BacSi,
                    item.LyDoKham,
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
