using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WeihanLi.Extensions
{
    public static partial class DataExtension
    {
              public static int Execute([NotNull]this DbConnection conn, string cmdText) => conn.Execute(cmdText, null);

        public static int Execute([NotNull]this DbConnection conn, string cmdText, object paramInfo) => conn.Execute(cmdText, CommandType.Text, paramInfo, null, null);

        public static int Execute([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters)=> conn.Execute(cmdText, CommandType.Text, paramInfo, paramters, null);

        public static int Execute([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] paramters)=> conn.Execute(cmdText, commandType, paramInfo, paramters, null);

        public static int Execute([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction)
        {
            conn.EnsureOpen();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = cmdText;
                command.CommandType = commandType;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.AttachDbParameters(paramInfo);
                return command.ExecuteNonQuery();
            }
        }

       public static Task<int> ExecuteAsync([NotNull]this DbConnection conn, string cmdText) => conn.ExecuteAsync(cmdText, null);

       public static Task<int> ExecuteAsync([NotNull]this DbConnection conn, string cmdText, object paramInfo) => conn.ExecuteAsync(cmdText, CommandType.Text, paramInfo, null, null);

       public static Task<int> ExecuteAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo) => conn.ExecuteAsync(cmdText, commandType, paramInfo, null, null);

       public static Task<int> ExecuteAsync([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters)=> conn.ExecuteAsync(cmdText, CommandType.Text, paramInfo, paramters, null);

       public static async Task<int> ExecuteAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction)
       {
           await conn.EnsureOpenAsync();
           using (var command = conn.CreateCommand())
           {
               command.CommandText = cmdText;
               command.CommandType = commandType;
               command.Transaction = transaction;

               if (parameters != null)
               {
                   command.Parameters.AddRange(parameters);
               }

               command.AttachDbParameters(paramInfo);
               return await command.ExecuteNonQueryAsync();
           }
       }
      public static object ExecuteScalar([NotNull]this DbConnection conn, string cmdText) => conn.ExecuteScalar(cmdText, null);

        public static object ExecuteScalar([NotNull]this DbConnection conn, string cmdText, object paramInfo) => conn.ExecuteScalar(cmdText, CommandType.Text, paramInfo, null, null);

        public static object ExecuteScalar([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters)=> conn.ExecuteScalar(cmdText, CommandType.Text, paramInfo, paramters, null);

        public static object ExecuteScalar([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] paramters)=> conn.ExecuteScalar(cmdText, commandType, paramInfo, paramters, null);

        public static object ExecuteScalar([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction)
        {
            conn.EnsureOpen();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = cmdText;
                command.CommandType = commandType;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.AttachDbParameters(paramInfo);
                return command.ExecuteScalar();
            }
        }

       public static Task<object> ExecuteScalarAsync([NotNull]this DbConnection conn, string cmdText) => conn.ExecuteScalarAsync(cmdText, null);

       public static Task<object> ExecuteScalarAsync([NotNull]this DbConnection conn, string cmdText, object paramInfo) => conn.ExecuteScalarAsync(cmdText, CommandType.Text, paramInfo, null, null);

       public static Task<object> ExecuteScalarAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo) => conn.ExecuteScalarAsync(cmdText, commandType, paramInfo, null, null);

       public static Task<object> ExecuteScalarAsync([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters)=> conn.ExecuteScalarAsync(cmdText, CommandType.Text, paramInfo, paramters, null);

       public static async Task<object> ExecuteScalarAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction)
       {
           await conn.EnsureOpenAsync();
           using (var command = conn.CreateCommand())
           {
               command.CommandText = cmdText;
               command.CommandType = commandType;
               command.Transaction = transaction;

               if (parameters != null)
               {
                   command.Parameters.AddRange(parameters);
               }

               command.AttachDbParameters(paramInfo);
               return await command.ExecuteScalarAsync();
           }
       }
      public static DataTable ExecuteDataTable([NotNull]this DbConnection conn, string cmdText) => conn.ExecuteDataTable(cmdText, null);

        public static DataTable ExecuteDataTable([NotNull]this DbConnection conn, string cmdText, object paramInfo) => conn.ExecuteDataTable(cmdText, CommandType.Text, paramInfo, null, null);

        public static DataTable ExecuteDataTable([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters)=> conn.ExecuteDataTable(cmdText, CommandType.Text, paramInfo, paramters, null);

        public static DataTable ExecuteDataTable([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] paramters)=> conn.ExecuteDataTable(cmdText, commandType, paramInfo, paramters, null);

        public static DataTable ExecuteDataTable([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction)
        {
            conn.EnsureOpen();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = cmdText;
                command.CommandType = commandType;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.AttachDbParameters(paramInfo);
                return command.ExecuteDataTable();
            }
        }

       public static Task<DataTable> ExecuteDataTableAsync([NotNull]this DbConnection conn, string cmdText) => conn.ExecuteDataTableAsync(cmdText, null);

       public static Task<DataTable> ExecuteDataTableAsync([NotNull]this DbConnection conn, string cmdText, object paramInfo) => conn.ExecuteDataTableAsync(cmdText, CommandType.Text, paramInfo, null, null);

       public static Task<DataTable> ExecuteDataTableAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo) => conn.ExecuteDataTableAsync(cmdText, commandType, paramInfo, null, null);

       public static Task<DataTable> ExecuteDataTableAsync([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters)=> conn.ExecuteDataTableAsync(cmdText, CommandType.Text, paramInfo, paramters, null);

       public static async Task<DataTable> ExecuteDataTableAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction)
       {
           await conn.EnsureOpenAsync();
           using (var command = conn.CreateCommand())
           {
               command.CommandText = cmdText;
               command.CommandType = commandType;
               command.Transaction = transaction;

               if (parameters != null)
               {
                   command.Parameters.AddRange(parameters);
               }

               command.AttachDbParameters(paramInfo);
               return await command.ExecuteDataTableAsync();
           }
       }

      public static IEnumerable<T> Select<T>([NotNull]this DbConnection conn, string cmdText) where T:new() => conn.Select<T>(cmdText,null, null);
        
        public static IEnumerable<T> Select<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo) where T:new() => conn.Select<T>(cmdText, CommandType.Text, paramInfo,null, null);

        public static IEnumerable<T> Select<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo) where T:new() => conn.Select<T>(cmdText, commandType, paramInfo, null, null);

        public static IEnumerable<T> Select<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters) where T:new()=> conn.Select<T>(cmdText, CommandType.Text, paramInfo , paramters, null);

        public static IEnumerable<T> Select<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction) where T:new()
        {
            conn.EnsureOpen();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = cmdText;
                command.CommandType = commandType;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.AttachDbParameters(paramInfo);
                return command.Select<T>();
            }
        }

       public static Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbConnection conn, string cmdText) where T:new() => conn.SelectAsync<T>(cmdText, null);

       public static Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo) where T:new() => conn.SelectAsync<T>(cmdText, CommandType.Text, paramInfo, null, null);

       public static Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo) where T:new() => conn.SelectAsync<T>(cmdText, commandType, paramInfo, null, null);

       public static Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters) where T:new()=> conn.SelectAsync<T>(cmdText, CommandType.Text, paramInfo, paramters, null);

       public static Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] paramters) where T:new()=> conn.SelectAsync<T>(cmdText, commandType, paramInfo, paramters, null);

       public static async Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction) where T:new()
       {
           await conn.EnsureOpenAsync();
           using (var command = conn.CreateCommand())
           {
               command.CommandText = cmdText;
               command.CommandType = commandType;
               command.Transaction = transaction;

               if (parameters != null)
               {
                   command.Parameters.AddRange(parameters);
               }
               command.AttachDbParameters(paramInfo);

               return await command.SelectAsync<T>();
           }
       }
      public static T Fetch<T>([NotNull]this DbConnection conn, string cmdText) where T:new() => conn.Fetch<T>(cmdText,null, null);
        
        public static T Fetch<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo) where T:new() => conn.Fetch<T>(cmdText, CommandType.Text, paramInfo,null, null);

        public static T Fetch<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo) where T:new() => conn.Fetch<T>(cmdText, commandType, paramInfo, null, null);

        public static T Fetch<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters) where T:new()=> conn.Fetch<T>(cmdText, CommandType.Text, paramInfo , paramters, null);

        public static T Fetch<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction) where T:new()
        {
            conn.EnsureOpen();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = cmdText;
                command.CommandType = commandType;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.AttachDbParameters(paramInfo);
                return command.Fetch<T>();
            }
        }

       public static Task<T> FetchAsync<T>([NotNull]this DbConnection conn, string cmdText) where T:new() => conn.FetchAsync<T>(cmdText, null);

       public static Task<T> FetchAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo) where T:new() => conn.FetchAsync<T>(cmdText, CommandType.Text, paramInfo, null, null);

       public static Task<T> FetchAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo) where T:new() => conn.FetchAsync<T>(cmdText, commandType, paramInfo, null, null);

       public static Task<T> FetchAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters) where T:new()=> conn.FetchAsync<T>(cmdText, CommandType.Text, paramInfo, paramters, null);

       public static Task<T> FetchAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] paramters) where T:new()=> conn.FetchAsync<T>(cmdText, commandType, paramInfo, paramters, null);

       public static async Task<T> FetchAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction) where T:new()
       {
           await conn.EnsureOpenAsync();
           using (var command = conn.CreateCommand())
           {
               command.CommandText = cmdText;
               command.CommandType = commandType;
               command.Transaction = transaction;

               if (parameters != null)
               {
                   command.Parameters.AddRange(parameters);
               }
               command.AttachDbParameters(paramInfo);

               return await command.FetchAsync<T>();
           }
       }


      public static T Query<T>([NotNull]this DbConnection conn, string cmdText, Func<DataTable, T> func) where T:new() => conn.Query<T>(cmdText,null, null, func);
        
        public static T Query<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, Func<DataTable, T> func) where T:new() => conn.Query<T>(cmdText, CommandType.Text, paramInfo,null, null, func);

        public static T Query<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, Func<DataTable, T> func) where T:new() => conn.Query<T>(cmdText, commandType, paramInfo, null, null, func);

        public static T Query<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, DbParameter[] paramters, Func<DataTable, T> func) where T:new()=> conn.Query<T>(cmdText, CommandType.Text, paramInfo , paramters, null, func);

        public static T Query<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters, DbTransaction transaction, Func<DataTable, T> func) where T:new()
        {
            conn.EnsureOpen();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = cmdText;
                command.CommandType = commandType;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.AttachDbParameters(paramInfo);
                return command.ExecuteDataTable(func);
            }
        }

       public static Task<T> QueryAsync<T>([NotNull]this DbConnection conn, string cmdText,Func<DataTable, T> func) where T:new() => conn.QueryAsync<T>(cmdText, null, func);

       public static Task<T> QueryAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo,Func<DataTable, T> func) where T:new() => conn.QueryAsync<T>(cmdText, CommandType.Text, paramInfo, null, null, func);

       public static Task<T> QueryAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo,Func<DataTable, T> func) where T:new() => conn.QueryAsync<T>(cmdText, commandType, paramInfo, null, null, func);

       public static Task<T> QueryAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, DbParameter[] paramters,Func<DataTable, T> func) where T:new()=> conn.QueryAsync<T>(cmdText, CommandType.Text, paramInfo, paramters, null, func);

       public static Task<T> QueryAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] paramters,Func<DataTable, T> func) where T:new()=> conn.QueryAsync<T>(cmdText, commandType, paramInfo, paramters, null);

       public static async Task<T> QueryAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction,Func<DataTable, T> func) where T:new()
       {
           await conn.EnsureOpenAsync();
           using (var command = conn.CreateCommand())
           {
               command.CommandText = cmdText;
               command.CommandType = commandType;
               command.Transaction = transaction;

               if (parameters != null)
               {
                   command.Parameters.AddRange(parameters);
               }
               command.AttachDbParameters(paramInfo);
               return func(await command.ExecuteDataTableAsync());
           }
       }

      public static T ExecuteScalarTo<T>([NotNull]this DbConnection conn, string cmdText) => conn.ExecuteScalarTo<T>(cmdText,null, null);
        
        public static T ExecuteScalarTo<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo) => conn.ExecuteScalarTo<T>(cmdText, CommandType.Text, paramInfo,null, null);

        public static T ExecuteScalarTo<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo) => conn.ExecuteScalarTo<T>(cmdText, commandType, paramInfo, null, null);

        public static T ExecuteScalarTo<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters)=> conn.ExecuteScalarTo<T>(cmdText, CommandType.Text, paramInfo , paramters, null);

        public static T ExecuteScalarTo<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction)
        {
            conn.EnsureOpen();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = cmdText;
                command.CommandType = commandType;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.AttachDbParameters(paramInfo);
                return command.ExecuteScalarTo<T>();
            }
        }

       public static Task<T> ExecuteScalarToAsync<T>([NotNull]this DbConnection conn, string cmdText) => conn.ExecuteScalarToAsync<T>(cmdText, null);

       public static Task<T> ExecuteScalarToAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo) => conn.ExecuteScalarToAsync<T>(cmdText, CommandType.Text, paramInfo, null, null);

       public static Task<T> ExecuteScalarToAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo) => conn.ExecuteScalarToAsync<T>(cmdText, commandType, paramInfo, null, null);

       public static Task<T> ExecuteScalarToAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters)=> conn.ExecuteScalarToAsync<T>(cmdText, CommandType.Text, paramInfo, paramters, null);

       public static Task<T> ExecuteScalarToAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] paramters)=> conn.ExecuteScalarToAsync<T>(cmdText, commandType, paramInfo, paramters, null);

       public static async Task<T> ExecuteScalarToAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction)
       {
           await conn.EnsureOpenAsync();
           using (var command = conn.CreateCommand())
           {
               command.CommandText = cmdText;
               command.CommandType = commandType;
               command.Transaction = transaction;

               if (parameters != null)
               {
                   command.Parameters.AddRange(parameters);
               }
               command.AttachDbParameters(paramInfo);
               return await command.ExecuteScalarToAsync<T>();
           }
       }
      public static T ExecuteScalarToOrDefault<T>([NotNull]this DbConnection conn, string cmdText) => conn.ExecuteScalarToOrDefault<T>(cmdText,null, null);
        
        public static T ExecuteScalarToOrDefault<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo) => conn.ExecuteScalarToOrDefault<T>(cmdText, CommandType.Text, paramInfo,null, null);

        public static T ExecuteScalarToOrDefault<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo) => conn.ExecuteScalarToOrDefault<T>(cmdText, commandType, paramInfo, null, null);

        public static T ExecuteScalarToOrDefault<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters)=> conn.ExecuteScalarToOrDefault<T>(cmdText, CommandType.Text, paramInfo , paramters, null);

        public static T ExecuteScalarToOrDefault<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction)
        {
            conn.EnsureOpen();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = cmdText;
                command.CommandType = commandType;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.AttachDbParameters(paramInfo);
                return command.ExecuteScalarToOrDefault<T>();
            }
        }

       public static Task<T> ExecuteScalarToOrDefaultAsync<T>([NotNull]this DbConnection conn, string cmdText) => conn.ExecuteScalarToOrDefaultAsync<T>(cmdText, null);

       public static Task<T> ExecuteScalarToOrDefaultAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo) => conn.ExecuteScalarToOrDefaultAsync<T>(cmdText, CommandType.Text, paramInfo, null, null);

       public static Task<T> ExecuteScalarToOrDefaultAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo) => conn.ExecuteScalarToOrDefaultAsync<T>(cmdText, commandType, paramInfo, null, null);

       public static Task<T> ExecuteScalarToOrDefaultAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, params DbParameter[] paramters)=> conn.ExecuteScalarToOrDefaultAsync<T>(cmdText, CommandType.Text, paramInfo, paramters, null);

       public static Task<T> ExecuteScalarToOrDefaultAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] paramters)=> conn.ExecuteScalarToOrDefaultAsync<T>(cmdText, commandType, paramInfo, paramters, null);

       public static async Task<T> ExecuteScalarToOrDefaultAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters,DbTransaction transaction)
       {
           await conn.EnsureOpenAsync();
           using (var command = conn.CreateCommand())
           {
               command.CommandText = cmdText;
               command.CommandType = commandType;
               command.Transaction = transaction;

               if (parameters != null)
               {
                   command.Parameters.AddRange(parameters);
               }
               command.AttachDbParameters(paramInfo);
               return await command.ExecuteScalarToOrDefaultAsync<T>();
           }
       }

      public static IEnumerable<T> QueryColumn<T>([NotNull]this DbConnection conn, string cmdText, int columnIndex = 0) => conn.QueryColumn<T>(cmdText, null, columnIndex);

        public static IEnumerable<T> QueryColumn<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, int columnIndex = 0) => conn.QueryColumn<T>(cmdText, CommandType.Text, paramInfo, null, null, columnIndex);

        public static IEnumerable<T> QueryColumn<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, int columnIndex = 0) => conn.QueryColumn<T>(cmdText, commandType, paramInfo, null, null, columnIndex);

        public static IEnumerable<T> QueryColumn<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, int columnIndex = 0, params DbParameter[] paramters) => conn.QueryColumn<T>(cmdText, CommandType.Text, paramInfo, paramters, null, columnIndex);

        public static IEnumerable<T> QueryColumn<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] paramters, int columnIndex = 0) => conn.QueryColumn<T>(cmdText, commandType, paramInfo, paramters, null, columnIndex);

        public static IEnumerable<T> QueryColumn<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters, DbTransaction transaction, int columnIndex = 0)
        {
            conn.EnsureOpen();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = cmdText;
                command.CommandType = commandType;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.AttachDbParameters(paramInfo);
                return command.ExecuteDataTable().ColumnToList<T>(columnIndex);
            }
        }

       public static Task<IEnumerable<T>> QueryColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, int columnIndex = 0)=> conn.QueryColumnAsync<T>(cmdText, null, columnIndex);

       public static Task<IEnumerable<T>> QueryColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, int columnIndex = 0)=> conn.QueryColumnAsync<T>(cmdText, CommandType.Text, paramInfo, null, null, columnIndex);


       public static Task<IEnumerable<T>> QueryColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, int columnIndex = 0)=> conn.QueryColumnAsync<T>(cmdText, commandType, paramInfo, null, null, columnIndex);

       public static Task<IEnumerable<T>> QueryColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, int columnIndex = 0, params DbParameter[] paramters)=> conn.QueryColumnAsync<T>(cmdText, CommandType.Text, paramInfo, paramters, null, columnIndex);

       public static Task<IEnumerable<T>> QueryColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] paramters, int columnIndex = 0) => conn.QueryColumnAsync<T>(cmdText, commandType, paramInfo, paramters, null, columnIndex);

       public static async Task<IEnumerable<T>> QueryColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters, DbTransaction transaction, int columnIndex = 0)
       {
           await conn.EnsureOpenAsync();
           using (var command = conn.CreateCommand())
           {
               command.CommandText = cmdText;
               command.CommandType = commandType;
               command.Transaction = transaction;

               if (parameters != null)
               {
                   command.Parameters.AddRange(parameters);
               }
               command.AttachDbParameters(paramInfo);
               return (await command.ExecuteDataTableAsync()).ColumnToList<T>(columnIndex);
           }
       }
      public static IEnumerable<T> SelectColumn<T>([NotNull]this DbConnection conn, string cmdText, int columnIndex = 0) => conn.SelectColumn<T>(cmdText, null, columnIndex);

        public static IEnumerable<T> SelectColumn<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, int columnIndex = 0) => conn.SelectColumn<T>(cmdText, CommandType.Text, paramInfo, null, null, columnIndex);

        public static IEnumerable<T> SelectColumn<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, int columnIndex = 0) => conn.SelectColumn<T>(cmdText, commandType, paramInfo, null, null, columnIndex);

        public static IEnumerable<T> SelectColumn<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, int columnIndex = 0, params DbParameter[] paramters) => conn.SelectColumn<T>(cmdText, CommandType.Text, paramInfo, paramters, null, columnIndex);

        public static IEnumerable<T> SelectColumn<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] paramters, int columnIndex = 0) => conn.SelectColumn<T>(cmdText, commandType, paramInfo, paramters, null, columnIndex);

        public static IEnumerable<T> SelectColumn<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters, DbTransaction transaction, int columnIndex = 0)
        {
            conn.EnsureOpen();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = cmdText;
                command.CommandType = commandType;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.AttachDbParameters(paramInfo);
                return command.ExecuteDataTable().ColumnToList<T>(columnIndex);
            }
        }

       public static Task<IEnumerable<T>> SelectColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, int columnIndex = 0)=> conn.SelectColumnAsync<T>(cmdText, null, columnIndex);

       public static Task<IEnumerable<T>> SelectColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, int columnIndex = 0)=> conn.SelectColumnAsync<T>(cmdText, CommandType.Text, paramInfo, null, null, columnIndex);


       public static Task<IEnumerable<T>> SelectColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, int columnIndex = 0)=> conn.SelectColumnAsync<T>(cmdText, commandType, paramInfo, null, null, columnIndex);

       public static Task<IEnumerable<T>> SelectColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, object paramInfo, int columnIndex = 0, params DbParameter[] paramters)=> conn.SelectColumnAsync<T>(cmdText, CommandType.Text, paramInfo, paramters, null, columnIndex);

       public static Task<IEnumerable<T>> SelectColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] paramters, int columnIndex = 0) => conn.SelectColumnAsync<T>(cmdText, commandType, paramInfo, paramters, null, columnIndex);

       public static async Task<IEnumerable<T>> SelectColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object paramInfo, DbParameter[] parameters, DbTransaction transaction, int columnIndex = 0)
       {
           await conn.EnsureOpenAsync();
           using (var command = conn.CreateCommand())
           {
               command.CommandText = cmdText;
               command.CommandType = commandType;
               command.Transaction = transaction;

               if (parameters != null)
               {
                   command.Parameters.AddRange(parameters);
               }
               command.AttachDbParameters(paramInfo);
               return (await command.ExecuteDataTableAsync()).ColumnToList<T>(columnIndex);
           }
       }
    }
}