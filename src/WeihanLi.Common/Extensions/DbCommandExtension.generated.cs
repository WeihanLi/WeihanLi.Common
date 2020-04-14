using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WeihanLi.Extensions
{
    public static partial class DataExtension
    {
              
        public static IEnumerable<dynamic> Select([NotNull]this DbCommand command) 
        {
            using (var reader = command.ExecuteReader())
            {
                return reader.ToExpandoObjects();
            }
        }

       public static async Task<IEnumerable<dynamic>> SelectAsync([NotNull]this DbCommand command, CancellationToken cancellationToken = default)
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

        public static IEnumerable<T> Select<T>([NotNull]this DbCommand command) 
        {
            using (var reader = command.ExecuteReader())
            {
                var list = new List<T>();
                while (reader.Read())
                {
                    list.Add(reader.ToEntity<T>(true));
                }
                return list;
            }
        }

       public static async Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbCommand command, CancellationToken cancellationToken = default)
       {
           using (var reader = await command.ExecuteReaderAsync(cancellationToken))
           {
                var list = new List<T>();
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    list.Add(reader.ToEntity<T>(true));
                }
                return list;
           }
       }

      
        public static dynamic Fetch([NotNull]this DbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                return reader.ToExpandoObject();
            }
        }

       public static async Task<dynamic> FetchAsync([NotNull]this DbCommand command, CancellationToken cancellationToken = default)
       {
           using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
           {
               await reader.ReadAsync().ConfigureAwait(false);
               return reader.ToExpandoObject(true);
           }
       }

        public static T Fetch<T>([NotNull]this DbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                return reader.ToEntity<T>();
            }
        }

       public static async Task<T> FetchAsync<T>([NotNull]this DbCommand command, CancellationToken cancellationToken = default)
       {
           using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
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

       public static async Task<DataTable> ExecuteDataTableAsync([NotNull]this DbCommand command, CancellationToken cancellationToken = default)
       {
           using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                return reader.ToDataTable();
            }
       }
    }
}
