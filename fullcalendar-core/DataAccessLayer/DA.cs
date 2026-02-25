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
															,event_type
															,is_recurring
															,recurrence_pattern
															,recurrence_interval
															,recurrence_end_date
															,parent_event_id
														from
															Events
														where
															event_start between @start and @end
															OR (is_recurring = 1 AND (recurrence_end_date IS NULL OR recurrence_end_date >= @start))", conn)) {
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
								UserName = dr["user_name"] == DBNull.Value ? null : Convert.ToString(dr["user_name"]),
								EventType = dr["event_type"] == DBNull.Value ? EventType.Meeting : (EventType)Convert.ToInt32(dr["event_type"]),
								IsRecurring = dr["is_recurring"] != DBNull.Value && Convert.ToBoolean(dr["is_recurring"]),
								RecurrencePattern = dr["recurrence_pattern"] == DBNull.Value ? RecurrencePattern.None : (RecurrencePattern)Convert.ToInt32(dr["recurrence_pattern"]),
								RecurrenceInterval = dr["recurrence_interval"] == DBNull.Value ? 1 : Convert.ToInt32(dr["recurrence_interval"]),
								RecurrenceEndDate = dr["recurrence_end_date"] == DBNull.Value ? null : Convert.ToString(dr["recurrence_end_date"]),
								ParentEventId = dr["parent_event_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["parent_event_id"])
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
														,event_type=@eventType
														,is_recurring=@isRecurring
														,recurrence_pattern=@recurrencePattern
														,recurrence_interval=@recurrenceInterval
														,recurrence_end_date=@recurrenceEndDate
													where
														event_id=@eventId", conn, trans);
					cmd.Parameters.AddWithValue("@eventId", evt.EventId);
					cmd.Parameters.AddWithValue("@title", evt.Title);
					cmd.Parameters.AddWithValue("@description", evt.Description);
					cmd.Parameters.AddWithValue("@start", evt.Start);
					cmd.Parameters.AddWithValue("@end", Helpers.ToDBNullOrDefault(evt.End));
					cmd.Parameters.AddWithValue("@allDay", evt.AllDay);
					cmd.Parameters.AddWithValue("@eventType", (int)evt.EventType);
					cmd.Parameters.AddWithValue("@isRecurring", evt.IsRecurring);
					cmd.Parameters.AddWithValue("@recurrencePattern", (int)evt.RecurrencePattern);
					cmd.Parameters.AddWithValue("@recurrenceInterval", evt.RecurrenceInterval);
					cmd.Parameters.AddWithValue("@recurrenceEndDate", Helpers.ToDBNullOrDefault(evt.RecurrenceEndDate));
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
													,event_type
													,is_recurring
													,recurrence_pattern
													,recurrence_interval
													,recurrence_end_date
													,parent_event_id
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
													,@eventType
													,@isRecurring
													,@recurrencePattern
													,@recurrenceInterval
													,@recurrenceEndDate
													,@parentEventId
												);
												select last_insert_rowid()", conn, trans);
				cmd.Parameters.AddWithValue("@title", evt.Title);
				cmd.Parameters.AddWithValue("@description", evt.Description);
				cmd.Parameters.AddWithValue("@start", evt.Start);
				cmd.Parameters.AddWithValue("@end", Helpers.ToDBNullOrDefault(evt.End));
				cmd.Parameters.AddWithValue("@allDay", evt.AllDay);
				cmd.Parameters.AddWithValue("@userId", Helpers.ToDBNullOrDefault(evt.UserId));
				cmd.Parameters.AddWithValue("@userName", Helpers.ToDBNullOrDefault(evt.UserName));
				cmd.Parameters.AddWithValue("@eventType", (int)evt.EventType);
				cmd.Parameters.AddWithValue("@isRecurring", evt.IsRecurring);
				cmd.Parameters.AddWithValue("@recurrencePattern", (int)evt.RecurrencePattern);
				cmd.Parameters.AddWithValue("@recurrenceInterval", evt.RecurrenceInterval);
				cmd.Parameters.AddWithValue("@recurrenceEndDate", Helpers.ToDBNullOrDefault(evt.RecurrenceEndDate));
				cmd.Parameters.AddWithValue("@parentEventId", Helpers.ToDBNullOrDefault(evt.ParentEventId));

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

		public string MoveEvent(int eventId, string start, string end, bool allDay, string currentUserId)
		{
			string message = "";
			SqliteConnection conn = GetConnection();
			SqliteTransaction trans = conn.BeginTransaction();

			try
			{
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
					message = "Je mag alleen je eigen events verplaatsen.";
				}
				else
				{
					SqliteCommand cmd = new SqliteCommand(@"update Events
														set event_start=@start,
															event_end=@end,
															all_day=@allDay
														where event_id=@eventId", conn, trans);
					cmd.Parameters.AddWithValue("@eventId", eventId);
					cmd.Parameters.AddWithValue("@start", start);
					cmd.Parameters.AddWithValue("@end", Helpers.ToDBNullOrDefault(end));
					cmd.Parameters.AddWithValue("@allDay", allDay);
					cmd.ExecuteNonQuery();
				}

				trans.Commit();
			}
			catch (Exception exp)
			{
				trans.Rollback();
				message = exp.Message;
			}
			finally
			{
				CloseConnection(conn);
			}

			return message;
		}

		public string ResizeEvent(int eventId, string start, string end, string currentUserId)
		{
			string message = "";
			SqliteConnection conn = GetConnection();
			SqliteTransaction trans = conn.BeginTransaction();

			try
			{
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
					message = "Je mag alleen je eigen events aanpassen.";
				}
				else
				{
					SqliteCommand cmd = new SqliteCommand(@"update Events
														set event_start=@start,
															event_end=@end
														where event_id=@eventId", conn, trans);
					cmd.Parameters.AddWithValue("@eventId", eventId);
					cmd.Parameters.AddWithValue("@start", start);
					cmd.Parameters.AddWithValue("@end", Helpers.ToDBNullOrDefault(end));
					cmd.ExecuteNonQuery();
				}

				trans.Commit();
			}
			catch (Exception exp)
			{
				trans.Rollback();
				message = exp.Message;
			}
			finally
			{
				CloseConnection(conn);
			}

			return message;
		}

		// Event Sharing Methods
		public string ShareEvent(EventShare share)
		{
			string message = "";
			SqliteConnection conn = GetConnection();
			SqliteTransaction trans = conn.BeginTransaction();

			try
			{
				// Check if already shared
				SqliteCommand checkCmd = new SqliteCommand(@"SELECT COUNT(*) FROM EventShares 
					WHERE event_id=@eventId AND shared_with_user_id=@sharedWithUserId", conn, trans);
				checkCmd.Parameters.AddWithValue("@eventId", share.EventId);
				checkCmd.Parameters.AddWithValue("@sharedWithUserId", share.SharedWithUserId);

				int count = Convert.ToInt32(checkCmd.ExecuteScalar());
				if (count > 0)
				{
					message = "Event is al gedeeld met deze gebruiker.";
				}
				else
				{
					SqliteCommand cmd = new SqliteCommand(@"INSERT INTO EventShares
						(event_id, owner_user_id, shared_with_user_id, shared_with_user_name, can_edit, shared_date)
						VALUES (@eventId, @ownerUserId, @sharedWithUserId, @sharedWithUserName, @canEdit, @sharedDate)", 
						conn, trans);
					cmd.Parameters.AddWithValue("@eventId", share.EventId);
					cmd.Parameters.AddWithValue("@ownerUserId", share.OwnerUserId);
					cmd.Parameters.AddWithValue("@sharedWithUserId", share.SharedWithUserId);
					cmd.Parameters.AddWithValue("@sharedWithUserName", share.SharedWithUserName);
					cmd.Parameters.AddWithValue("@canEdit", share.CanEdit);
					cmd.Parameters.AddWithValue("@sharedDate", share.SharedDate);
					cmd.ExecuteNonQuery();
				}

				trans.Commit();
			}
			catch (Exception exp)
			{
				trans.Rollback();
				message = exp.Message;
			}
			finally
			{
				CloseConnection(conn);
			}

			return message;
		}

		public string UnshareEvent(int eventId, string sharedWithUserId, string currentUserId)
		{
			string message = "";
			SqliteConnection conn = GetConnection();
			SqliteTransaction trans = conn.BeginTransaction();

			try
			{
				// Verify ownership
				SqliteCommand checkCmd = new SqliteCommand(@"SELECT owner_user_id FROM EventShares 
					WHERE event_id=@eventId AND shared_with_user_id=@sharedWithUserId", conn, trans);
				checkCmd.Parameters.AddWithValue("@eventId", eventId);
				checkCmd.Parameters.AddWithValue("@sharedWithUserId", sharedWithUserId);

				var ownerUserId = checkCmd.ExecuteScalar();
				if (ownerUserId == null || Convert.ToString(ownerUserId) != currentUserId)
				{
					message = "Je mag alleen je eigen gedeelde events intrekken.";
				}
				else
				{
					SqliteCommand cmd = new SqliteCommand(@"DELETE FROM EventShares 
						WHERE event_id=@eventId AND shared_with_user_id=@sharedWithUserId", conn, trans);
					cmd.Parameters.AddWithValue("@eventId", eventId);
					cmd.Parameters.AddWithValue("@sharedWithUserId", sharedWithUserId);
					cmd.ExecuteNonQuery();
				}

				trans.Commit();
			}
			catch (Exception exp)
			{
				trans.Rollback();
				message = exp.Message;
			}
			finally
			{
				CloseConnection(conn);
			}

			return message;
		}

		public List<EventShare> GetEventShares(int eventId)
		{
			List<EventShare> shares = new List<EventShare>();

			using (SqliteConnection conn = GetConnection())
			{
				using (SqliteCommand cmd = new SqliteCommand(@"SELECT share_id, event_id, owner_user_id, 
					shared_with_user_id, shared_with_user_name, can_edit, shared_date
					FROM EventShares WHERE event_id=@eventId", conn))
				{
					cmd.Parameters.AddWithValue("@eventId", eventId);

					using (SqliteDataReader dr = cmd.ExecuteReader())
					{
						while (dr.Read())
						{
							shares.Add(new EventShare
							{
								ShareId = Convert.ToInt32(dr["share_id"]),
								EventId = Convert.ToInt32(dr["event_id"]),
								OwnerUserId = Convert.ToString(dr["owner_user_id"]),
								SharedWithUserId = Convert.ToString(dr["shared_with_user_id"]),
								SharedWithUserName = dr["shared_with_user_name"] == DBNull.Value ? null : Convert.ToString(dr["shared_with_user_name"]),
								CanEdit = Convert.ToBoolean(dr["can_edit"]),
								SharedDate = Convert.ToString(dr["shared_date"])
							});
						}
					}
				}
			}

			return shares;
		}

		public List<int> GetSharedEventIds(string userId)
		{
			List<int> eventIds = new List<int>();

			using (SqliteConnection conn = GetConnection())
			{
				using (SqliteCommand cmd = new SqliteCommand(@"SELECT DISTINCT event_id 
					FROM EventShares WHERE shared_with_user_id=@userId", conn))
				{
					cmd.Parameters.AddWithValue("@userId", userId);

					using (SqliteDataReader dr = cmd.ExecuteReader())
					{
						while (dr.Read())
						{
							eventIds.Add(Convert.ToInt32(dr["event_id"]));
						}
					}
				}
			}

			return eventIds;
		}
	}
}
