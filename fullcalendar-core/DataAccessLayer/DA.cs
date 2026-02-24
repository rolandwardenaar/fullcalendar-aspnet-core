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
															,user_id
															,user_name
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
								AllDay = Convert.ToBoolean(dr["all_day"]),
								UserId = dr["user_id"] == DBNull.Value ? null : Convert.ToString(dr["user_id"]),
								UserName = dr["user_name"] == DBNull.Value ? null : Convert.ToString(dr["user_name"])
							});
						}
					}
				}
			}

			return events;
		}

		public string UpdateEvent(Event evt, string currentUserId) {
			string message = "";
			SqliteConnection conn = GetConnection();
			SqliteTransaction trans = conn.BeginTransaction();

			try {
				// First check if the event belongs to the current user
				SqliteCommand checkCmd = new SqliteCommand(@"select user_id from Events where event_id=@eventId", conn, trans);
				checkCmd.Parameters.AddWithValue("@eventId", evt.EventId);
				var existingUserId = checkCmd.ExecuteScalar();

				if (existingUserId == null || existingUserId == DBNull.Value)
				{
					message = "Event niet gevonden.";
				}
				else if (Convert.ToString(existingUserId) != currentUserId)
				{
					message = "Je mag alleen je eigen events bewerken.";
				}
				else
				{
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
				}

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
													,user_id
													,user_name
												)
												values
												(
													@title
													,@description
													,@start
													,@end
													,@allDay
													,@userId
													,@userName
												);
												select last_insert_rowid()", conn, trans);
				cmd.Parameters.AddWithValue("@title", evt.Title);
				cmd.Parameters.AddWithValue("@description", evt.Description);
				cmd.Parameters.AddWithValue("@start", evt.Start);
				cmd.Parameters.AddWithValue("@end", Helpers.ToDBNullOrDefault(evt.End));
				cmd.Parameters.AddWithValue("@allDay", evt.AllDay);
				cmd.Parameters.AddWithValue("@userId", Helpers.ToDBNullOrDefault(evt.UserId));
				cmd.Parameters.AddWithValue("@userName", Helpers.ToDBNullOrDefault(evt.UserName));

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

		public string DeleteEvent(int eventId, string currentUserId) {
			string message = "";
			SqliteConnection conn = GetConnection();
			SqliteTransaction trans = conn.BeginTransaction();

			try {
				// First check if the event belongs to the current user
				SqliteCommand checkCmd = new SqliteCommand(@"select user_id from Events where event_id=@eventId", conn, trans);
				checkCmd.Parameters.AddWithValue("@eventId", eventId);
				var existingUserId = checkCmd.ExecuteScalar();

				if (existingUserId == null || existingUserId == DBNull.Value)
				{
					message = "Event niet gevonden.";
				}
				else if (Convert.ToString(existingUserId) != currentUserId)
				{
					message = "Je mag alleen je eigen events verwijderen.";
				}
				else
				{
					SqliteCommand cmd = new SqliteCommand(@"delete from 
														Events
													where
														event_id=@eventId", conn, trans);
					cmd.Parameters.AddWithValue("@eventId", eventId);
					cmd.ExecuteNonQuery();
				}

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
