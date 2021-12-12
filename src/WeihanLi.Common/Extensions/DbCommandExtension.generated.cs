using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WeihanLi.Extensions
{
#nullable enable
    public static partial class DataExtension
    {
              
        public static IEnumerable<dynamic> Select(this DbCommand command) 
        {
            using (var reader = command.ExecuteReader())
            {
                return reader.ToExpandoObjects();
            }
        }

       public static async Task<IEnumerable<dynamic>> SelectAsync(this DbCommand command, CancellationToken cancellationToken = default)
       {
           using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
           {
                var list = new List<dynamic>();
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    list.Add(reader.ToExpandoObject(true));
                }
                return list;
           }
       }

        public static IEnumerable<T> Select<T>(this DbCommand command) 
        {
            using (var reader = command.ExecuteReader())
            {
                var list = new List<T>();
                while (reader.Read())
                {
                    list.Add(reader.ToEntity<T>(true)!);
                }
                return list;
            }
        }

       public static async Task<IEnumerable<T>> SelectAsync<T>(this DbCommand command, CancellationToken cancellationToken = default)
       {
           using (var reader = await command.ExecuteReaderAsync(cancellationToken))
           {
                var list = new List<T>();
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    list.Add(reader.ToEntity<T>(true)!);
                }
                return list;
           }
       }

      
        public static dynamic Fetch(this DbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                return reader.ToExpandoObject();
            }
        }

       public static async Task<dynamic> FetchAsync(this DbCommand command, CancellationToken cancellationToken = default)
       {
           using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
           {
               await reader.ReadAsync().ConfigureAwait(false);
               return reader.ToExpandoObject(true);
           }
       }

        public static T? Fetch<T>(this DbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                return reader.ToEntity<T>();
            }
        }

       public static async Task<T?> FetchAsync<T>(this DbCommand command, CancellationToken cancellationToken = default)
       {
           using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
           {
               return reader.ToEntity<T>();
           }
       }

      
        public static DataTable ExecuteDataTable(this DbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                return reader.ToDataTable();
            }
        }

       public static async Task<DataTable> ExecuteDataTableAsync(this DbCommand command, CancellationToken cancellationToken = default)
       {
           using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                return reader.ToDataTable();
            }
       }
    }
}
