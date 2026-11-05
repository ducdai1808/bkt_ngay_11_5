# Lab Equipment Checkout

Ứng dụng Windows Forms quản lý mượn/trả thiết bị tại phòng thực hành bằng C#.

## Công nghệ
- C# / Windows Forms
- .NET 8.0
- BindingList<T>, BindingSource
- DataGridView cấu hình cột thủ công
- ErrorProvider
- MenuStrip, StatusStrip
- SaveFileDialog xuất CSV

## Chức năng
- Thêm, cập nhật, xóa phiếu mượn (có xác nhận khi xóa).
- Chọn thiết bị, nhập số lượng, ngày mượn và hạn trả.
- Tìm kiếm theo mã sinh viên hoặc tên thiết bị.
- Lọc phiếu Đang mượn / Quá hạn / Đã trả.
- Tự động xác định phiếu quá hạn nếu chưa trả và hạn trả trước ngày hiện tại.
- Đánh dấu đã trả, tự cập nhật ngày trả thực tế.
- Kiểm tra mã phiếu duy nhất, mã sinh viên, thiết bị, số lượng và tính hợp lệ của ngày.
- Xuất các phiếu đang hiển thị ra CSV.
- StatusStrip hiển thị tổng số phiếu và số phiếu quá hạn.

## Cách chạy
1. Cài Visual Studio 2022 và workload **.NET desktop development**.
2. Mở `LabEquipmentCheckout.csproj`.
3. Chọn framework .NET 8.0.
4. Nhấn `F5` hoặc `Ctrl + F5`.

## Quy tắc trạng thái
- **Đã trả**: có ngày trả thực tế.
- **Quá hạn**: chưa có ngày trả thực tế và hạn trả nhỏ hơn ngày hiện tại.
- **Đang mượn**: chưa có ngày trả thực tế và chưa quá hạn.

## Ghi chú
Dữ liệu mẫu được khởi tạo khi chạy và lưu trong bộ nhớ. Đóng ứng dụng sẽ không giữ lại các thay đổi.
