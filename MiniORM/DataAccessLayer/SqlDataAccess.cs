
using MiniORM.Entities;
using System.Collections;
using System.Data.SqlClient;
using System.Reflection;

namespace MiniORM.DataAccessLayer
{
    public class SqlDataAccess<T> : ISqlDataAccess<T> where T : class
    {
        public const string ConnectionString =
        "Server = DESKTOP-2FD1OPA\\SQLEXPRESS;" +
            "Initial Catalog = MiniORM; Integrated Security = true";
        public void Delete(int id)
        {
            DeleteById(id, typeof(T));
        }
        private void DeleteById(int id, object? mainClass = null, object? nestedClass = null)
        {
            NestedDeleteById(id, mainClass, nestedClass);
            string sql = "";
            if (mainClass != null)
                sql = $"Delete from {typeof(T).Name} where Id=@Id";
            else
                sql = $"Delete from {nestedClass?.GetType().Name} where Id=@Id";
            // Console.WriteLine(sql + " " + id);
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        private void NestedDeleteById(int id, object? mainClass = null, object? nestedClass = null)
        {
            PropertyInfo?[]? properties = null;
            if (nestedClass == null)
                properties = typeof(T).GetProperties();
            else
                properties = nestedClass.GetType().GetProperties();

            foreach (var property in properties)
            {
                Type propType = property.PropertyType;
                if (propType.IsGenericType)
                    propType = propType.GetGenericTypeDefinition();

                if (propType == typeof(List<>))
                {
                    Type elementType = property.PropertyType.GetGenericArguments()[0];
                    var nestedClassInstance = Activator.CreateInstance(elementType);
                    int idToDelete;
                    if (nestedClass == null)
                    {
                        var count = GetCountId(id, typeof(T).Name, elementType.Name);
                        for (int i = 0; i < count; i++)
                        {
                            idToDelete = GetId(id, typeof(T).Name, elementType.Name);
                            DeleteById(idToDelete, null, nestedClassInstance);
                        }
                    }
                    else
                    {

                        var count = GetCountId(id, nestedClass.GetType().Name, elementType.Name);
                        for (int i = 0; i < count; i++)
                        {
                            idToDelete = GetId(id, nestedClass.GetType().Name, elementType.Name);
                            DeleteById(idToDelete, null, nestedClassInstance);
                        }
                    }

                }
                else if (propType != typeof(int) && !propType.IsEnum &&
                         propType != typeof(string) && propType != typeof(double) &&
                         propType != typeof(DateTime) && propType != typeof(Array) &&
                         propType != typeof(bool) && propType != typeof(List<>) &&
                         propType != typeof(decimal)
                        )
                {
                    var nestedClassInstance = Activator.CreateInstance(propType);

                    int idToDelete;
                    if (nestedClass == null)
                    {
                        var count = GetCountId(id, typeof(T).Name, nestedClassInstance?.GetType().Name);
                        for (int i = 0; i < count; i++)
                        {
                            idToDelete = GetId(id, typeof(T).Name, nestedClassInstance?.GetType().Name);
                            DeleteById(idToDelete, null, nestedClassInstance);
                        }
                    }
                    else
                    {
                        var count = GetCountId(id, nestedClass.GetType().Name, nestedClassInstance?.GetType().Name);
                        for (int i = 0; i < count; i++)
                        {
                            idToDelete = GetId(id, nestedClass.GetType().Name, nestedClassInstance?.GetType().Name);
                            DeleteById(idToDelete, null, nestedClassInstance);
                        }
                    }

                }

            }

        }

        private int GetId(int id, string? mainclass, string? nestedClass, string? propertyName = null, int? offsetValue = -1)
        {
            string sql = "";
            if (propertyName == "PermanentAddress")
                sql = $"Select Id from {nestedClass} where {mainclass}Id=@Id order by (select null) Offset 1 rows";
            else if (offsetValue > 0)
                sql = $"Select Id from {nestedClass} where {mainclass}Id=@Id order by (select null) Offset {offsetValue} rows";
            else
                sql = $"Select Id from {nestedClass} where {mainclass}Id=@Id";
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Id", id);
                int value = (Int32)command.ExecuteScalar();
                return value;
            }
        }
        private int GetCountId(int id, string? mainclass, string? nestedClass)
        {
            var sql = $"Select count(*) from {nestedClass} where {mainclass}Id=@Id";
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Id", id);
                int value = (Int32)command.ExecuteScalar();
                return value;
            }
        }


