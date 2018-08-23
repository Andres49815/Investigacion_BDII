using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;

namespace CountriesApp.Models
{
    public class SQLTransactionManager
    {
        private static volatile SQLTransactionManager instance = null;
        private static CountriesEntities db;
        private static DbContextTransaction Transaction;

        #region Contructor
        public static SQLTransactionManager Instance()
        {
            if (instance == null)
                instance = new SQLTransactionManager();
            return instance;
        }
        private SQLTransactionManager()
        {
            db = new CountriesEntities();
            Transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);
        }
        #endregion

        public static void UploadPerson(Person person)
        {
            db.People.Add(person);
        }

        public static void SendCommit()
        {
            db.SaveChanges();
            Transaction.Commit();
        }

        public static void ResetInstance()
        {
            instance = null;
        }
    }
}

//try
//{
//    context.Database.ExecuteSqlCommand("EXEC dbo.InsertPerson @firstname = '" + person.firstName + "', @lastname = '" + person.lastName + "', @birthcountry = "
//    + person.birthCountry.ToString() + ", @birthdate = '" + person.birthdate.Year.ToString() + "-" + person.birthdate.Month.ToString() + "-" +
//    person.birthdate.Day.ToString() + "'");
//}
//catch (Exception)
//{
//    Transaction.Rollback();
//}

//try
//{
//    context.Database.ExecuteSqlCommand("EXEC dbo.InsertPerson @firstname = '" + person.firstName + "', @lastname = '" + person.lastName + "', @birthcountry = "
//        + person.birthCountry.ToString() + ", @birthdate = '" + person.birthdate.Year.ToString() + "-" + person.birthdate.Month.ToString() + "-" +
//        person.birthdate.Day.ToString() + "'");

//    context.SaveChanges();
//    System.Diagnostics.Debug.Write(":V +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++#####################################################");
//    //Transaction.Commit();
//    //dbContextTransactions.Commit();
//}
//catch (Exception)
//{
//    System.Diagnostics.Debug.Write(".l. :V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V:V");
//    Transaction.Rollback();
//}
//System.Diagnostics.Debug.Write("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");

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

//SqlCommand command = new SqlCommand("dbo.InsertPerson", connection);
//command.Transaction = Transaction;
//command.CommandType = System.Data.CommandType.StoredProcedure;
//command.Parameters.AddWithValue("@firstname", person.firstName);
//command.Parameters.AddWithValue("@lastname", person.lastName);
//command.Parameters.AddWithValue("@birthcountry", person.birthCountry);
//command.Parameters.AddWithValue("@birthdate", person.birthdate);
//command.ExecuteNonQuery();



//SqlCommand command = new SqlCommand("dbo.InsertPerson", connection);
//command.CommandType = System.Data.CommandType.StoredProcedure;
//command.Parameters.AddWithValue("@firstname", person.firstName);
//command.Parameters.AddWithValue("@lastname", person.lastName);
//command.Parameters.AddWithValue("@birthcountry", person.birthCountry);
//command.Parameters.AddWithValue("@birthdate", person.birthdate);
//command.ExecuteNonQuery();
//context = new DbContext(connectionInfo);
//var dbContextTransaction = context.Database.BeginTransaction();

////try
////{
//    int h = await context.Database.ExecuteSqlCommandAsync("EXEC dbo.InsertPerson @firstname = '" + person.firstName + "', @lastname = '" + person.lastName + "', @birthcountry = "
//                + person.birthCountry.ToString() + ", @birthdate = '" + person.birthdate.Year.ToString() + "-" + person.birthdate.Month.ToString() + "-" +
//                person.birthdate.Day.ToString() + "'");
//    //context.SaveChanges();
//    int x = await context.SaveChangesAsync();

//}
//catch (Exception)
//{
//    System.Diagnostics.Debug.Write("Se fue a la gaver");
//    dbContextTransaction.Rollback();
//}
//using (var conn = new SqlConnection(connectionInfo))
//{
//    conn.Open();


//    using (var sqlTxn = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
//    {
//        context = new DbContext(connectionInfo);
//        //try
//        //{
//            var sqlCommand = new SqlCommand();
//            sqlCommand.Connection = conn;
//            sqlCommand.Transaction = sqlTxn;
//            sqlCommand.CommandText = "EXEC dbo.InsertPerson @firstname = '" + person.firstName + "', @lastname = '" + person.lastName + "', @birthcountry = "
//                + person.birthCountry.ToString() + ", @birthdate = '" + person.birthdate.Year.ToString() + "-" + person.birthdate.Month.ToString() + "-" +
//                person.birthdate.Day.ToString() + "'";
//            sqlCommand.ExecuteNonQuery();

//            context.Database.UseTransaction(sqlTxn);
//            context.SaveChanges();

//            sqlTxn.Commit();
//        //}
//        //catch (Exception)
//        //{
//        //    System.Diagnostics.Debug.Write(".|. ----------------------------------------------------------------------------------------------------------");
//        //    sqlTxn.Rollback();
//        //}
//    }
//}

//db.SaveChanges();
//Transaction.Commit();

//SqlConnection connection = new SqlConnection(connectionInfo);
//connection.Open();
//SqlCommand command = new SqlCommand();
//command.CommandText = "EXEC dbo.InsertPerson @firstname = '" + person.firstName + "', @lastname = '" + person.lastName + "', @birthcountry = "
//    + person.birthCountry.ToString() + ", @birthdate = '" + person.birthdate.Year.ToString() + "-" + person.birthdate.Month.ToString() + "-" +
//    person.birthdate.Day.ToString() + "'";
//command.Connection = connection;
//command.ExecuteNonQuery();
//connection.Close();

//connection = new SqlConnection(connectionInfo);
//connection.Open();
//Transaction = context.Database.BeginTransaction();