using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace CountriesApp.Models
{
    public class SQLTransactionManager
    {
        private static volatile SQLTransactionManager instance = null;
        private static SqlTransaction Transaction;
        private static string connectionInfo = "data source=ecRhin.ec.tec.ac.cr\\Estudiantes;initial catalog=Countries;persist security info=True;" +
            "user id=anobando;password=anobando;MultipleActiveResultSets=True;App=EntityFramework";
        private static SqlConnection connection = new SqlConnection(connectionInfo);

        #region Contructors
        private SQLTransactionManager()
        {
            connection.Open();
            using (var sqlTxn = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    Transaction = sqlTxn;

                    //var sqlCommand = new SqlCommand();
                    //sqlCommand.Connection = conn;
                    //sqlCommand.Transaction = sqlTxn;
                    //sqlCommand.ExecuteNonQuery();

                    //sqlTxn.Commit();
                }
                catch (Exception)
                {
                    sqlTxn.Rollback();
                }
            }
            //using (var conn = new SqlConnection(connectionInfo))
            //{
            //    conn.Open();

            //    using (var sqlTxn = conn.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
            //    {
            //        try
            //        {
            //            Transaction = sqlTxn;

            //            var sqlCommand = new SqlCommand("UPDATE dbo.Person SET idNumber = 112 WHERE idNumber > 971");
            //            sqlCommand.Connection = conn;
            //            sqlCommand.Transaction = sqlTxn;
            //            sqlCommand.ExecuteNonQuery();

            //            sqlTxn.Commit();
            //        }
            //        catch (Exception)
            //        {
            //            sqlTxn.Rollback();
            //        }
            //    }
            //}
        }
        public static SQLTransactionManager Instance()
        {
            if (instance == null)
                instance = new SQLTransactionManager();
            return instance;
        }
        #endregion

        public static void UploadPerson(Person person)
        {
            using (var context = new DbContext(connectionInfo))
            {
                using (var dbContextTransactions = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Database.ExecuteSqlCommand("EXEC dbo.InsertPerson @firstname = '" + person.firstName + "', @lastname = '" + person.lastName + "', @birthcountry = "
                            + person.birthCountry.ToString() + "@birthdate = '" + person.birthdate.Year.ToString() + "-" + person.birthdate.Month.ToString() + "-" +
                            person.birthdate.Day.ToString() + "'");

                        //context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        dbContextTransactions.Rollback();
                    }
                }
            }
            //SqlCommand command = new SqlCommand("dbo.InsertPerson", connection);
            //command.Transaction = Transaction;
            //command.CommandType = System.Data.CommandType.StoredProcedure;
            //command.Parameters.AddWithValue("@firstname", person.firstName);
            //command.Parameters.AddWithValue("@lastname", person.lastName);
            //command.Parameters.AddWithValue("@birthcountry", person.birthCountry);
            //command.Parameters.AddWithValue("@birthdate", person.birthdate);
            //command.ExecuteNonQuery();
        }
    }
}