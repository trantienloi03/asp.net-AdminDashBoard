namespace SV21T1020484.DataLayers
{
    public interface ISimpleQueryDAL<T> where T : class
    {
        /// <summary>
        /// truy van lay toan bo dw lieu
        /// </summary>
        /// <returns></returns>
        List<T> ListNoParam();
    }
}
