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
    public class QuotationController : ApiController
    {
        SqlConnection con = DataAccessLayer.CreateConnection();

        [HttpGet]
        public List<Quotation> GetItems()
        {
            DataTable dt = new DataTable();
            List<Quotation> QuotaionList = new List<Quotation>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Quotation", con);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            con.Close();

            foreach (DataRow row in dt.Rows)
            {
                QuotaionList.Add(new Quotation
                {
                    Id = int.Parse(row["Id"].ToString()),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = int.Parse(row["Quantity"].ToString()),
                    Description = row["Description"].ToString(),
                    Supplier = row["Supplier"].ToString(),
                     Date = Convert.ToDateTime(row["Date"]),
                     Status = row["Status"].ToString(),
                     ResponseInfo = row["ResponseInfo"].ToString(),
                    ResponseStatus = row["ResponseStatus"].ToString()

                });
            }

            return QuotaionList;
        }

        [HttpGet]
        public Quotation GetItem(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Quotation WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            Quotation Quotation = null;
            if (reader.Read())
            {
                Quotation = new Quotation
                {
                    Id = (int)reader["Id"],
                    ItemName = reader["ItemName"].ToString(),
                    Quantity = (int)reader["Quantity"],
                    Supplier = reader["Supplier"].ToString(),
                    Description = reader["Description"].ToString(),
                    
                    Date = Convert.ToDateTime(reader["Date"]),
                    Status = reader["Status"].ToString(),
                    ResponseInfo = reader["ResponseInfo"].ToString(),
                    ResponseStatus = reader["ResponseStatus"].ToString()

                };
            }
            con.Close();
            return Quotation;
        }

        [HttpPost]
        public HttpResponseMessage InsertItem([FromBody] Quotation quotation)
        {
            SqlCommand cmd1 = new SqlCommand("INSERT INTO Quotation (ItemName, Quantity, Supplier, Description) VALUES (@ItemName, @Quantity, @Supplier, @Description)", con);
            cmd1.Parameters.AddWithValue("@ItemName", quotation.ItemName);
            cmd1.Parameters.AddWithValue("@Quantity", quotation.Quantity);
            cmd1.Parameters.AddWithValue("@Supplier", quotation.Supplier);
            cmd1.Parameters.AddWithValue("@Description", quotation.Description);
           

            con.Open();
            int result = cmd1.ExecuteNonQuery();
            con.Close();

            if (result > 0)
                return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            else
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        }

        [HttpDelete]
        public HttpResponseMessage DeleteItem(int id)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM Quotation WHERE Id=@Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();

            if (result > 0)
                return new HttpResponseMessage(HttpStatusCode.OK);
            else
                return new HttpResponseMessage(HttpStatusCode.NotFound);
        }


        //only status
        [HttpPatch]
        [Route("api/Quotation/{Id}/status")]
        public HttpResponseMessage UpdateOrderStatus(int Id, [FromBody] Quotation qoutation)
        {
            using (SqlCommand cmd = new SqlCommand("UPDATE Quotation SET Status=@Status WHERE Id=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@Status", !string.IsNullOrEmpty(qoutation.Status) ? qoutation.Status : "Pending");

                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();

                if (result > 0)
                    return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                else
                    return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
        }
        [HttpPut]
        public HttpResponseMessage UpdateItem(int id, [FromBody] Quotation quotation)
        {
            SqlCommand cmd = new SqlCommand("UPDATE Quotation SET ItemName = @ItemName, Quantity = @Quantity, Supplier = @Supplier, Description = @Description WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@ItemName", quotation.ItemName);
            cmd.Parameters.AddWithValue("@Quantity", quotation.Quantity);
            cmd.Parameters.AddWithValue("@Supplier", quotation.Supplier);
            cmd.Parameters.AddWithValue("@Description", quotation.Description);

            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();

            if (result > 0)
                return new HttpResponseMessage(HttpStatusCode.OK);
            else
                return new HttpResponseMessage(HttpStatusCode.NotFound);
        }


        [HttpPatch]
        [Route("api/Quotation/{Id}/response")]
        public HttpResponseMessage UpdateQuotationResponse(int Id, [FromBody] Quotation quotation)
        {
            using (SqlCommand cmd = new SqlCommand("UPDATE Quotation SET ResponseInfo = @ResponseInfo, ResponseStatus = @ResponseStatus WHERE Id = @Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@ResponseInfo", quotation.ResponseInfo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ResponseStatus", quotation.ResponseStatus ?? (object)DBNull.Value);

                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();

                if (result > 0)
                    return new HttpResponseMessage(HttpStatusCode.OK);
                else
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        // search 


        [HttpGet]
        [Route("api/Quotation/Search")]
        public List<Quotation> SearchOrders(string itemName = null, string supplier = null)
        {
            List<Quotation> orderList = new List<Quotation>();
            DataTable dt = new DataTable();

            string query = "SELECT * FROM Quotation WHERE 1=1";

            if (!string.IsNullOrEmpty(itemName))
                query += " AND ItemName LIKE @ItemName";
            if (!string.IsNullOrEmpty(supplier))
                query += " AND Supplier LIKE @Supplier";
           
            SqlCommand cmd = new SqlCommand(query, con);

            if (!string.IsNullOrEmpty(itemName))
                cmd.Parameters.AddWithValue("@ItemName", "%" + itemName + "%");
            if (!string.IsNullOrEmpty(supplier))
                cmd.Parameters.AddWithValue("@Supplier", "%" + supplier + "%");
           

            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            con.Close();

            foreach (DataRow row in dt.Rows)
            {
                orderList.Add(new Quotation
                {
                    Id = int.Parse(row["Id"].ToString()),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = int.Parse(row["Quantity"].ToString()),        
                    Supplier = row["Supplier"].ToString(),
                    Description = row["Description"].ToString()

                });
            }

            return orderList;
        }
    }
}