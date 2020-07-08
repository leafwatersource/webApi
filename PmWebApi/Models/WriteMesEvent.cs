using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Data.SqlClient;

namespace PmWebApi.Models
{
    public class WriteMesEvent
    {
        /// <summary>
        /// 执行写入mes事件队列的方法
        /// </summary>
        /// <param name="edata">api接收到的数据</param>
        /// <param name="writetype">0:开始切换;1:结束切换;2:报工;3:暂停;4:恢复生产;5:切换设备;6:换班</param>
        /// <returns>SQL执行结果bool值</returns>
        public bool WriteMesEvent_Call(COrderList edata,int writetype)
        { 
            SqlCommand cmd = PmConnections.SchCmd();
            int maxUID = PublicFunc.GetMaxUID("S", "wapMesEventRec", "PMUID");
            if (writetype == 0)
            {
                cmd.CommandText = @"INSERT INTO wapMesEventRec 
                (PMUID,EventType,EventName,EventMessage,EventTime,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,MesDate,MesOperator,FailedQty,ScrappedQty,DayShift,PlanQty,OrderUID) VALUES ('" +
                 maxUID + "','S','StartChange','开始切换','" + DateTime.Now + "','" + edata.MesResName + "','" + edata.WorkID + "','" + edata.MesOpName + "','" + edata.ProductID + "','" + edata.ItemDesp +
                 "','" + edata.JobQty + "','" + edata.FinishedQty + "','" + DateTime.Now.Date + "','" + edata.MesOperator + "','" + edata.FailedQty + "','" + edata.ScrappedQty + "','" + edata.DayShift +
                 "','" + edata.Plannedqty + "','" + edata.OrderUID + "')";
            }
            else if(writetype == 1)
            {
                cmd.CommandText = @"INSERT INTO wapMesEventRec 
                (PMUID,EventType,EventName,EventMessage,EventTime,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,MesDate,MesOperator,FailedQty,ScrappedQty,DayShift,PlanQty,OrderUID) VALUES ('" +
                maxUID + "','E','EndChange','结束切换','" + DateTime.Now + "','" + edata.MesResName + "','" + edata.WorkID + "','" + edata.MesOpName + "','" + edata.ProductID + "','" + edata.ItemDesp +
                "','" + edata.JobQty + "','" + edata.FinishedQty + "','" + DateTime.Now.Date + "','" + edata.MesOperator + "','" + edata.FailedQty + "','" + edata.ScrappedQty + "','" + edata.DayShift +
                "','" + edata.Plannedqty + "','" + edata.OrderUID + "')";
            }
            else if( writetype == 2)
            {
                cmd.CommandText = @"INSERT INTO wapMesEventRec 
                (PMUID,EventType,EventName,EventMessage,EventTime,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,MesDate,MesOperator,FailedQty,ScrappedQty,DayShift,PlanQty,OrderUID) VALUES ('" +
                maxUID + "','R','Report','报工','" + DateTime.Now + "','" + edata.MesResName + "','" + edata.WorkID + "','" + edata.MesOpName + "','" + edata.ProductID + "','" + edata.ItemDesp +
                "','" + edata.JobQty + "','" + edata.FinishedQty + "','" + DateTime.Now.Date + "','" + edata.MesOperator + "','" + edata.FailedQty + "','" + edata.ScrappedQty + "','" + edata.DayShift +
                "','" + edata.Plannedqty + "','" + edata.OrderUID + "')";
            }
            else if(writetype == 3)
            {
                cmd.CommandText = @"INSERT INTO wapMesEventRec 
                (PMUID,EventType,EventName,EventMessage,EventTime,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,MesDate,MesOperator,FailedQty,ScrappedQty,DayShift,PlanQty,OrderUID) VALUES ('" +
               maxUID + "','P','Pause','暂停生产','" + DateTime.Now + "','" + edata.MesResName + "','" + edata.WorkID + "','" + edata.MesOpName + "','" + edata.ProductID + "','" + edata.ItemDesp +
               "','" + edata.JobQty + "','" + edata.FinishedQty + "','" + DateTime.Now.Date + "','" + edata.MesOperator + "','" + edata.FailedQty + "','" + edata.ScrappedQty + "','" + edata.DayShift +
               "','" + edata.Plannedqty + "','" + edata.OrderUID + "')";
            }
            else if(writetype == 4)
            {
                cmd.CommandText = @"INSERT INTO wapMesEventRec 
                (PMUID,EventType,EventName,EventMessage,EventTime,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,MesDate,MesOperator,FailedQty,ScrappedQty,DayShift,PlanQty,OrderUID) VALUES ('" +
               maxUID + "','U','Resume','恢复生产','" + DateTime.Now + "','" + edata.MesResName + "','" + edata.WorkID + "','" + edata.MesOpName + "','" + edata.ProductID + "','" + edata.ItemDesp +
               "','" + edata.JobQty + "','" + edata.FinishedQty + "','" + DateTime.Now.Date + "','" + edata.MesOperator + "','" + edata.FailedQty + "','" + edata.ScrappedQty + "','" + edata.DayShift +
               "','" + edata.Plannedqty + "','" + edata.OrderUID + "')";
            }
            else if(writetype == 5)
            {
                cmd.CommandText = @"INSERT INTO wapMesEventRec 
                (PMUID,EventType,EventName,EventMessage,EventTime,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,MesDate,MesOperator,FailedQty,ScrappedQty,DayShift,PlanQty,OrderUID) VALUES ('" +
              maxUID + "','C','ChangeRes','推送订单到=>" + edata.ChangeResName + "','" + DateTime.Now + "','" + edata.MesResName + "','" + edata.WorkID + "','" + edata.MesOpName + "','" + edata.ProductID 
              + "','" + edata.ItemDesp + "','" + edata.JobQty + "','" + edata.FinishedQty + "','" + DateTime.Now.Date + "','" + edata.MesOperator + "','" + edata.FailedQty + "','" + edata.ScrappedQty 
              + "','" + edata.DayShift + "','" + edata.Plannedqty + "','" + edata.OrderUID + "')";
            }
            else if (writetype == 6)
            {
                cmd.CommandText = @"INSERT INTO wapMesEventRec 
                (PMUID,EventType,EventName,EventMessage,EventTime,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,MesDate,MesOperator,FailedQty,ScrappedQty,DayShift,PlanQty,OrderUID) VALUES ('" +
               maxUID + "','EW','EndWorking','换班','" + DateTime.Now + "','" + edata.MesResName + "','" + edata.WorkID + "','" + edata.MesOpName + "','" + edata.ProductID
               + "','" + edata.ItemDesp + "','" + edata.JobQty + "','" + edata.FinishedQty + "','" + DateTime.Now.Date + "','" + edata.MesOperator + "','" + edata.FailedQty + "','" + edata.ScrappedQty
               + "','" + edata.DayShift + "','" + edata.Plannedqty + "','" + edata.OrderUID + "')";
            }
            int result = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            if(result == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
