using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Student_Att.DataAccess
{
    public static class DbErrorHelper
    {
        // When SaveChanges() breaks a UNIQUE rule in the database, EF Core throws a
        // DbUpdateException. The real SQL Server error is inside it (InnerException).
        // 2627 = UNIQUE constraint, 2601 = unique index.
        public static bool IsDuplicateKey(DbUpdateException ex)
        {
            SqlException sqlError = ex.InnerException as SqlException;
            return sqlError != null && (sqlError.Number == 2627 || sqlError.Number == 2601);
        }
    }
}
