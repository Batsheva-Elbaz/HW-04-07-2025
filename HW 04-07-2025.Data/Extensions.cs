using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW_04_07_2025.Data
{
    public static class Extensions
    {
        public static T GetOrDefault<T>(this SqlDataReader reader, string column)
        {
            object value = reader[column];
            if (value == DBNull.Value)
            {
                return default(T);
            }

            return (T)value;
        }
    }
}
