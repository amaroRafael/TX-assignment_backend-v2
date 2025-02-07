namespace TX.RMC.UnitTests.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Models;

internal class DataRepository<T> where T : class, new()
{
    /// <summary>
    /// The DataTable that will hold the data
    /// </summary>
    protected DataTable _dataTable;

    public DataRepository()
    {
        // Create the DataTable
        this._dataTable = new DataTable();

        // Add the columns to the DataTable
        foreach (var propertyInfo in typeof(T).GetProperties())
        {
            Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
            DataColumn column = new DataColumn(propertyInfo.Name, propertyType);

            this._dataTable.Columns.Add(column);
        }
    }

    /// <summary>
    /// Gets the model by the Id
    /// </summary>
    /// <param name="id">Model identity</param>
    /// <returns>Returns model retrieved from database</returns>
    protected T? GetById(Guid id)
    {
        // Create a new instance of the model
        T model = new T();

        // Gets the DataRow from the DataTable
        var dataRow = (from rows in this._dataTable.AsEnumerable()
                       where rows.Field<Guid>("Id") == id
                       select rows)
                        .SingleOrDefault();

        if (dataRow == null)
        {
            return null;
        }

        // Populate the model with the data from the DataRow
        PopulateModel(model, dataRow);

        // Return the model
        return model;
    }

    /// <summary>
    /// Adds the model to the DataTable
    /// </summary>
    protected T Add(T model)
    {
        // Creates a new DataRow
        var dataRow = this._dataTable.NewRow();

        // Loop through the properties of the model and set the value in the DataRow
        foreach (var propertyInfo in typeof(T).GetProperties())
        {
            // If the property is the Id, set the value to a new Guid
            if (propertyInfo.Name == "Id")
            {
                propertyInfo.SetValue(model, Guid.NewGuid());
            }

            var propertyValue = propertyInfo.GetValue(model);

            // Set the value in the DataRow
            dataRow[propertyInfo.Name] = propertyValue ?? DBNull.Value;
        }

        // Add the DataRow to the DataTable
        this._dataTable.Rows.Add(dataRow);
        // Accept the changes
        this._dataTable.AcceptChanges();

        // Return the model
        return model;
    }

    /// <summary>
    /// Populates the model with the data from the DataRow
    /// </summary>
    protected static void PopulateModel(T model, DataRow? dataRow)
    {
        // If the DataRow is not null, populate the model
        if (dataRow != null)
        {
            // Loop through the properties of the model and set the value from the DataRow
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var dataRowValue = dataRow[propertyInfo.Name];
                if (dataRowValue != DBNull.Value) propertyInfo.SetValue(model, dataRowValue);
            }
        }
    }
}
