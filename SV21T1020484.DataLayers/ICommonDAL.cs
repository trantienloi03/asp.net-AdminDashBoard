namespace SV21T1020484.DataLayers
{
    /// <summary>
    /// định nghĩa các phép xử lý dữ liệu thường dùng trên bảng (customer, employee, shipper)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// Timf kiếm và lấy danh sách dữ liệu kiểu <T> dưới dạng phân trang 
        /// </summary>
        /// <param name="page">trang cần hiển thị</param>
        /// <param name="pageSize">số dòng đc hiển thị trên mỗi trang( bằng 0 nếu ko phân trang)</param>
        /// <param name="searchValue">giá trị cần tìm kiếm( chuỗi rỗng nếu lấy toàn bộ dữ liệu)</param>
        /// <returns></returns>
        List<T> List(int page = 1, int pageSize = 0, string searchValue = "");
        /// <summary>
        /// đếm số lượng dòng dữ kiệu đếm đc theo searchValue
        /// </summary>
        /// <param name="searchValue">Giá trị cần tìm kiếm(chuỗi rỗng nếu đếm trên toàn bộ dữ liệu</param>
        /// <returns></returns>
        int Count(string searchValue = "");
        /// <summary>
        /// lấy 1 bản ghi dữ liệu của T theo id(trả về null nếu dữ liệu ko tồn tại
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? GetByID(int id);
        /// <summary>
        /// bổ sung 1 dữ liệu. trả về id của dữ liệu vừa add
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);
        /// <summary>
        /// cập nhật 1 bản dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);
        bool Delete(int id);
        /// <summary>
        /// kiểm tra xem 1 bản ghi dữ liệu có đc tham chiếu ở bảng khác hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUse(int id);
        List<T> GetAll();
    }
}
