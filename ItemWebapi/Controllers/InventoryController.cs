using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ItemWebapi.DataAccess;
using ItemWebapi.Models;

namespace ItemWebapi.Controllers
{
    public class InventoryController : ApiController
    {
        SqlConnection con = DataAccessLayer.CreateConnection();

        [HttpGet]
        public List<Inventory> GetInventory()
        {
            DataTable dt = new DataTable();
            List<Inventory> itemList = new List<Inventory>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Inventory", con);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            con.Close();

            foreach (DataRow row in dt.Rows)
            {
                itemList.Add(new Inventory
                {
                    Id = int.Parse(row["Id"].ToString()),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = int.Parse(row["Quantity"].ToString()),
                    Price = decimal.Parse(row["Price"].ToString()),
                    Discount = decimal.Parse(row["Discount"].ToString()),
                    Supplier = row["Supplier"].ToString(),
                   Date = Convert.ToDateTime(row["Date"])
                });
            }

            return itemList;
        }


        [HttpGet]
        public Inventory GetItem(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Inventory WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            Inventory item = null;
            if (reader.Read())
            {
                item = new Inventory
                {
                    Id = (int)reader["Id"],
                    ItemName = reader["ItemName"].ToString(),
                    Quantity = (int)reader["Quantity"],
                    Price = (decimal)reader["Price"],
                    Discount = (decimal)reader["Discount"],
                    Supplier = reader["Supplier"].ToString(),
                    Date = Convert.ToDateTime(reader["DateAdded"])
                };
            }
            con.Close();
            return item;
        }


        [HttpPost]
        public HttpResponseMessage InsertItem([FromBody] Inventory item)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Inventory (ItemName, Quantity, Price, Discount,Supplier) VALUES (@ItemName, @Quantity, @Price, @Discount, @Supplier)", con);
            cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@Discount", item.Discount);
            cmd.Parameters.AddWithValue("@Supplier", item.Supplier);

            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();

            if (result > 0)
                return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            else
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        }


        // Delete

        public HttpResponseMessage DeleteItem(int id)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM Inventory WHERE Id=@Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();

            if (result > 0)
                return new HttpResponseMessage(HttpStatusCode.OK);
            else
                return new HttpResponseMessage(HttpStatusCode.NotFound);
        }



        // update

        [HttpPut]
        public HttpResponseMessage UpdateItem(int id, [FromBody] Inventory inven)
        {
            SqlCommand cmd = new SqlCommand("UPDATE Inventory SET ItemName=@ItemName, Quantity=@Quantity, Price=@Price, Discount=@Discount, Supplier=@Supplier  WHERE Id=@Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@ItemName", inven.ItemName);
            cmd.Parameters.AddWithValue("@Quantity", inven.Quantity);
            cmd.Parameters.AddWithValue("@Price", inven.Price);
            cmd.Parameters.AddWithValue("@Discount", inven.Discount);
            cmd.Parameters.AddWithValue("@Supplier", inven.Supplier);

            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();

            if (result > 0)
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            else
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }



        // search 


        [HttpGet]
        [Route("api/Inventory/Search")]
        public List<Inventory> SearchOrders(string itemName = null, string supplier = null, string status = null)
        {
            List<Inventory> orderList = new List<Inventory>();
            DataTable dt = new DataTable();

            string query = "SELECT * FROM Inventory WHERE 1=1";

            if (!string.IsNullOrEmpty(itemName))
                query += " AND ItemName LIKE @ItemName";
            if (!string.IsNullOrEmpty(supplier))
                query += " AND Supplier LIKE @Supplier";
            if (!string.IsNullOrEmpty(status))
                query += " AND Status LIKE @Status";

            SqlCommand cmd = new SqlCommand(query, con);

            if (!string.IsNullOrEmpty(itemName))
                cmd.Parameters.AddWithValue("@ItemName", "%" + itemName + "%");
            if (!string.IsNullOrEmpty(supplier))
                cmd.Parameters.AddWithValue("@Supplier", "%" + supplier + "%");
            if (!string.IsNullOrEmpty(status))
                cmd.Parameters.AddWithValue("@Status", "%" + status + "%");

            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            con.Close();

            foreach (DataRow row in dt.Rows)
            {
                orderList.Add(new Inventory
                {
                    Id = int.Parse(row["Id"].ToString()),
                    
                    ItemName = row["ItemName"].ToString(),
                    Quantity = int.Parse(row["Quantity"].ToString()),
                    Price = decimal.Parse(row["Price"].ToString()),
                    Discount = decimal.Parse(row["Discount"].ToString()),
                    Supplier = row["Supplier"].ToString(),
                    
                });
            }

            return orderList;
        }
    }

}
