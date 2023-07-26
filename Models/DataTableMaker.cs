using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace test.Models
{
    public class DataTableMaker
    {
        public static DataTable MakeDataTable<T> ( List<T> entities )
        {
            if ( entities == null ) return null;

            // Create empty table
            // Will return a table contains entities' columns.
            var result = CreateTable<T>();

            // Fill datas to result
            FillData(result, entities);
            return result;
        }

        private static DataTable CreateTable<T>()
        {
            var result = new DataTable();
            var type = typeof(T);

            // Get T's properties (public & instance) and loop each
            foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                // Get property type, ex : System.String
                var propertyType = property.PropertyType;

                if ((propertyType.IsGenericType) && (propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    propertyType = propertyType.GetGenericArguments()[0];

                // Add column, like a new column 'Address' with type 'System.String'.
                result.Columns.Add(property.Name, propertyType);
            }
            return result;
        }

        private static void FillData<T> (DataTable dt, IEnumerable<T> entities)
        {
            foreach ( var entity in entities )       
                dt.Rows.Add( CreateRow(dt, entity) );
        }

        private static DataRow CreateRow<T> (DataTable dt, T entity) {
            DataRow newrow = dt.NewRow();
            var type = typeof(T);

            foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)) 
                newrow[property.Name] = property.GetValue(entity) ?? DBNull.Value;

            return newrow;
        }
    }
}