        public void Delete(T item)
        {
            NestedDelete(item);

            var properties = item.GetType().GetProperties();

            var id = Convert.ToInt32(properties.Where(p => p.Name == "Id").Select(i => i.GetValue(item)).ToList().FirstOrDefault());
            var sql = $"Delete from {item.GetType().Name} where Id=@Id";
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        private void NestedDelete(T item)
        {
            var properties = item.GetType().GetProperties();

            foreach (var val in properties)
            {
                var propertyValue = val.GetValue(item);

                Type propType = val.PropertyType;

                if (propType.IsGenericType)
                    propType = propType.GetGenericTypeDefinition();
                if (propType != typeof(int) && propType != typeof(string) && propType != typeof(double)
                    && propType != typeof(DateTime) && propType != typeof(bool)
                    && propType != typeof(decimal))
                {
                    if (propType == typeof(List<>))
                    {

                        Type genericTypeArgument = val.PropertyType.GetGenericArguments()[0];

                        var list = propertyValue as ICollection;

                        foreach (var it in list)
                            Delete((T)it);

                    }
                    else
                        Delete((T)propertyValue);

                }
            }

        }

        public string GetAll()
        {
            string result = "";
            var properties = typeof(T).GetProperties();

            var columnNames = string.Join(", ", properties.Where(prop => prop.PropertyType == typeof(int))
                                                                .Select(p => p.Name));

            var columns = string.Join(", ", columnNames);

            var sql = $"SELECT {columns} FROM {typeof(T).Name}";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        foreach (var property in properties)
                        {
                            if (property.Name == "Id")
                            {
                                result += GetById((int)reader[property.Name]);
                                result += ",\n";
                            }
                        }

                    }
                }
            }
            return result;
        }

        public string GetById(int id)
        {
            string result = "{";
            GetNestedById(id, ref result, typeof(T), null);
            result += "\n}";
            return result;
        }
        private void GetNestedById(int id, ref string result, object? mainClass = null,
                                   object? nestedClass = null, string? type = null)
        {
            PropertyInfo?[]? properties = null;
            if (nestedClass == null)
                properties = typeof(T).GetProperties();
            else
                properties = nestedClass.GetType().GetProperties();

            bool check = true;
            string currentBracket = "";
            if (check)
            {
                var columnNames = string.Join(", ", properties.Where(prop => prop.PropertyType == typeof(int) ||
                                                    prop.PropertyType == typeof(string) ||
                                                    prop.PropertyType == typeof(double) ||
                                                    prop.PropertyType == typeof(DateTime) ||
                                                    prop.PropertyType == typeof(bool) ||
                                                    prop.PropertyType == typeof(decimal))
                                                   .Select(p => p.Name));

                var columns = string.Join(", ", columnNames);
                if (nestedClass == null)
                {
                    result += "\n";
                    result += "\"" + typeof(T).Name + "\"" + " : " + "{\n";
                    GetValuesFromDB(id, columns, typeof(T).Name, properties, ref result);
                    currentBracket = "{";
                }
                else
                {
                    result += "\n";
                    if (type == "list")
                    {
                        result += "\"" + nestedClass.GetType().Name + "\"" + " : " + "[\n";
                        currentBracket = "[";
                    }
                    else
                    {
                        result += "\"" + nestedClass.GetType().Name + "\"" + " : " + "{\n";
                        currentBracket = "{";
                    }
                    GetValuesFromDB(id, columns, nestedClass.GetType().Name, properties, ref result);

                }
                check = false;
            }

            foreach (var property in properties)
            {
                Type propType = property.PropertyType;
                if (propType.IsGenericType)
                    propType = propType.GetGenericTypeDefinition();

                if (propType == typeof(List<>))
                {
                    Type elementType = property.PropertyType.GetGenericArguments()[0];
                    var nestedClassInstance = Activator.CreateInstance(elementType);
                    int idToRetrieve;
                    if (nestedClass == null)
                    {
                        var count = GetCountId(id, typeof(T).Name, elementType.Name);
                        if (count <= 1)
                        {
                            idToRetrieve = GetId(id, typeof(T).Name, elementType.Name);

                            GetNestedById(idToRetrieve, ref result, null, nestedClassInstance, "list");
                        }
                        else
                        {
                            for (int i = 0; i < count; i++)
                            {
                                idToRetrieve = GetId(id, typeof(T).Name, elementType.Name, null, i);
                                GetNestedById(idToRetrieve, ref result, null, nestedClassInstance, "list");
                            }
                        }
                    }
                    else
                    {
                        var count = GetCountId(id, nestedClass.GetType().Name, elementType.Name);
                        if (count <= 1)
                        {
                            idToRetrieve = GetId(id, nestedClass.GetType().Name, elementType.Name);

                            GetNestedById(idToRetrieve, ref result, null, nestedClassInstance, "list");
                        }
                        else
                        {
                            for (int i = 0; i < count; i++)
                            {

                                idToRetrieve = GetId(id, nestedClass.GetType().Name, elementType.Name, null, i);
                                GetNestedById(idToRetrieve, ref result, null, nestedClassInstance, "list");
                            }
                        }
                    }


                }
                else if (propType != typeof(int) && !propType.IsEnum &&
                         propType != typeof(string) && propType != typeof(double) &&
                         propType != typeof(DateTime) && propType != typeof(Array) &&
                         propType != typeof(bool) && propType != typeof(List<>) &&
                         propType != typeof(decimal)
                        )
                {
                    var nestedClassInstance = Activator.CreateInstance(propType);

                    int idToRetrieve;
                    if (nestedClass == null)
                        idToRetrieve = GetId(id, typeof(T).Name, nestedClassInstance?.GetType().Name, property.Name);
                    else
                        idToRetrieve = GetId(id, nestedClass.GetType().Name, nestedClassInstance?.GetType().Name, property.Name);


                    GetNestedById(idToRetrieve, ref result, null, nestedClassInstance, "object");

                }

            }
            if (currentBracket == "{")
                result += "\n},";
            else
                result += "\n],";
        }

        private void GetValuesFromDB(int id, string columns, string name,
                                     PropertyInfo[] properties, ref string result
                                    )
        {

            var sql = $"SELECT {columns} FROM {name} where Id=@Id";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foreach (var property in properties)
                        {
                            Type propType = property.PropertyType;
                            if (propType == typeof(int) || propType == typeof(decimal) || propType == typeof(string) || propType == typeof(double) ||
                                propType == typeof(DateTime) || propType == typeof(Array) ||
                                propType == typeof(bool) || propType == typeof(double) ||
                                propType == typeof(string)
                                )
                            {
                                result += $"{property.Name} : {reader[property.Name]}";
                                result += ",\n";
                            }
                        }

                    }
                }
            }
        }

        public void Insert(T item)
        {
            InsertToDB(item);
        }
        private void InsertToDB(T item)
        {
            NestedInsertion(item);

            var properties = item.GetType().GetProperties();

            var columns = string.Join(", ", properties.Where(prop => prop.PropertyType == typeof(int) ||
                                            prop.PropertyType == typeof(string) ||
                                            prop.PropertyType == typeof(double) ||
                                            prop.PropertyType == typeof(DateTime) ||
                                            prop.PropertyType == typeof(bool) ||
                                            prop.PropertyType == typeof(decimal))
                                           .Select(p => p.Name));
            var columnParameters = string.Join(", ",
                                            properties.Where(prop => prop.PropertyType == typeof(int) ||
                                            prop.PropertyType == typeof(string) ||
                                            prop.PropertyType == typeof(double) ||
                                            prop.PropertyType == typeof(DateTime) ||
                                            prop.PropertyType == typeof(bool) ||
                                            prop.PropertyType == typeof(decimal))
                                            .Select(p => $"@{p.Name}"));
            var sql = $"INSERT INTO {item.GetType().Name} ({columns}) VALUES ({columnParameters})";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {

                foreach (var property in properties)
                {
                    var propertyValue = property.GetValue(item);
                    Type propType = property.PropertyType;
                    if (propType == typeof(int) || propType == typeof(string) ||
                        propType == typeof(double) || propType == typeof(DateTime) ||
                        propType == typeof(bool) || propType == typeof(decimal))
                    {
                        command.Parameters.AddWithValue($"@{property.Name}", property.GetValue(item));
                    }

                }
                connection.Open();

                command.ExecuteNonQuery();

            }
        }
        private void NestedInsertion(T item)
        {
            var properties = item.GetType().GetProperties();

            foreach (var val in properties)
            {
                var propertyValue = val.GetValue(item);

                Type propType = val.PropertyType;

                if (propType.IsGenericType)
                    propType = propType.GetGenericTypeDefinition();
                if (propType != typeof(int) && propType != typeof(string) && propType != typeof(double)
                    && propType != typeof(DateTime) && propType != typeof(bool)
                    && propType != typeof(decimal))
                {
                    if (propType == typeof(List<>))
                    {

                        Type genericTypeArgument = val.PropertyType.GetGenericArguments()[0];

                        var list = propertyValue as ICollection;

                        foreach (var it in list)
                            InsertToDB((T)it);

                    }
                    else
                        InsertToDB((T)propertyValue);
                }
            }

        }

        public void Update(T item)
        {
            UpdateToDB(item);
        }
        private void UpdateToDB(T item)
        {
            NestedUpdate(item);
            var properties = item.GetType().GetProperties();

            var columnUpdates = properties.Where(prop => (prop.Name != "Id" &&
                                            (prop.PropertyType == typeof(int) ||
                                            prop.PropertyType == typeof(string) ||
                                            prop.PropertyType == typeof(double) ||
                                            prop.PropertyType == typeof(DateTime) ||
                                            prop.PropertyType == typeof(bool) ||
                                            prop.PropertyType == typeof(decimal))
                                            ))
                                          .Select(p => $"{p.Name} = @{p.Name}");
            var columns = string.Join(", ", columnUpdates);

            var columnId = string.Join(", ", properties.Select(p => p.Name).Where(p => p == "Id"));

            var sql = $"UPDATE {item.GetType().Name} SET {columns} WHERE {columnId} = @{columnId}";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                foreach (var property in properties)
                {
                    var propertyValue = property.GetValue(item);
                    Type propType = property.PropertyType;
                    if (propType == typeof(int) || propType == typeof(string) ||
                        propType == typeof(double) || propType == typeof(DateTime) ||
                        propType == typeof(bool) || propType == typeof(decimal))
                    {
                        command.Parameters.AddWithValue($"@{property.Name}", property.GetValue(item));
                    }

                }
                connection.Open();
                command.ExecuteNonQuery();

            }
        }
        private void NestedUpdate(T item)
        {
            var properties = item.GetType().GetProperties();

            foreach (var val in properties)
            {
                var propertyValue = val.GetValue(item);

                Type propType = val.PropertyType;

                if (propType.IsGenericType)
                    propType = propType.GetGenericTypeDefinition();
                if (propType != typeof(int) && propType != typeof(string) && propType != typeof(double)
                    && propType != typeof(DateTime) && propType != typeof(bool)
                    && propType != typeof(decimal))
                {
                    if (propType == typeof(List<>))
                    {

                        Type genericTypeArgument = val.PropertyType.GetGenericArguments()[0];

                        var list = propertyValue as ICollection;

                        foreach (var it in list)
                            UpdateToDB((T)it);

                    }
                    else
                        UpdateToDB((T)propertyValue);

                }
            }

        }
    }

}
