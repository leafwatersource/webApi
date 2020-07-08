using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PmWebApi.Classes.StaticClasses
{
    public static class PmConnections
    {
        public static string Modconnstr { get; set; }
        public static string Schconnstr { get; set; }
        public static string Ctrlconnstr { get; set; }

        public static SqlCommand SchCmd()
        {
            SqlConnection conn = new SqlConnection(Schconnstr);
            SqlCommand cmd = null;
            if (conn.State == ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                    cmd = conn.CreateCommand();
                }
                catch (SqlException e)
                {
                    CErrorMsg error = new CErrorMsg
                    {
                        ErrorName = "Schedule SqlCommand Error",
                        ErrorMessage = e.Message,
                        ErrorTime = DateTime.Now
                    };
                    if (SysMsgList.ErrorList == null)
                    {
                        SysMsgList.ErrorList = new List<CErrorMsg>();
                    }
                    SysMsgList.ErrorList.Add(error);
                }
            }
            return cmd;
        }
        //Modeler 数据库cmd；
        public static SqlCommand ModCmd()
        {
            SqlConnection conn = new SqlConnection(Modconnstr);
            SqlCommand cmd = new SqlCommand();
            if (conn.State == ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                    cmd = conn.CreateCommand();
                }
                catch (SqlException e)
                {
                    CErrorMsg error = new CErrorMsg
                    {
                        ErrorName = "Modeler SqlCommand Error",
                        ErrorMessage = e.Message,
                        ErrorTime = DateTime.Now
                    };
                    if (SysMsgList.ErrorList == null)
                    {
                        SysMsgList.ErrorList = new List<CErrorMsg>();
                    }
                    SysMsgList.ErrorList.Add(error);
                }

            }
            return cmd;
        }
        //Control 数据库cmd；
        public static SqlCommand CtrlCmd()
        {
            SqlConnection conn = new SqlConnection(Ctrlconnstr);
            SqlCommand cmd = null;
            if (conn.State == ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                    cmd = conn.CreateCommand();
                } 
                catch (SqlException e)
                {
                    CErrorMsg error = new CErrorMsg
                    {
                        ErrorName = "PmControl SqlCommand Error",
                        ErrorMessage = e.Message,
                        ErrorTime = DateTime.Now
                    };
                    if (SysMsgList.ErrorList == null)
                    {
                        SysMsgList.ErrorList = new List<CErrorMsg>();
                    }
                    SysMsgList.ErrorList.Add(error);
                }
            }
            return cmd;
        }
    }
}
