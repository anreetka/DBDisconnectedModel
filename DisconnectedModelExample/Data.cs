using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DisconnectedModelExample
{
    class Data
    {
        SqlConnection conn;
        SqlDataAdapter adapter;
        DataSet ds;
        DataTable dataTable;
        SqlCommandBuilder cmdBuilder;

        string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=NorthWind;Integrated Security=True";

        public string ConnStr { get { return connStr; } }

        public Data()
        {
            conn = new SqlConnection(ConnStr);
            string query = "SELECT ProductID, ProductName, UnitPrice, UnitsInStock FROM Products";
            adapter = new SqlDataAdapter(query, conn);
            cmdBuilder = new SqlCommandBuilder(adapter);
            FillDataSet();
        }

        public void FillDataSet()
        {
            ds = new DataSet();
            adapter.Fill(ds);
            dataTable= ds.Tables[0];

            //define primary key for data table because data set cannot see columns so it doesn't give us a primary key
            DataColumn[] pk = new DataColumn[1];
            pk[0] = dataTable.Columns["ProductID"];
            pk[0].AutoIncrement = true; //set autoincrement to true so that it can increase automatically

            dataTable.PrimaryKey = pk;  //set the primary key equal to pl
        }

        public DataTable LoadDataTable()
        {
            FillDataSet();
            return dataTable;
        }

        public DataRow FindProductById(int id)
        {
            DataRow row = null;
            row = dataTable.Rows.Find(id);
            return row;

        }

        public DataTable InsertProduct(String name, decimal price, short units)
        {
            DataRow row = dataTable.NewRow();  //new empty row created
            row["ProductName"] = name;  //add values to new Row 
            row["UnitPrice"] = price;
            row["UnitsInStock"] = units;

            dataTable.Rows.Add(row);   //add row to the Product Table
 

            adapter.InsertCommand = cmdBuilder.GetInsertCommand();  //insert query
            adapter.Update(dataTable); //update original database using Update
            return dataTable;
        }

        public DataTable DeleteProductById(int id)
        {
            DataRow productRow = FindProductById(id);
            if(productRow != null)
            {
                productRow.Delete();
                MessageBox.Show("Row deleted!");
            }
            else
            {
                MessageBox.Show("id doesn't exist");
            }

            //you can only modify the data you added due to dependencies with other tables
            adapter.DeleteCommand = cmdBuilder.GetDeleteCommand();
            adapter.Update(dataTable);
            return dataTable;
        }
    }
}
