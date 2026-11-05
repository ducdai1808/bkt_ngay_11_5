using System.ComponentModel;

namespace LabEquipmentCheckout;

public class PhieuMuon : INotifyPropertyChanged
{
    private string maPhieu = "";
    private string maSinhVien = "";
    private string hoTenSinhVien = "";
    private string thietBi = "";
    private int soLuong = 1;
    private DateTime ngayMuon = DateTime.Today;
    private DateTime hanTra = DateTime.Today.AddDays(7);
    private DateTime? ngayTraThucTe;

    public string MaPhieu
    {
        get => maPhieu;
        set { if (maPhieu != value) { maPhieu = value; OnPropertyChanged(nameof(MaPhieu)); } }
    }

    public string MaSinhVien
    {
        get => maSinhVien;
        set { if (maSinhVien != value) { maSinhVien = value; OnPropertyChanged(nameof(MaSinhVien)); } }
    }

    public string HoTenSinhVien
    {
        get => hoTenSinhVien;
        set { if (hoTenSinhVien != value) { hoTenSinhVien = value; OnPropertyChanged(nameof(HoTenSinhVien)); } }
    }

    public string ThietBi
    {
        get => thietBi;
        set { if (thietBi != value) { thietBi = value; OnPropertyChanged(nameof(ThietBi)); } }
    }

    public int SoLuong
    {
        get => soLuong;
        set { if (soLuong != value) { soLuong = value; OnPropertyChanged(nameof(SoLuong)); } }
    }

    public DateTime NgayMuon
    {
        get => ngayMuon;
        set { if (ngayMuon != value) { ngayMuon = value; OnPropertyChanged(nameof(NgayMuon)); } }
    }

    public DateTime HanTra
    {
        get => hanTra;
        set
        {
            if (hanTra != value)
            {
                hanTra = value;
                OnPropertyChanged(nameof(HanTra));
                OnPropertyChanged(nameof(TrangThai));
            }
        }
    }

    public DateTime? NgayTraThucTe
    {
        get => ngayTraThucTe;
        set
        {
            if (ngayTraThucTe != value)
            {
                ngayTraThucTe = value;
                OnPropertyChanged(nameof(NgayTraThucTe));
                OnPropertyChanged(nameof(TrangThai));
            }
        }
    }

    // Trang thai duoc tinh tu han tra va ngay tra thuc te.
    public string TrangThai =>
        NgayTraThucTe.HasValue
            ? "Đã trả"
            : HanTra.Date < DateTime.Today
                ? "Quá hạn"
                : "Đang mượn";

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
