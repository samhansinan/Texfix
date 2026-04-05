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
    public class LoginController : ApiController
    {
        SqlConnection con = DataAccessLayer.CreateConnection();

        [HttpGet]
        public List<login> GetItems()
        {
            DataTable dt = new DataTable();
            List<login> itemList = new List<login>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM login", con);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            con.Close();

            foreach (DataRow row in dt.Rows)
            {
                itemList.Add(new login
                {
                    id = int.Parse(row["Id"].ToString()),
                    username = row["username"].ToString(),
                   password = row["password"].ToString(),
                    usertype = row["usertype"].ToString(),
                    email = row["email"].ToString(),
                    contact = row["contact"].ToString()
                });
            }

            return itemList;
        }

        [HttpGet]
        public login GetItem(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM login WHERE id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            login login1 = null;
            if (reader.Read())
            {
                login1 = new login
                {
                    id = (int)reader["id"],
                    username = reader["username"].ToString(),
                    password = reader["password"].ToString()
                    
                };
            }
            con.Close();
            return login1;
        }

        [HttpPost]
        [Route("api/login/validate")]
        public HttpResponseMessage ValidateLogin([FromBody] login login1)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM login WHERE username = @UserName AND password = @Password", con);
            cmd.Parameters.AddWithValue("@UserName", login1.username);
            cmd.Parameters.AddWithValue("@Password", login1.password);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            login user = null;
            if (reader.Read())
            {
                user = new login
                {
                    id = (int)reader["id"],
                    username = reader["username"].ToString(),
                    password = reader["password"].ToString(),
                    usertype = reader["usertype"].ToString()
                };
            }
            con.Close();

            if (user != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }
        [HttpPost]
        public HttpResponseMessage InsertItem([FromBody] login login1)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO login (username, password, usertype, email, contact ) VALUES (@UserName, @Password, @UserType, @email, @contact)", con);
            cmd.Parameters.AddWithValue("@UserName", login1.username);
            cmd.Parameters.AddWithValue("@Password", login1.password);
            cmd.Parameters.AddWithValue("@UserType", login1.usertype);
            cmd.Parameters.AddWithValue("@email", login1.email);
            cmd.Parameters.AddWithValue("@contact", login1.contact);
            

            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();

            if (result > 0)
                return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            else
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        }


        [HttpDelete]

        public HttpResponseMessage DeleteItem(int id)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM login WHERE Id=@Id", con);
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
        [Route("api/login/Search")]
        public List<login> SearchOrders(string username = null, string email = null)
        {
            List<login> orderList = new List<login>();
            DataTable dt = new DataTable();

            string query = "SELECT * FROM login WHERE 1=1"; // Ensures base query

            if (!string.IsNullOrEmpty(username))
                query += " AND username LIKE @username";

            if (!string.IsNullOrEmpty(email))
                query += " AND email LIKE @email";  // Add email condition

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (!string.IsNullOrEmpty(username))
                    cmd.Parameters.AddWithValue("@username", "%" + username + "%");

                if (!string.IsNullOrEmpty(email))
                    cmd.Parameters.AddWithValue("@email", "%" + email + "%");

                con.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                con.Close();
            }

            foreach (DataRow row in dt.Rows)
            {
                orderList.Add(new login
                {
                    id = Convert.ToInt32(row["Id"]),
                    username = row["Username"].ToString(),
                    password = row["Password"].ToString(),
                    usertype = row["Usertype"].ToString(),
                    contact = row["Contact"].ToString()
                });
            }

            return orderList;
        }

    }
}
