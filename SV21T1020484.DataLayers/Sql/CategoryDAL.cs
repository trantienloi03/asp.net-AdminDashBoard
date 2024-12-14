using Dapper;
using SV21T1020484.DomainModels;
using System.Collections.Generic;

namespace SV21T1020484.DataLayers.Sql
{
    public class CategoryDAL : BaseDAL, ICommonDAL<Category>
    {
        public CategoryDAL(string connectString) : base(connectString)
        {
        }

        public int Add(Category data)
        {
            int id = 0;
            using(var conn = OpenConnection()) 
            {
                var sql = @"if exists(select * from Categories where CategoryName = @CategoryName)
                                select -1
                            else
                                begin
                                    insert into Categories(CategoryName, Description)
                                    values (@CategoryName, @Description);
                                    select SCOPE_IDENTITY();
                                end";
                var parameters = new 
                {
                    CategoryName = data.CategoryName ??"", 
                    Description = data.Description ?? ""
                };
                id = conn.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return id;
        }

        public int Count(string searchValue)
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var conn = OpenConnection())
            {
                String sql = @"select count(*) from Categories where CategoryName like @searchValue";
                var parameters = new { searchValue = searchValue };
                count = conn.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            }
                return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using(var conn = OpenConnection())
            {
                string sql = @"delete from categories
                               where CategoryID = @CategoryID";
                var parameters = new { CategoryID = id };
                result = conn.Execute(sql:sql, param:parameters, commandType:System.Data.CommandType.Text) >0;
                conn.Close();
            }
            return result;
        }

        public Category? GetByID(int id)
        {
            Category? data = null;
            using(var conn = OpenConnection())
            {
                string sql = @"select * from Categories where CategoryID = @CategoryID";
                var parameters = new { CategoryID = id };
                data = conn.QueryFirstOrDefault<Category>(sql:sql, param:parameters, commandType: System.Data.CommandType.Text);
                conn.Close() ;
            }
            return data;
        }

        public bool InUse(int id)
        {
           bool result = false ;
            using(var conn = OpenConnection())
            {
                string sql = @"";
            }
            return result;
        }

        public List<Category> List(int page, int pageSize, string searchValue )
        {
           List<Category> list = new List<Category>();
            searchValue = $"%{searchValue}%";
            using (var conn = OpenConnection())
            {
                String sql = @"select * from (
                                            select *, row_number() over(order by CategoryName) as RowNumber
                                            from Categories
                                            where CategoryName like @searchValue) as c
                                where (@page = 0) or(c.RowNumber between (@page-1)*@pageSize+1 and @page*@pageSize)
                                order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                list = conn.Query<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            }
                return list;
        }

        public bool Update(Category data)
        {
            bool result = false;
            using(var conn = OpenConnection())
            {
                var sql = @"if not exists(select * from Categories where CategoryID <> @CategoryID and CategoryName = @CategoryName )
                                begin
                                    update Categories
                                    set 
                                        CategoryName = @CategoryName,
                                        Description = @Description
                                    where CategoryID = @CategoryID
                                end";
                var parameters = new { CategoryName = data.CategoryName, Description = data.Description, CategoryID = data.CategoryID};
                result = conn.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) >0;
                conn.Close();
            }
            return result;
        }
        public List<Category> GetAll()
        {
            List<Category> list = new List<Category>();
            using (var conn = OpenConnection())
            {
                String sql = @"select * from Categories "
;
                list = conn.Query<Category>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            }
            return list;
        }
    }
}
