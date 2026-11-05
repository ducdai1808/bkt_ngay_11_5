using System.ComponentModel;

namespace ClinicAppointmentManager;

public class LichHen : INotifyPropertyChanged
{
    private string maLichHen = "";
    private string hoTen = "";
    private string soDienThoai = "";
    private DateTime ngayGioHen = DateTime.Now;
    private string bacSi = "";
    private string lyDoKham = "";
    private string trangThai = "Chờ khám";

    public string MaLichHen
    {
        get => maLichHen;
        set { if (maLichHen != value) { maLichHen = value; OnPropertyChanged(nameof(MaLichHen)); } }
    }

    public string HoTen
    {
        get => hoTen;
        set { if (hoTen != value) { hoTen = value; OnPropertyChanged(nameof(HoTen)); } }
    }

    public string SoDienThoai
    {
        get => soDienThoai;
        set { if (soDienThoai != value) { soDienThoai = value; OnPropertyChanged(nameof(SoDienThoai)); } }
    }

    public DateTime NgayGioHen
    {
        get => ngayGioHen;
        set { if (ngayGioHen != value) { ngayGioHen = value; OnPropertyChanged(nameof(NgayGioHen)); } }
    }

    public string BacSi
    {
        get => bacSi;
        set { if (bacSi != value) { bacSi = value; OnPropertyChanged(nameof(BacSi)); } }
    }

    public string LyDoKham
    {
        get => lyDoKham;
        set { if (lyDoKham != value) { lyDoKham = value; OnPropertyChanged(nameof(LyDoKham)); } }
    }

    public string TrangThai
    {
        get => trangThai;
        set { if (trangThai != value) { trangThai = value; OnPropertyChanged(nameof(TrangThai)); } }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
