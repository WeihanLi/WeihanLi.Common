using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WeihanLi.Extensions
{
#nullable enable
    public static partial class DataExtension
    {
              public static int Execute([NotNull]this DbConnection conn, string cmdText, int commandTimeout = 60) => conn.Execute(cmdText, null, commandTimeout);

        public static int Execute([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout = 60) => conn.Execute(cmdText, CommandType.Text, paramInfo, null, null, commandTimeout);

        public static int Execute([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters)=> conn.Execute(cmdText, CommandType.Text, paramInfo, parameters, null);

        public static int Execute([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, int commandTimeout = 60)=> conn.Execute(cmdText, commandType, paramInfo, parameters, null, commandTimeout);

        public static int Execute([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60)
        {
            conn.EnsureOpen();
try{
            using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
            {
                return command.ExecuteNonQuery();
            }
}
finally{
   conn.Close();
}
        }

       public static Task<int> ExecuteAsync([NotNull]this DbConnection conn, string cmdText, int commandTimeout = 60, CancellationToken cancellationToken = default) => conn.ExecuteAsync(cmdText, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<int> ExecuteAsync([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout = 60, CancellationToken cancellationToken = default) => conn.ExecuteAsync(cmdText, CommandType.Text, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<int> ExecuteAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int commandTimeout = 60, CancellationToken cancellationToken = default) => conn.ExecuteAsync(cmdText, commandType, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<int> ExecuteAsync([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters)=> conn.ExecuteAsync(cmdText, CommandType.Text, paramInfo, parameters, null);

       public static async Task<int> ExecuteAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60, CancellationToken cancellationToken = default)
       {
           await conn.EnsureOpenAsync().ConfigureAwait(false);
try{        
   using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
           {
               return await command.ExecuteNonQueryAsync(cancellationToken);
           }
}
finally{
   conn.Close();
}
       }
      public static object? ExecuteScalar([NotNull]this DbConnection conn, string cmdText, int commandTimeout = 60) => conn.ExecuteScalar(cmdText, null, commandTimeout);

        public static object? ExecuteScalar([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout = 60) => conn.ExecuteScalar(cmdText, CommandType.Text, paramInfo, null, null, commandTimeout);

        public static object? ExecuteScalar([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters)=> conn.ExecuteScalar(cmdText, CommandType.Text, paramInfo, parameters, null);

        public static object? ExecuteScalar([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, int commandTimeout = 60)=> conn.ExecuteScalar(cmdText, commandType, paramInfo, parameters, null, commandTimeout);

        public static object? ExecuteScalar([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60)
        {
            conn.EnsureOpen();
try{
            using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
            {
                return command.ExecuteScalar();
            }
}
finally{
   conn.Close();
}
        }

       public static Task<object?> ExecuteScalarAsync([NotNull]this DbConnection conn, string cmdText, int commandTimeout = 60, CancellationToken cancellationToken = default) => conn.ExecuteScalarAsync(cmdText, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<object?> ExecuteScalarAsync([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout = 60, CancellationToken cancellationToken = default) => conn.ExecuteScalarAsync(cmdText, CommandType.Text, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<object?> ExecuteScalarAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int commandTimeout = 60, CancellationToken cancellationToken = default) => conn.ExecuteScalarAsync(cmdText, commandType, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<object?> ExecuteScalarAsync([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters)=> conn.ExecuteScalarAsync(cmdText, CommandType.Text, paramInfo, parameters, null);

       public static async Task<object?> ExecuteScalarAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60, CancellationToken cancellationToken = default)
       {
           await conn.EnsureOpenAsync().ConfigureAwait(false);
try{        
   using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
           {
               return await command.ExecuteScalarAsync(cancellationToken);
           }
}
finally{
   conn.Close();
}
       }
      public static dynamic Fetch([NotNull]this DbConnection conn, string cmdText, int commandTimeout = 60) => conn.Fetch(cmdText, null, commandTimeout);

        public static dynamic Fetch([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout = 60) => conn.Fetch(cmdText, CommandType.Text, paramInfo, null, null, commandTimeout);

        public static dynamic Fetch([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters)=> conn.Fetch(cmdText, CommandType.Text, paramInfo, parameters, null);

        public static dynamic Fetch([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, int commandTimeout = 60)=> conn.Fetch(cmdText, commandType, paramInfo, parameters, null, commandTimeout);

        public static dynamic Fetch([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60)
        {
            conn.EnsureOpen();
try{
            using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
            {
                return command.Fetch();
            }
}
finally{
   conn.Close();
}
        }

       public static Task<dynamic> FetchAsync([NotNull]this DbConnection conn, string cmdText, int commandTimeout = 60, CancellationToken cancellationToken = default) => conn.FetchAsync(cmdText, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<dynamic> FetchAsync([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout = 60, CancellationToken cancellationToken = default) => conn.FetchAsync(cmdText, CommandType.Text, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<dynamic> FetchAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int commandTimeout = 60, CancellationToken cancellationToken = default) => conn.FetchAsync(cmdText, commandType, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<dynamic> FetchAsync([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters)=> conn.FetchAsync(cmdText, CommandType.Text, paramInfo, parameters, null);

       public static async Task<dynamic> FetchAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60, CancellationToken cancellationToken = default)
       {
           await conn.EnsureOpenAsync().ConfigureAwait(false);
try{        
   using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
           {
               return await command.FetchAsync(cancellationToken);
           }
}
finally{
   conn.Close();
}
       }
      public static DataTable ExecuteDataTable([NotNull]this DbConnection conn, string cmdText, int commandTimeout = 60) => conn.ExecuteDataTable(cmdText, null, commandTimeout);

        public static DataTable ExecuteDataTable([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout = 60) => conn.ExecuteDataTable(cmdText, CommandType.Text, paramInfo, null, null, commandTimeout);

        public static DataTable ExecuteDataTable([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters)=> conn.ExecuteDataTable(cmdText, CommandType.Text, paramInfo, parameters, null);

        public static DataTable ExecuteDataTable([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, int commandTimeout = 60)=> conn.ExecuteDataTable(cmdText, commandType, paramInfo, parameters, null, commandTimeout);

        public static DataTable ExecuteDataTable([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60)
        {
            conn.EnsureOpen();
try{
            using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
            {
                return command.ExecuteDataTable();
            }
}
finally{
   conn.Close();
}
        }

       public static Task<DataTable> ExecuteDataTableAsync([NotNull]this DbConnection conn, string cmdText, int commandTimeout = 60, CancellationToken cancellationToken = default) => conn.ExecuteDataTableAsync(cmdText, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<DataTable> ExecuteDataTableAsync([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout = 60, CancellationToken cancellationToken = default) => conn.ExecuteDataTableAsync(cmdText, CommandType.Text, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<DataTable> ExecuteDataTableAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int commandTimeout = 60, CancellationToken cancellationToken = default) => conn.ExecuteDataTableAsync(cmdText, commandType, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<DataTable> ExecuteDataTableAsync([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters)=> conn.ExecuteDataTableAsync(cmdText, CommandType.Text, paramInfo, parameters, null);

       public static async Task<DataTable> ExecuteDataTableAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60, CancellationToken cancellationToken = default)
       {
           await conn.EnsureOpenAsync().ConfigureAwait(false);
try{        
   using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
           {
               return await command.ExecuteDataTableAsync(cancellationToken);
           }
}
finally{
   conn.Close();
}
       }

   public static T? Fetch<T>([NotNull]this DbConnection conn, string cmdText, int commandTimeout = 60)  => conn.Fetch<T>(cmdText, CommandType.Text, null, commandTimeout);
        
        public static T? Fetch<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout = 60)  => conn.Fetch<T>(cmdText, CommandType.Text, paramInfo,null, null, commandTimeout);

        public static T? Fetch<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int commandTimeout = 60)  => conn.Fetch<T>(cmdText, commandType, paramInfo, null, null, commandTimeout);

        public static T? Fetch<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters) => conn.Fetch<T>(cmdText, CommandType.Text, paramInfo , parameters, null);

        public static T? Fetch<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60) 
        {
            conn.EnsureOpen();
            try
            {
                using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
                {
                    return command.Fetch<T>();
                }
            }
            finally
            {
                conn.Close();
            }
        }

       public static Task<T?> FetchAsync<T>([NotNull]this DbConnection conn, string cmdText, int commandTimeout=60, CancellationToken cancellationToken = default)  => conn.FetchAsync<T>(cmdText, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<T?> FetchAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout=60, CancellationToken cancellationToken = default)  => conn.FetchAsync<T>(cmdText, CommandType.Text, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<T?> FetchAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int commandTimeout=60, CancellationToken cancellationToken = default)  => conn.FetchAsync<T>(cmdText, commandType, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<T?> FetchAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters) => conn.FetchAsync<T>(cmdText, CommandType.Text, paramInfo, parameters, null);

       public static Task<T?> FetchAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, int commandTimeout=60, CancellationToken cancellationToken = default) => conn.FetchAsync<T>(cmdText, commandType, paramInfo, parameters, null, commandTimeout, cancellationToken: cancellationToken);

       public static async Task<T?> FetchAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60, CancellationToken cancellationToken = default) 
       {
           await conn.EnsureOpenAsync().ConfigureAwait(false);
try{
           using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
           {
               return await command.FetchAsync<T>(cancellationToken);
           }
}
finally{
   conn.Close();
}
       }


   public static IEnumerable<T> Select<T>([NotNull]this DbConnection conn, string cmdText, int commandTimeout = 60)  => conn.Select<T>(cmdText, CommandType.Text, null, commandTimeout);
        
        public static IEnumerable<T> Select<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout = 60)  => conn.Select<T>(cmdText, CommandType.Text, paramInfo,null, null, commandTimeout);

        public static IEnumerable<T> Select<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int commandTimeout = 60)  => conn.Select<T>(cmdText, commandType, paramInfo, null, null, commandTimeout);

        public static IEnumerable<T> Select<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters) => conn.Select<T>(cmdText, CommandType.Text, paramInfo , parameters, null);

        public static IEnumerable<T> Select<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60) 
        {
            conn.EnsureOpen();
try
{
            using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
            {
                return command.Select<T>();
            }
}
finally{
   conn.Close();
}
        }

       public static Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbConnection conn, string cmdText, int commandTimeout=60, CancellationToken cancellationToken = default)  => conn.SelectAsync<T>(cmdText, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout=60, CancellationToken cancellationToken = default)  => conn.SelectAsync<T>(cmdText, CommandType.Text, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int commandTimeout=60, CancellationToken cancellationToken = default)  => conn.SelectAsync<T>(cmdText, commandType, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters) => conn.SelectAsync<T>(cmdText, CommandType.Text, paramInfo, parameters, null);

       public static Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, int commandTimeout=60, CancellationToken cancellationToken = default) => conn.SelectAsync<T>(cmdText, commandType, paramInfo, parameters, null, commandTimeout, cancellationToken: cancellationToken);

       public static async Task<IEnumerable<T>> SelectAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60, CancellationToken cancellationToken = default) 
       {
           await conn.EnsureOpenAsync().ConfigureAwait(false);
try
{
           using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
           {
               return await command.SelectAsync<T>(cancellationToken);
           }
}
finally{
   conn.Close();
}
       }
   public static IEnumerable<dynamic> Select([NotNull]this DbConnection conn, string cmdText, int commandTimeout = 60)  => conn.Select(cmdText, CommandType.Text, null, commandTimeout);
        
        public static IEnumerable<dynamic> Select([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout = 60)  => conn.Select(cmdText, CommandType.Text, paramInfo,null, null, commandTimeout);

        public static IEnumerable<dynamic> Select([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int commandTimeout = 60)  => conn.Select(cmdText, commandType, paramInfo, null, null, commandTimeout);

        public static IEnumerable<dynamic> Select([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters) => conn.Select(cmdText, CommandType.Text, paramInfo , parameters, null);

        public static IEnumerable<dynamic> Select([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60) 
        {
            conn.EnsureOpen();
try
{
            using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
            {
                return command.Select();
            }
}
finally{
   conn.Close();
}
        }

       public static Task<IEnumerable<dynamic>> SelectAsync([NotNull]this DbConnection conn, string cmdText, int commandTimeout=60, CancellationToken cancellationToken = default)  => conn.SelectAsync(cmdText, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<IEnumerable<dynamic>> SelectAsync([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int commandTimeout=60, CancellationToken cancellationToken = default)  => conn.SelectAsync(cmdText, CommandType.Text, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<IEnumerable<dynamic>> SelectAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int commandTimeout=60, CancellationToken cancellationToken = default)  => conn.SelectAsync(cmdText, commandType, paramInfo, null, null, commandTimeout, cancellationToken: cancellationToken);

       public static Task<IEnumerable<dynamic>> SelectAsync([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters) => conn.SelectAsync(cmdText, CommandType.Text, paramInfo, parameters, null);

       public static Task<IEnumerable<dynamic>> SelectAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, int commandTimeout=60, CancellationToken cancellationToken = default) => conn.SelectAsync(cmdText, commandType, paramInfo, parameters, null, commandTimeout, cancellationToken: cancellationToken);

       public static async Task<IEnumerable<dynamic>> SelectAsync([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout = 60, CancellationToken cancellationToken = default) 
       {
           await conn.EnsureOpenAsync().ConfigureAwait(false);
try
{
           using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
           {
               return await command.SelectAsync(cancellationToken);
           }
}
finally{
   conn.Close();
}
       }


      public static T ExecuteDataTable<T>([NotNull]this DbConnection conn, string cmdText, Func<DataTable, T> func)  => conn.ExecuteDataTable<T>(cmdText,null, null, func);
        
        public static T ExecuteDataTable<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, Func<DataTable, T> func)  => conn.ExecuteDataTable<T>(cmdText, CommandType.Text, paramInfo,null, null, func);

        public static T ExecuteDataTable<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, Func<DataTable, T> func)  => conn.ExecuteDataTable<T>(cmdText, commandType, paramInfo, null, null, func);

        public static T ExecuteDataTable<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, DbParameter[]? parameters, Func<DataTable, T> func) => conn.ExecuteDataTable<T>(cmdText, CommandType.Text, paramInfo , parameters, null, func);

        public static T ExecuteDataTable<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, DbTransaction? transaction, Func<DataTable, T> func, int commandTimeout = 60) 
        {
            conn.EnsureOpen();
try{
            using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
            {
                return command.ExecuteDataTable(func);
            }
}
finally{
   conn.Close();
}
        }

       public static Task<T> ExecuteDataTableAsync<T>([NotNull]this DbConnection conn, string cmdText,Func<DataTable, T> func, CancellationToken cancellationToken = default)  => conn.ExecuteDataTableAsync<T>(cmdText, null, func, cancellationToken: cancellationToken);

       public static Task<T> ExecuteDataTableAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo,Func<DataTable, T> func, CancellationToken cancellationToken = default)  => conn.ExecuteDataTableAsync<T>(cmdText, CommandType.Text, paramInfo, null, null, func, cancellationToken: cancellationToken);

       public static Task<T> ExecuteDataTableAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo,Func<DataTable, T> func, CancellationToken cancellationToken = default)  => conn.ExecuteDataTableAsync<T>(cmdText, commandType, paramInfo, null, null, func, cancellationToken: cancellationToken);

       public static Task<T> ExecuteDataTableAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, DbParameter[]? parameters,Func<DataTable, T> func, CancellationToken cancellationToken = default) => conn.ExecuteDataTableAsync<T>(cmdText, CommandType.Text, paramInfo, parameters, null, func, cancellationToken: cancellationToken);

       public static Task<T> ExecuteDataTableAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,Func<DataTable, T> func, CancellationToken cancellationToken = default) => conn.ExecuteDataTableAsync<T>(cmdText, commandType, paramInfo, parameters, func, cancellationToken: cancellationToken);

       public static async Task<T> ExecuteDataTableAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction,Func<DataTable, T> func, int commandTimeout = 60, CancellationToken cancellationToken = default) 
       {
           await conn.EnsureOpenAsync().ConfigureAwait(false);
try{
           using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
           {
               return func(await command.ExecuteDataTableAsync(cancellationToken));
           }
}
finally{
   conn.Close();
}
       }

      public static T ExecuteScalarTo<T>([NotNull]this DbConnection conn, string cmdText) => conn.ExecuteScalarTo<T>(cmdText,null, null);
        
        public static T ExecuteScalarTo<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo) => conn.ExecuteScalarTo<T>(cmdText, CommandType.Text, paramInfo,null, null);

        public static T ExecuteScalarTo<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo) => conn.ExecuteScalarTo<T>(cmdText, commandType, paramInfo, null, null);

        public static T ExecuteScalarTo<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters)=> conn.ExecuteScalarTo<T>(cmdText, CommandType.Text, paramInfo , parameters, null);

        public static T ExecuteScalarTo<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout=60)
        {
            conn.EnsureOpen();
try
{
            using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
            {
                return command.ExecuteScalarTo<T>();
            }
}
finally{
   conn.Close();
}
        }

       public static Task<T> ExecuteScalarToAsync<T>([NotNull]this DbConnection conn, string cmdText, CancellationToken cancellationToken = default) => conn.ExecuteScalarToAsync<T>(cmdText, null, cancellationToken: cancellationToken);

       public static Task<T> ExecuteScalarToAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, CancellationToken cancellationToken = default) => conn.ExecuteScalarToAsync<T>(cmdText, CommandType.Text, paramInfo, null, null, cancellationToken: cancellationToken);

       public static Task<T> ExecuteScalarToAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, CancellationToken cancellationToken = default) => conn.ExecuteScalarToAsync<T>(cmdText, commandType, paramInfo, null, null, cancellationToken: cancellationToken);

       public static Task<T> ExecuteScalarToAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters)=> conn.ExecuteScalarToAsync<T>(cmdText, CommandType.Text, paramInfo, parameters, null);

       public static Task<T> ExecuteScalarToAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, CancellationToken cancellationToken = default)=> conn.ExecuteScalarToAsync<T>(cmdText, commandType, paramInfo, parameters, null, cancellationToken: cancellationToken);

       public static async Task<T> ExecuteScalarToAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout=60, CancellationToken cancellationToken = default)
       {
           await conn.EnsureOpenAsync().ConfigureAwait(false);
try{
           using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
           {
               return await command.ExecuteScalarToAsync<T>(cancellationToken);
           }
}
finally{
   conn.Close();
}
       }
      public static T? ExecuteScalarToOrDefault<T>([NotNull]this DbConnection conn, string cmdText) => conn.ExecuteScalarToOrDefault<T>(cmdText,null, null);
        
        public static T? ExecuteScalarToOrDefault<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo) => conn.ExecuteScalarToOrDefault<T>(cmdText, CommandType.Text, paramInfo,null, null);

        public static T? ExecuteScalarToOrDefault<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo) => conn.ExecuteScalarToOrDefault<T>(cmdText, commandType, paramInfo, null, null);

        public static T? ExecuteScalarToOrDefault<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters)=> conn.ExecuteScalarToOrDefault<T>(cmdText, CommandType.Text, paramInfo , parameters, null);

        public static T? ExecuteScalarToOrDefault<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout=60)
        {
            conn.EnsureOpen();
try
{
            using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
            {
                return command.ExecuteScalarToOrDefault<T>();
            }
}
finally{
   conn.Close();
}
        }

       public static Task<T?> ExecuteScalarToOrDefaultAsync<T>([NotNull]this DbConnection conn, string cmdText, CancellationToken cancellationToken = default) => conn.ExecuteScalarToOrDefaultAsync<T>(cmdText, null, cancellationToken: cancellationToken);

       public static Task<T?> ExecuteScalarToOrDefaultAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, CancellationToken cancellationToken = default) => conn.ExecuteScalarToOrDefaultAsync<T>(cmdText, CommandType.Text, paramInfo, null, null, cancellationToken: cancellationToken);

       public static Task<T?> ExecuteScalarToOrDefaultAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, CancellationToken cancellationToken = default) => conn.ExecuteScalarToOrDefaultAsync<T>(cmdText, commandType, paramInfo, null, null, cancellationToken: cancellationToken);

       public static Task<T?> ExecuteScalarToOrDefaultAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, params DbParameter[]? parameters)=> conn.ExecuteScalarToOrDefaultAsync<T>(cmdText, CommandType.Text, paramInfo, parameters, null);

       public static Task<T?> ExecuteScalarToOrDefaultAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, CancellationToken cancellationToken = default)=> conn.ExecuteScalarToOrDefaultAsync<T>(cmdText, commandType, paramInfo, parameters, null, cancellationToken: cancellationToken);

       public static async Task<T?> ExecuteScalarToOrDefaultAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters,DbTransaction? transaction, int commandTimeout=60, CancellationToken cancellationToken = default)
       {
           await conn.EnsureOpenAsync().ConfigureAwait(false);
try{
           using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
           {
               return await command.ExecuteScalarToOrDefaultAsync<T>(cancellationToken);
           }
}
finally{
   conn.Close();
}
       }

      public static IEnumerable<T> QueryColumn<T>([NotNull]this DbConnection conn, string cmdText, int columnIndex = 0) => conn.QueryColumn<T>(cmdText, null, columnIndex);

        public static IEnumerable<T> QueryColumn<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int columnIndex = 0) => conn.QueryColumn<T>(cmdText, CommandType.Text, paramInfo, null, null, columnIndex);

        public static IEnumerable<T> QueryColumn<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int columnIndex = 0) => conn.QueryColumn<T>(cmdText, commandType, paramInfo, null, null, columnIndex);

        public static IEnumerable<T> QueryColumn<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int columnIndex = 0, params DbParameter[]? parameters) => conn.QueryColumn<T>(cmdText, CommandType.Text, paramInfo, parameters, null, columnIndex);

        public static IEnumerable<T> QueryColumn<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, int columnIndex = 0) => conn.QueryColumn<T>(cmdText, commandType, paramInfo, parameters, null, columnIndex);

        public static IEnumerable<T> QueryColumn<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, DbTransaction? transaction, int columnIndex = 0, int commandTimeout = 60)
        {
try
{
            conn.EnsureOpen();
            using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
            {
                using (var reader = command.ExecuteReader())
                {
                    var list = new List<T>();
                    while (reader.Read())
                    {
                        list.Add(reader[columnIndex].To<T>());
                    }
                    return list;
                }
            }
}
finally{
   conn.Close();
}
        }

       public static Task<IEnumerable<T>> QueryColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, int columnIndex = 0, CancellationToken cancellationToken = default)=> conn.QueryColumnAsync<T>(cmdText, null, columnIndex);

       public static Task<IEnumerable<T>> QueryColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int columnIndex = 0, CancellationToken cancellationToken = default)=> conn.QueryColumnAsync<T>(cmdText, CommandType.Text, paramInfo, null, null, columnIndex);


       public static Task<IEnumerable<T>> QueryColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int columnIndex = 0, CancellationToken cancellationToken = default)=> conn.QueryColumnAsync<T>(cmdText, commandType, paramInfo, null, null, columnIndex, cancellationToken: cancellationToken);

       public static Task<IEnumerable<T>> QueryColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int columnIndex = 0, params DbParameter[]? parameters)=> conn.QueryColumnAsync<T>(cmdText, CommandType.Text, paramInfo, parameters, null, columnIndex);

       public static Task<IEnumerable<T>> QueryColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, int columnIndex = 0, CancellationToken cancellationToken = default) => conn.QueryColumnAsync<T>(cmdText, commandType, paramInfo, parameters, null, columnIndex, cancellationToken: cancellationToken);

       public static async Task<IEnumerable<T>> QueryColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, DbTransaction? transaction, int columnIndex = 0, int commandTimeout=60, CancellationToken cancellationToken = default)
       {
           await conn.EnsureOpenAsync().ConfigureAwait(false);
try
{
           using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
           {
               using (var reader = command.ExecuteReader())
                {
                    var list = new List<T>();
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        list.Add(reader[columnIndex].To<T>());
                    }

                    return list;
                }
           }
}
finally{
   conn.Close();
}
       }
      public static IEnumerable<T> SelectColumn<T>([NotNull]this DbConnection conn, string cmdText, int columnIndex = 0) => conn.SelectColumn<T>(cmdText, null, columnIndex);

        public static IEnumerable<T> SelectColumn<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int columnIndex = 0) => conn.SelectColumn<T>(cmdText, CommandType.Text, paramInfo, null, null, columnIndex);

        public static IEnumerable<T> SelectColumn<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int columnIndex = 0) => conn.SelectColumn<T>(cmdText, commandType, paramInfo, null, null, columnIndex);

        public static IEnumerable<T> SelectColumn<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int columnIndex = 0, params DbParameter[]? parameters) => conn.SelectColumn<T>(cmdText, CommandType.Text, paramInfo, parameters, null, columnIndex);

        public static IEnumerable<T> SelectColumn<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, int columnIndex = 0) => conn.SelectColumn<T>(cmdText, commandType, paramInfo, parameters, null, columnIndex);

        public static IEnumerable<T> SelectColumn<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, DbTransaction? transaction, int columnIndex = 0, int commandTimeout = 60)
        {
try
{
            conn.EnsureOpen();
            using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
            {
                using (var reader = command.ExecuteReader())
                {
                    var list = new List<T>();
                    while (reader.Read())
                    {
                        list.Add(reader[columnIndex].To<T>());
                    }
                    return list;
                }
            }
}
finally{
   conn.Close();
}
        }

       public static Task<IEnumerable<T>> SelectColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, int columnIndex = 0, CancellationToken cancellationToken = default)=> conn.SelectColumnAsync<T>(cmdText, null, columnIndex);

       public static Task<IEnumerable<T>> SelectColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int columnIndex = 0, CancellationToken cancellationToken = default)=> conn.SelectColumnAsync<T>(cmdText, CommandType.Text, paramInfo, null, null, columnIndex);


       public static Task<IEnumerable<T>> SelectColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, int columnIndex = 0, CancellationToken cancellationToken = default)=> conn.SelectColumnAsync<T>(cmdText, commandType, paramInfo, null, null, columnIndex, cancellationToken: cancellationToken);

       public static Task<IEnumerable<T>> SelectColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, object? paramInfo, int columnIndex = 0, params DbParameter[]? parameters)=> conn.SelectColumnAsync<T>(cmdText, CommandType.Text, paramInfo, parameters, null, columnIndex);

       public static Task<IEnumerable<T>> SelectColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, int columnIndex = 0, CancellationToken cancellationToken = default) => conn.SelectColumnAsync<T>(cmdText, commandType, paramInfo, parameters, null, columnIndex, cancellationToken: cancellationToken);

       public static async Task<IEnumerable<T>> SelectColumnAsync<T>([NotNull]this DbConnection conn, string cmdText, CommandType commandType, object? paramInfo, DbParameter[]? parameters, DbTransaction? transaction, int columnIndex = 0, int commandTimeout=60, CancellationToken cancellationToken = default)
       {
           await conn.EnsureOpenAsync().ConfigureAwait(false);
try
{
           using (var command = conn.GetDbCommand(cmdText,commandType: commandType, paramInfo: paramInfo, parameters: parameters, transaction: transaction, commandTimeout: commandTimeout))
           {
               using (var reader = command.ExecuteReader())
                {
                    var list = new List<T>();
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        list.Add(reader[columnIndex].To<T>());
                    }

                    return list;
                }
           }
}
finally{
   conn.Close();
}
       }
    }
}
