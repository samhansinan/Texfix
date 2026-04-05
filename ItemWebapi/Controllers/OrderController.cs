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
    public class OrderController : ApiController
    {
        SqlConnection con = DataAccessLayer.CreateConnection();

        [HttpGet]
        public List<Order> GetOrder()
        {
            DataTable dt = new DataTable();
            List<Order> OrderList = new List<Order>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Orders", con);

            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            con.Close();

            foreach (DataRow row in dt.Rows)
            {
                OrderList.Add(new Order
                {
                    Id = int.Parse(row["Id"].ToString()),
                    ItemId = int.Parse(row["ItemId"].ToString()),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = int.Parse(row["Quantity"].ToString()),
                    Supplier = row["Supplier"].ToString(),
                    Status = row["Status"].ToString(),
                    Date = Convert.ToDateTime(row["Date"])

                });
            }

            return OrderList;
        }


        [HttpGet]
        public Order GetOrder(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Orders WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            Order order = null;
            if (reader.Read())
            {
                order = new Order
                {
                    Id = (int)reader["Id"],
                    ItemId = (int)reader["ItemId"],
                    ItemName = reader["ItemName"].ToString(),
                    Quantity = (int)reader["Quantity"],
                    Supplier = reader["Supplier"].ToString(),
                    Status = reader["Status"].ToString(),
                    Date = Convert.ToDateTime(reader["Date"])

                };
            }
            con.Close();
            return order;
        }


        //intert of orders

        [HttpPost]
        public HttpResponseMessage InsertItem([FromBody] Order orders)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Orders (ItemId, ItemName, Quantity, Supplier, Status) VALUES (@ItemId, @ItemName, @Quantity, @Supplier, @Status)", con);
            cmd.Parameters.AddWithValue("@ItemId", orders.ItemId);
            cmd.Parameters.AddWithValue("@ItemName", orders.ItemName);
            cmd.Parameters.AddWithValue("@Quantity", orders.Quantity);
            cmd.Parameters.AddWithValue("@Supplier", orders.Supplier);
            cmd.Parameters.AddWithValue("@Status", orders.Status);

            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();

            if (result > 0)
                return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            else
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        }



        [HttpPut]
        public HttpResponseMessage UpdateItem(int Id, [FromBody] Order order)
        {
            Console.WriteLine("Received Status: " + order.Status);
            using (SqlCommand cmd = new SqlCommand("UPDATE Orders SET ItemId=@ItemId, ItemName=@ItemName, Quantity=@Quantity, Supplier=@Supplier, Status=@Status WHERE Id=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@ItemId", order.ItemId);
                cmd.Parameters.AddWithValue("@ItemName", order.ItemName);
                cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                cmd.Parameters.AddWithValue("@Supplier", order.Supplier);
                cmd.Parameters.AddWithValue("@Status", !string.IsNullOrEmpty(order.Status) ? order.Status : "Pending");

                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();

                if (result > 0)
                    return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                else
                    return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
        }


        //only status
        [HttpPatch]
        [Route("api/Order/{Id}/status")]
        public HttpResponseMessage UpdateOrderStatus(int Id, [FromBody] Order order)
        {
            using (SqlCommand cmd = new SqlCommand("UPDATE Orders SET Status=@Status WHERE Id=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@Status", !string.IsNullOrEmpty(order.Status) ? order.Status : "Pending");

                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();

                if (result > 0)
                    return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                else
                    return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
        }


        [HttpDelete]

        public HttpResponseMessage DeleteOrder(int id)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM Orders WHERE Id=@Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();

            if (result > 0)
                return new HttpResponseMessage(HttpStatusCode.OK);
            else
                return new HttpResponseMessage(HttpStatusCode.NotFound);
        }


        [HttpGet]
        [Route("api/Order/Search")]
        public List<Order> SearchOrders(string itemName = null, string supplier = null, string status = null)
        {
            List<Order> orderList = new List<Order>();
            DataTable dt = new DataTable();

            string query = "SELECT * FROM Orders WHERE 1=1";

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
                orderList.Add(new Order
                {
                    Id = int.Parse(row["Id"].ToString()),
                    ItemId = int.Parse(row["ItemId"].ToString()),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = int.Parse(row["Quantity"].ToString()),
                    Supplier = row["Supplier"].ToString(),
                    Status = row["Status"].ToString()
                });
            }

            return orderList;
        }


    }
}
