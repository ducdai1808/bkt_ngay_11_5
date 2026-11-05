# Clinic Appointment Manager

Ứng dụng Windows Forms quản lý lịch hẹn phòng khám bằng C#.

## Công nghệ
- C# / Windows Forms
- .NET 8.0
- BindingList<T>, BindingSource
- DataGridView cấu hình cột thủ công
- ErrorProvider
- MenuStrip, StatusStrip
- SaveFileDialog xuất CSV

## Chức năng
- Thêm, cập nhật, xóa lịch hẹn (có xác nhận khi xóa).
- Tìm kiếm tức thời theo tên bệnh nhân hoặc số điện thoại.
- Lọc theo ngày hẹn và trạng thái.
- Kiểm tra mã lịch hẹn duy nhất, họ tên, số điện thoại 10 chữ số, lý do khám và ngày hẹn.
- Xuất danh sách đang hiển thị ra CSV.
- Hiển thị số lịch hẹn sau khi lọc.

## Cách chạy
1. Cài Visual Studio 2022 và workload **.NET desktop development**.
2. Mở `ClinicAppointmentManager.csproj`.
3. Chọn framework .NET 8.0.
4. Nhấn `F5` hoặc `Ctrl + F5`.

## Ghi chú
Dữ liệu mẫu được khởi tạo khi chạy và lưu trong bộ nhớ. Đóng ứng dụng sẽ không giữ lại các thay đổi.
