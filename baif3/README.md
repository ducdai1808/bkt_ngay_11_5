# Repair Service Tracker

Ứng dụng Windows Forms quản lý đơn đăng ký dịch vụ sửa chữa thiết bị.

## Công nghệ
- C# / Windows Forms
- .NET 8 (`net8.0-windows`)
- `BindingList<T>`, `BindingSource`, `DataGridView`
- `ErrorProvider`, `OpenFileDialog`, `SaveFileDialog`

## Chạy chương trình
1. Giải nén thư mục.
2. Mở `RepairServiceTracker.csproj` bằng Visual Studio 2022 (cài workload .NET desktop development).
3. Chọn chạy project hoặc nhấn `Ctrl + F5`.

## Chức năng
- Thêm, cập nhật, xóa đơn sửa chữa (có xác nhận xóa).
- Tìm kiếm theo mã đơn, tên khách hàng, số điện thoại.
- Lọc theo trạng thái.
- Chọn ảnh thiết bị và hiển thị bằng `PictureBoxSizeMode.Zoom`.
- Kiểm tra mã đơn trùng, tên, số điện thoại, mô tả lỗi và chi phí không âm.
- Xuất các đơn đang hiển thị ra CSV.
- StatusStrip hiển thị số đơn theo bộ lọc và tổng chi phí các đơn chưa hoàn tất.

## Lưu ý
Dữ liệu hiện được lưu trong bộ nhớ khi ứng dụng chạy; chưa kết nối cơ sở dữ liệu hoặc lưu lâu dài sau khi tắt ứng dụng.
