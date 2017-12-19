using System;
using System.Data;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;

namespace LoyaltyViewerWpf {
    class SystemFirebirdClient {
        private FbConnection connection;

		public SystemFirebirdClient(string ipAddress, string baseName, string userName, string password) {
			SystemLogging.LogMessageToFile("Создание подключения к базе FB: " + 
				ipAddress + ":" + baseName);

			FbConnectionStringBuilder cs = new FbConnectionStringBuilder();
            cs.DataSource = ipAddress;
            cs.Database = baseName;
            cs.UserID = userName;
            cs.Password = password;
            cs.Charset = "NONE";
            cs.Pooling = false;

            connection = new FbConnection(cs.ToString());
		}

		public DataTable GetDataTable(string query, Dictionary<string, string> parameters, 
			ref int errorsCountMisDb) {
			DataTable dataTable = new DataTable();

			try {
				connection.Open();
				FbCommand command = new FbCommand(query, connection);
				
				if (parameters.Count > 0)
					foreach (KeyValuePair<string, string> parameter in parameters)
						command.Parameters.AddWithValue(parameter.Key, parameter.Value);

				FbDataAdapter fbDataAdapter = new FbDataAdapter(command);
				fbDataAdapter.Fill(dataTable);
				errorsCountMisDb = 0;
			} catch (Exception e) {
				SystemLogging.LogMessageToFile("Не удалось получить данные, запрос: " + query + 
					Environment.NewLine + e.Message + " @ " + e.StackTrace);
				errorsCountMisDb++;
			} finally {
				connection.Close();
			}

			return dataTable;
		}

		public bool ExecuteUpdateQuery(string query, Dictionary<string, string> parameters) {
			bool updated = false;
			try {
				connection.Open();
				FbCommand update = new FbCommand(query, connection);

				if (parameters.Count > 0) {
					foreach (KeyValuePair<string, string> parameter in parameters)
						update.Parameters.AddWithValue(parameter.Key, parameter.Value);
				}

				updated = update.ExecuteNonQuery() > 0 ? true : false;
			} catch (Exception e) {
				SystemLogging.LogMessageToFile("Не удалось выполнить запрос: " + query + 
					Environment.NewLine + e.Message + " @ " + e.StackTrace);
			} finally {
				connection.Close();
			}

			return updated;
		}
    }
}
