using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ItemWebapi.DataAccess;
using ItemWebapi.Models;

namespace ItemWebapi.Controllers
{
    public class ItemController : ApiController
    {
        SqlConnection con = DataAccessLayer.CreateConnection();

        [HttpGet]
        public List<Item> GetItems()
        {
            DataTable dt = new DataTable();
            List<Item> itemList = new List<Item>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Items", con);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            con.Close();

            foreach (DataRow row in dt.Rows)
            {
                itemList.Add(new Item
                {
                    Id = int.Parse(row["Id"].ToString()),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = int.Parse(row["Quantity"].ToString()),
                    Price = decimal.Parse(row["Price"].ToString()),
                    Discount = decimal.Parse(row["Discount"].ToString()),
                    Supplier = row["Supplier"].ToString()
                });
            }

            return itemList;
        }

        [HttpGet]
        public Item GetItem(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Items WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            Item item = null;
            if (reader.Read())
            {
                item = new Item
                {
                    Id = (int)reader["Id"],
                    ItemName = reader["ItemName"].ToString(),
                    Quantity = (int)reader["Quantity"],
                    Price = (decimal)reader["Price"],
                    Discount = (decimal)reader["Discount"],
                    Supplier = reader["Supplier"].ToString()
                };
            }
            con.Close();
            return item;
        }

        [HttpPost]
        public HttpResponseMessage InsertItem([FromBody] Item item)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Items (ItemName, Quantity, Price, Discount,Supplier) VALUES (@ItemName, @Quantity, @Price, @Discount, @Supplier)", con);
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

        // Update an Item
        [HttpPut]
        public HttpResponseMessage UpdateItem(int id, [FromBody] Item item)
        {
            SqlCommand cmd = new SqlCommand("UPDATE Items SET ItemName=@ItemName, Quantity=@Quantity, Price=@Price, Discount=@Discount, Supplier=@Supplier  WHERE Id=@Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@Discount", item.Discount);
            cmd.Parameters.AddWithValue("@Supplier", item.Supplier);

            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();

            if (result > 0)
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            else
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

        [HttpDelete]
       
        public HttpResponseMessage DeleteItem(int id)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM Items WHERE Id=@Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();

            if (result > 0)
                return new HttpResponseMessage(HttpStatusCode.OK);
            else
                return new HttpResponseMessage(HttpStatusCode.NotFound);
        }


        // search 
        [HttpGet]
        [Route("api/Item/Search")]
        public List<Item> SearchOrders(string itemName = null, string supplier = null, string status = null)
        {
            List<Item> orderList = new List<Item>();
            DataTable dt = new DataTable();

            string query = "SELECT * FROM Items WHERE 1=1";

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
                orderList.Add(new Item
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
