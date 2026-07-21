# Backlog capstone Inventory - tuần 8

Tài liệu này là danh sách việc dự kiến cho mini project Inventory ở tuần 8. Mục tiêu là tận dụng lại phần đăng nhập và phân quyền của tuần 7, sau đó tập trung vào nghiệp vụ nhập, xuất và theo dõi tồn kho.

## Phạm vi bản đầu tiên

Bản đầu tiên chỉ cần quản lý một kho. Hàng hóa có thể dùng lại Product và Category đang có. Chưa làm nhiều chi nhánh, chuyển kho, kiểm kê bằng máy quét hoặc báo cáo tài chính.

Luồng chính cần chạy được:

1. Admin chuẩn bị tài khoản và dữ liệu Product/Category.
2. Staff tạo phiếu nhập hoặc phiếu xuất ở trạng thái nháp.
3. Phiếu được xác nhận thì số lượng tồn kho mới thay đổi.
4. User chỉ được xem danh sách hàng và số lượng tồn.
5. Mọi thay đổi tồn kho đều phải tra lại được từ lịch sử giao dịch.

## Phân quyền dự kiến

| Chức năng | User | Staff | Admin |
| --- | :---: | :---: | :---: |
| Xem Product, Category và tồn kho | Có | Có | Có |
| Tạo phiếu nhập/xuất nháp | Không | Có | Có |
| Sửa phiếu đang nháp | Không | Có | Có |
| Xác nhận phiếu nhập/xuất | Không | Có | Có |
| Hủy phiếu đã xác nhận | Không | Không | Có |
| Quản lý Product và Category | Không | Có | Có |
| Quản lý tài khoản và role | Không | Không | Có |
| Xem toàn bộ lịch sử giao dịch kho | Không | Có | Có |

## Danh sách việc theo thứ tự

### 1. Chuẩn bị domain và database

- [ ] Chốt tên entity và trạng thái của chứng từ kho.
- [ ] Tạo `InventoryTransaction` để lưu phiếu nhập hoặc xuất.
- [ ] Tạo `InventoryTransactionItem` để lưu Product, số lượng và đơn giá của từng dòng.
- [ ] Thêm số phiếu duy nhất, loại giao dịch, trạng thái, ghi chú và người tạo.
- [ ] Tạo migration và kiểm tra quan hệ trong PostgreSQL.

Kết quả mong đợi: database lưu được một phiếu có nhiều dòng hàng, chưa làm thay đổi tồn kho khi phiếu còn ở trạng thái nháp.

### 2. API xem tồn kho

- [ ] Tạo endpoint xem tồn kho hiện tại theo Product.
- [ ] Hỗ trợ tìm theo tên Product và lọc theo Category.
- [ ] Trả thêm cảnh báo khi số lượng thấp hơn ngưỡng tối thiểu.
- [ ] Giữ endpoint xem tồn kho cho cả ba role sau khi đăng nhập.

Kết quả mong đợi: người dùng biết trong kho còn bao nhiêu hàng mà không phải tự cộng các giao dịch.

### 3. Phiếu nhập kho

- [ ] Tạo phiếu nhập ở trạng thái nháp.
- [ ] Thêm, sửa và xóa dòng hàng khi phiếu còn nháp.
- [ ] Validate Product tồn tại, số lượng lớn hơn 0 và không có Product trùng trong cùng phiếu.
- [ ] Xác nhận phiếu nhập và cộng tồn kho trong một transaction database.
- [ ] Chặn xác nhận lại một phiếu đã hoàn tất.

Kết quả mong đợi: xác nhận phiếu nhập thành công thì toàn bộ dòng hàng được cộng tồn; nếu một dòng lỗi thì không dòng nào được cập nhật.

### 4. Phiếu xuất kho

- [ ] Tạo phiếu xuất ở trạng thái nháp.
- [ ] Thêm, sửa và xóa dòng hàng khi phiếu còn nháp.
- [ ] Kiểm tra tồn kho trước khi xác nhận.
- [ ] Xác nhận phiếu xuất và trừ tồn kho trong một transaction database.
- [ ] Không cho số lượng tồn xuống dưới 0.
- [ ] Chặn xác nhận lại một phiếu đã hoàn tất.

Kết quả mong đợi: API trả lỗi rõ ràng khi hàng không đủ và không trừ dở một phần phiếu.

### 5. Hủy và điều chỉnh giao dịch

- [ ] Admin được hủy phiếu đã xác nhận.
- [ ] Khi hủy, tạo giao dịch đảo ngược thay vì xóa lịch sử cũ.
- [ ] Bắt buộc nhập lý do hủy.
- [ ] Không cho hủy nếu thao tác đảo ngược làm tồn kho âm.

Kết quả mong đợi: số tồn được sửa đúng nhưng lịch sử ban đầu vẫn còn để kiểm tra.

### 6. Lịch sử và truy vết

- [ ] Xem danh sách phiếu theo loại, trạng thái, ngày tạo và người tạo.
- [ ] Xem chi tiết một phiếu cùng các dòng hàng.
- [ ] Xem lịch sử nhập/xuất của một Product.
- [ ] Lưu `CreatedByUserId`, thời gian xác nhận và người xác nhận.
- [ ] Phân trang các danh sách có thể tăng lớn.

Kết quả mong đợi: từ một Product có thể tìm ra số lượng đã thay đổi bởi phiếu nào, lúc nào và do ai thao tác.

### 7. Test và tài liệu

- [ ] Thêm Postman flow cho nhập kho thành công.
- [ ] Thêm Postman flow cho xuất kho thành công.
- [ ] Test trường hợp xuất quá số lượng tồn.
- [ ] Test User không được tạo hoặc xác nhận phiếu.
- [ ] Test Staff không được hủy phiếu đã xác nhận.
- [ ] Test một phiếu không thể xác nhận hai lần.
- [ ] Test transaction rollback khi một dòng hàng không hợp lệ.
- [ ] Cập nhật bảng phân quyền và hướng dẫn chạy trong README.

Kết quả mong đợi: collection có thể chạy lại, tự lưu ID cần dùng và dọn dữ liệu test nếu phù hợp.

## Thứ tự commit gợi ý

Mỗi phần nên tách thành commit nhỏ để dễ review:

1. Domain model và migration.
2. API đọc tồn kho.
3. Tạo và cập nhật phiếu nhập nháp.
4. Xác nhận phiếu nhập.
5. Tạo và cập nhật phiếu xuất nháp.
6. Xác nhận phiếu xuất và kiểm tra tồn.
7. Hủy phiếu bằng giao dịch đảo ngược.
8. API lịch sử, phân trang và bộ lọc.
9. Postman, README và kiểm tra cuối.

## Điều kiện xem là hoàn thành

- Tồn kho không bị âm.
- Một phiếu chỉ được xác nhận một lần.
- Cập nhật nhiều dòng hàng phải nằm trong cùng transaction database.
- API trả đúng `401` khi thiếu token và `403` khi sai role.
- Không xóa lịch sử giao dịch đã ảnh hưởng đến tồn kho.
- Có thể giải thích được cách tính tồn, cách rollback và lý do phân quyền cho từng endpoint.
