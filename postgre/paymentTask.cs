using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace QuoteLineBot1
{
    public class PaymentReqObj
    {
        public string id;
        public string create_date;
        public string create_user;
        public string modify_date;
        public string modify_user;
        public string status;
        public string req_date;
        public string user_id;
        public string address;
        public string user_ip;
        public string amount;
        public string start_balance;
        public string end_balance;
        public string comment;

        public PaymentReqObj(
         string id,
         string create_date,
         string create_user,
         string modify_date,
         string modify_user,
         string status,
         string req_date,
         string user_id,
         string address,
         string user_ip,
         string amount,
         string start_balance,
         string end_balance,
         string comment)
        {
            this.id = id;
            this.create_date = create_date;
            this.create_user = create_user;
            this.modify_date = modify_date;
            this.modify_user = modify_user;
            this.status = status;
            this.req_date = req_date;
            this.user_id = user_id;
            this.address = address;
            this.user_ip = user_ip;
            this.amount = amount;
            this.start_balance = start_balance;
            this.end_balance = end_balance;
            this.comment = comment;
        }
    }


    public static class PaymentTask
    {
        static string connstr = @"Server=localhost;Database=USDT_CAP;User Id=postgres;Password=tnfd5503";

        public static List<PaymentReqObj> SelectPendingReq()
        {
            List<PaymentReqObj> paymentReqObjList = new List<PaymentReqObj>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connstr))
            {
                connection.Open();
                string sql = $@"SELECT id, create_date, create_user, modify_date, modify_user, 
                                    status, req_date, user_id, address, user_ip, amount, 
                                    start_balance, end_balance, comment
                                FROM public._payment_req
                                where status = 'pending'
	                        ";               

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string id = reader["id"].ToString();
                        string create_date = reader["create_date"].ToString();
                        string create_user = reader["create_user"].ToString();
                        string modify_date = reader["modify_date"].ToString();
                        string modify_user = reader["modify_user"].ToString();
                        string status = reader["status"].ToString();
                        string req_date = reader["req_date"].ToString();
                        string user_id = reader["user_id"].ToString();
                        string address = reader["address"].ToString();
                        string user_ip = reader["user_ip"].ToString();
                        string amount = reader["amount"].ToString();
                        string start_balance = reader["start_balance"].ToString();
                        string end_balance = reader["end_balance"].ToString();
                        string comment = reader["comment"].ToString();

                        PaymentReqObj paymentReqObj 
                            = new PaymentReqObj(id, create_date, create_user, modify_date, modify_user, 
                            status, req_date, user_id, address, user_ip, amount, 
                            start_balance, end_balance, comment);

                        paymentReqObjList.Add(paymentReqObj);
                    }
                }

                connection.Close();
            }
            return paymentReqObjList;
        }

        public static int Update_payment_req_Status(string id, string status)
        {
            int result = 0;
            using (NpgsqlConnection connection = new NpgsqlConnection(connstr))
            {
                connection.Open();
                string cmdStr = $"update _payment_req set status = '{status}' where id = '{id}'";

                NpgsqlCommand cmd = new NpgsqlCommand(cmdStr, connection);

                result = cmd.ExecuteNonQuery();
                cmd.Dispose();
                connection.Close();
            }

            return result;
        }

        public static int Update_payment_req_end_balance(string id, string end_balance)
        {
            int result = 0;
            using (NpgsqlConnection connection = new NpgsqlConnection(connstr))
            {
                connection.Open();
                string cmdStr = $"update _payment_req set end_balance = '{end_balance}' where id = '{id}'";

                NpgsqlCommand cmd = new NpgsqlCommand(cmdStr, connection);

                result = cmd.ExecuteNonQuery();
                cmd.Dispose();
                connection.Close();
            }

            return result;
        }

    }
}