using fullcalendarcore.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace fullcalendarcore.DataAccessLayer
{
    public class DA
    {
        public string _ConnectionStrVC { get; set; }

        public DA(string ConnectionStrVC) {
            _ConnectionStrVC = ConnectionStrVC;
        }

        private SqliteConnection GetConnection() {
            SqliteConnection conn = new SqliteConnection(_ConnectionStrVC);
            conn.Open();

            return conn;
        }

        private void CloseConnection(SqliteConnection conn) {
            conn.Close();
        }
        
        public List<Event> GetCalendarEvents(string start, string end) {
            List<Event> events = new List<Event>();

            using (SqliteConnection conn = GetConnection()) {
                using (SqliteCommand cmd = new SqliteCommand(@"select
                                                            event_id
                                                            ,title
                                                            ,description
                                                            ,event_start
                                                            ,event_end
                                                            ,all_day
                                                        from
                                                            Events
                                                        where
                                                            event_start between @start and @end", conn)) {
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@end", end);

                    using (SqliteDataReader dr = cmd.ExecuteReader()) {
                        while (dr.Read()) {
                            events.Add(new Event() {
                                EventId = Convert.ToInt32(dr["event_id"]),
                                Title = Convert.ToString(dr["title"]),
                                Description = Convert.ToString(dr["description"]),
                                Start = Convert.ToString(dr["event_start"]),
                                End = Convert.ToString(dr["event_end"]),
                                AllDay = Convert.ToBoolean(dr["all_day"])
                            });
                        }
                    }
                }
            }

            return events;
        }

		public string UpdateEvent(Event evt) {
			string message = "";
			SqliteConnection conn = GetConnection();
			SqliteTransaction trans = conn.BeginTransaction();

			try {
				SqliteCommand cmd = new SqliteCommand(@"update
													Events
												set
													description=@description
													,title=@title
													,event_start=@start
													,event_end=@end 
													,all_day=@allDay
												where
													event_id=@eventId", conn, trans);
				cmd.Parameters.AddWithValue("@eventId", evt.EventId);
				cmd.Parameters.AddWithValue("@title", evt.Title);
				cmd.Parameters.AddWithValue("@description", evt.Description);
				cmd.Parameters.AddWithValue("@start", evt.Start);
				cmd.Parameters.AddWithValue("@end", Helpers.ToDBNullOrDefault(evt.End));
				cmd.Parameters.AddWithValue("@allDay", evt.AllDay);
				cmd.ExecuteNonQuery();

				trans.Commit();
			} catch (Exception exp) {
				trans.Rollback();
				message = exp.Message;
			} finally {
				CloseConnection(conn);
			}

			return message;
		}

		public string AddEvent(Event evt, out int eventId) {
			string message = "";
			SqliteConnection conn = GetConnection();
			SqliteTransaction trans = conn.BeginTransaction();
			eventId = 0;

			try {
				SqliteCommand cmd = new SqliteCommand(@"insert into Events
												(
													title
													,description
													,event_start
													,event_end
													,all_day
												)
												values
												(
													@title
													,@description
													,@start
													,@end
													,@allDay
												);
												select last_insert_rowid()", conn, trans);
				cmd.Parameters.AddWithValue("@title", evt.Title);
				cmd.Parameters.AddWithValue("@description", evt.Description);
				cmd.Parameters.AddWithValue("@start", evt.Start);
				cmd.Parameters.AddWithValue("@end", Helpers.ToDBNullOrDefault(evt.End));
				cmd.Parameters.AddWithValue("@allDay", evt.AllDay);

				eventId =  Convert.ToInt32(cmd.ExecuteScalar());

				trans.Commit();
			} catch (Exception exp) {
				trans.Rollback();
				message = exp.Message;
			} finally {
				CloseConnection(conn);
			}

			return message;
		}

		public string DeleteEvent(int eventId) {
			string message = "";
			SqliteConnection conn = GetConnection();
			SqliteTransaction trans = conn.BeginTransaction();

			try {
				SqliteCommand cmd = new SqliteCommand(@"delete from 
													Events
												where
													event_id=@eventId", conn, trans);
				cmd.Parameters.AddWithValue("@eventId", eventId);
				cmd.ExecuteNonQuery();

				trans.Commit();
			} catch (Exception exp) {
				trans.Rollback();
				message = exp.Message;
			} finally {
				CloseConnection(conn);
			}

			return message;
		}
    }
}
