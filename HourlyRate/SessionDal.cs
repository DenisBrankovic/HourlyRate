using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HourlyRate
{
    static class SessionDal
    {
        public static ObservableCollection<Session> GetAllSessions()
        {
            ObservableCollection<Session> sessionList = new ObservableCollection<Session>();

            using (SqlConnection conn=new SqlConnection(Connection.connPokerResults))
            {
                using (SqlCommand cmnd = new SqlCommand("SELECT * FROM Sessions ORDER BY SessionDate", conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader= cmnd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Session s = new Session();
                                s.SessionId = (int)reader["SessionID"];
                                s.SessionDate = (DateTime)reader["SessionDate"];
                                s.Stakes = reader["Stakes"].ToString();
                                s.HandsPlayed = (int)reader["HandsPlayed"];
                                s.Result = Math.Round((decimal)reader["Result"], 2); 

                                sessionList.Add(s);
                            }
                        }
                        return sessionList;
                    }
                    catch (Exception)
                    {
                        return null; 
                    }
                }
            }
        }
                

        public static int NewSession(Session s)
        {
            using (SqlConnection conn=new SqlConnection(Connection.connPokerResults))
            {
                using (SqlCommand cmnd = new SqlCommand("INSERT INTO Sessions VALUES(@sessionDate, @stakes, @handsPlayed, @result)", conn))
                {
                    try
                    {
                        cmnd.Parameters.AddWithValue("@sessionDate", s.SessionDate);
                        cmnd.Parameters.AddWithValue("@stakes", s.Stakes);
                        cmnd.Parameters.AddWithValue("@handsPlayed", s.HandsPlayed);
                        cmnd.Parameters.AddWithValue("@result", s.Result);

                        conn.Open(); 
                        cmnd.ExecuteScalar();
                        return 1;
                    }
                    catch (Exception)
                    {
                        return 0; 
                    }
                }
            }
        }

        public static int ModifySession(Session s)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("UPDATE Sessions SET SessionDate = @sessionDate, ");
            sb.AppendLine("Stakes = @stakes, HandsPlayed = @handsPlayed, Result = @result ");
            sb.AppendLine("where SessionID = @sessionId"); 
            using (SqlConnection conn=new SqlConnection(Connection.connPokerResults))
            {
                using (SqlCommand cmnd = new SqlCommand(sb.ToString(), conn))
                {
                    cmnd.Parameters.AddWithValue("@sessionDate", s.SessionDate);
                    cmnd.Parameters.AddWithValue("@stakes", s.Stakes);
                    cmnd.Parameters.AddWithValue("@handsPlayed", s.HandsPlayed);
                    cmnd.Parameters.AddWithValue("@result", s.Result);
                    cmnd.Parameters.AddWithValue("@sessionId", s.SessionId);

                    try
                    {
                        conn.Open();
                        cmnd.ExecuteNonQuery();
                        return 1;
                    }
                    catch (Exception)
                    {
                        return 0;                        
                    }
                }
            }
        }

        public static int DeleteSession(Session s)
        {
            using (SqlConnection conn=new SqlConnection(Connection.connPokerResults))
            {
                using (SqlCommand cmnd = new SqlCommand("DELETE FROM Sessions WHERE SessionID = @sessionId", conn))
                {
                    cmnd.Parameters.AddWithValue("@sessionId", s.SessionId);

                    try
                    {
                        conn.Open();
                        cmnd.ExecuteNonQuery();
                        return 1;
                    }
                    catch (Exception)
                    {
                        return 0; 
                    }
                }
            }
        }
    }
}
