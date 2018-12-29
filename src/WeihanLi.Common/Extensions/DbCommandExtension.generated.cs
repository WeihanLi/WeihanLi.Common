using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WeihanLi.Extensions
{
    public static partial class DataExtension
    {
              
        public static IEnumerable<T> Select<T>([NotNull]this DbCommand command) where T:new()
        {
            using (var reader = command.ExecuteReader())
            {
                return reader.ToDataTable().ToEntities<T>();
            }
        }

       public static async Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbCommand command) where T:new()
       {
           using (var reader = await command.ExecuteReaderAsync())
           {
                return reader.ToDataTable().ToEntities<T>();
           }
       }

      
        public static T Fetch<T>([NotNull]this DbCommand command) where T:new()
        {
            using (var reader = command.ExecuteReader())
            {
                return reader.ToEntity<T>();
            }
        }

       public static async Task<T> FetchAsync<T>([NotNull]this DbCommand command) where T:new()
       {
           using (var reader = await command.ExecuteReaderAsync())
            {
                return reader.ToEntity<T>();
            }
       }

      
        public static DataTable ExecuteDataTable([NotNull]this DbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                return reader.ToDataTable();
            }
        }

       public static async Task<DataTable> ExecuteDataTableAsync([NotNull]this DbCommand command)
       {
           using (var reader = await command.ExecuteReaderAsync())
            {
                return reader.ToDataTable();
            }
       }
    }
}