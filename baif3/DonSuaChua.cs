using System.ComponentModel;

namespace RepairServiceTracker;

public class DonSuaChua : INotifyPropertyChanged
{
    private string _maDon = "";
    private string _tenKhachHang = "";
    private string _soDienThoai = "";
    private string _loaiThietBi = "";
    private string _moTaLoi = "";
    private DateTime _ngayTiepNhan = DateTime.Today;
    private decimal _chiPhiDuKien;
    private string _trangThai = "Mới tiếp nhận";
    private string _duongDanAnh = "";

    public string MaDon { get => _maDon; set => Set(ref _maDon, value); }
    public string TenKhachHang { get => _tenKhachHang; set => Set(ref _tenKhachHang, value); }
    public string SoDienThoai { get => _soDienThoai; set => Set(ref _soDienThoai, value); }
    public string LoaiThietBi { get => _loaiThietBi; set => Set(ref _loaiThietBi, value); }
    public string MoTaLoi { get => _moTaLoi; set => Set(ref _moTaLoi, value); }
    public DateTime NgayTiepNhan { get => _ngayTiepNhan; set => Set(ref _ngayTiepNhan, value); }
    public decimal ChiPhiDuKien { get => _chiPhiDuKien; set => Set(ref _chiPhiDuKien, value); }
    public string TrangThai { get => _trangThai; set => Set(ref _trangThai, value); }
    public string DuongDanAnh { get => _duongDanAnh; set => Set(ref _duongDanAnh, value); }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Set<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
