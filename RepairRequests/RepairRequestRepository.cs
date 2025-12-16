using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace RepairRequests
{
        public class RepairRequestRepository
        {
            private readonly string _connectionString;

            public RepairRequestRepository(string connectionString)
            {
                _connectionString = connectionString;
            }

            // Получить все заявки
            public List<Model> GetAllRequests()
            {
                var requests = new List<Model>();

                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM repairs ORDER BY id DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            requests.Add(MapFromReader(reader));
                        }
                    }
                }

                return requests;
            }

        // Получить заявку по ID
        public Model GetById(int id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM repairs WHERE id = @id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapFromReader(reader);
                        }
                    }
                }
            }

            return null;
        }

        // Добавить новую заявку
        public int AddRequest(Model request)
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO repairs 
                                (Наименование, `Описание проблемы`, `Срок выполнения`, Ответственный, Статус) 
                                VALUES (@name, @problem, @deadline, @responsible, @status);
                                SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", request.Name);
                        cmd.Parameters.AddWithValue("@problem", request.Problem);
                        cmd.Parameters.AddWithValue("@deadline", request.Deadline);
                        cmd.Parameters.AddWithValue("@responsible", request.Responsible);
                        cmd.Parameters.AddWithValue("@status", request.Status);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }

            // Обновить статус заявки
            public bool UpdateStatus(int id, string newStatus)
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE repairs SET Статус = @status WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", newStatus);
                        cmd.Parameters.AddWithValue("@id", id);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }

            // Маппинг из DataReader в модель
            private Model MapFromReader(MySqlDataReader reader)
            {
                return new Model
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["Наименование"].ToString(),
                    Problem = reader["Описание проблемы"].ToString(),
                    Deadline = Convert.ToDateTime(reader["Срок выполнения"]),
                    Responsible= reader["Ответственный"].ToString(),
                    Status = reader["Статус"].ToString()
                };
            }
        }
}
