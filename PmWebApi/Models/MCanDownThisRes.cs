using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PmWebApi.Models
{
    public class MCanDownThisRes
    {
        public List<COrderList> CanDownThisRes_Call (string resname,int dayshift)
        {
            SqlCommand cmd = PmConnections.SchCmd();
            cmd.CommandText = "SELECT * FROM User_MesDailyData WHERE  resCanUsed like '%"+ resname.Split(':')[0] +"%' and datatype = 'P' and dailyDate >='" + DateTime.Now + "' and TaskFinishState < '4' ORDER BY workid,productid,pmresname,pmopname,PlanStartTime Desc";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable data = new DataTable();
            da.Fill(data);
            da.Dispose();
            cmd.Connection.Close();

            //获取现有设备上的工单数据
            cmd.CommandText = "SELECT pmresname,pmopname,workid FROM User_MesDailyData WHERE pmresname = '" + resname + "' and datatype ='P'  and taskfinishstate < 4 and dailydate = '" + DateTime.Now.Date + "' and dayshift = '" + dayshift + "'";
            da = new SqlDataAdapter(cmd);
            DataTable dtthisrestasks = new DataTable();
            da.Fill(dtthisrestasks);
            da.Dispose();
            cmd.Connection.Close();

            //对拉取过来的数据去重
            DataTable newdata = data.Clone();
            string thisworkid, preworkid = string.Empty;
            string thisproduct, preproduct = string.Empty;
            string thisresname, preresname = string.Empty;
            string thisopname, preopname = string.Empty;
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if(i == 0)
                {
                    preworkid = data.Rows[i]["workid"].ToString();
                    preproduct = data.Rows[i]["productid"].ToString();
                    preresname = data.Rows[i]["pmresname"].ToString();
                    preopname = data.Rows[i]["pmopname"].ToString();
                    continue;
                }
                else
                {
                    thisworkid = data.Rows[i]["workid"].ToString();
                    thisproduct = data.Rows[i]["productid"].ToString();
                    thisresname = data.Rows[i]["pmresname"].ToString();
                    thisopname = data.Rows[i]["pmopname"].ToString();
                    DataRow[] finddr = dtthisrestasks.Select("pmresname = '" + data.Rows[i]["pmresname"].ToString() + "' and pmopname = '"
                    + data.Rows[i]["pmopname"].ToString() + "' and  workid = '" + data.Rows[i]["workid"].ToString() + "'");
                    if (finddr.Count() > 0)
                    {
                        continue;
                    }
                    if (thisworkid == preworkid)
                    {
                        //工单一样,比较产品是否一样
                        if (thisproduct == preproduct)
                        {
                            //产品一样 比较设备是否一样
                            if (thisresname == preresname)
                            {
                                //如果设备一样,比较工序是否一样
                                if (thisopname == preopname)
                                {
                                    continue;
                                }
                                else
                                {
                                    DataRow dr = newdata.NewRow();
                                    dr.ItemArray = data.Rows[i].ItemArray;
                                    newdata.Rows.Add(dr);
                                }
                            }
                            else
                            {
                                //设备不一样 添加
                                DataRow dr = newdata.NewRow();
                                dr.ItemArray = data.Rows[i].ItemArray;
                                newdata.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            //如果产品不一样,添加
                            DataRow dr = newdata.NewRow();
                            dr.ItemArray = data.Rows[i].ItemArray;
                            newdata.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        //工单不一样,直接添加
                        DataRow dr = newdata.NewRow();
                        dr.ItemArray = data.Rows[i].ItemArray;
                        newdata.Rows.Add(dr);
                    }
                    preworkid = thisworkid;
                    preproduct = thisproduct;
                    preresname = thisresname;
                    preopname = thisopname;
                }
            }
            DataView dv = newdata.DefaultView;
            dv.Sort = "PlanStartTime ASC";
            newdata = dv.ToTable();

            List<COrderList> orderlist = new List<COrderList>();
            if(newdata.Rows.Count > 0)
            {
                foreach (DataRow item in newdata.Rows)
                {
                    DataRow checkeddata = PublicFunc.CheckEmptyVal(data, item);
                    COrderList cOrder = new COrderList()
                    {
                        MesResName = checkeddata["MesResName"].ToString(),
                        MesOpName = checkeddata["MesOpName"].ToString(),
                        MesOperator = checkeddata["bgPerson"].ToString(),
                        WorkID = checkeddata["WorkID"].ToString(),
                        PlanStartTime = PublicFunc.ForMatDateTimeStr(Convert.ToDateTime(checkeddata["PlanStartTime"]), 1),
                        Planendtime = PublicFunc.ForMatDateTimeStr(Convert.ToDateTime(checkeddata["Planendtime"]), 1),
                        PmResName = checkeddata["PmResName"].ToString(),
                        PmOpName = checkeddata["PmOpName"].ToString(),
                        ProductID = checkeddata["ProductID"].ToString(),
                        TaskFinishState = Convert.ToInt32(checkeddata["TaskFinishState"]),
                        FinishedQty = Convert.ToDouble(checkeddata["FinishedQty"]),
                        Plannedqty = Convert.ToDouble(checkeddata["Plannedqty"]),
                        FailedQty = Convert.ToDouble(checkeddata["FailQty"]),
                        AllFinishedQty = Convert.ToDouble(checkeddata["AllFinishedQty"]),
                        JobQty = Convert.ToDouble(checkeddata["JobQty"]),
                        ItemAttr1 = checkeddata["ItemAttr1"].ToString(),
                        ItemAttr2 = checkeddata["ItemAttr2"].ToString(),
                        ItemAttr3 = checkeddata["ItemAttr3"].ToString(),
                        ItemAttr4 = checkeddata["ItemAttr4"].ToString(),
                        DayShift = Convert.ToInt32(checkeddata["DayShift"]),
                        ItemDesp = checkeddata["itemDesp"].ToString(),
                        WorkHours = Convert.ToDouble(checkeddata["workHour"]),
                        SetupTime = Convert.ToDouble(checkeddata["setupTime"]),
                        OrderUID = Convert.ToInt32(checkeddata["UID"]),
                        BomComused = Convert.ToDouble(checkeddata["AllJobTaskqty"]) / Convert.ToDouble(checkeddata["JobQty"]),
                        CanReport = true,
                        CanReportQty = Convert.ToDouble(checkeddata["AllJobTaskqty"]) - Convert.ToDouble(checkeddata["AllFinishedQty"]),
                        ChangeResName = string.Empty
                    };
                    orderlist.Add(cOrder);
                }
                bool hasoutput = false;
                foreach (COrderList c in orderlist)
                {
                    if (c.TaskFinishState != 0 && c.TaskFinishState != 3)
                    {
                        hasoutput = true;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (hasoutput == true)
                {
                    foreach (COrderList c in orderlist)
                    {
                        if (c.TaskFinishState != 0)
                        {
                            c.CanReport = true;
                        }
                        else
                        {
                            c.CanReport = false;
                        }
                    }
                }
            }
            //处理重复的数据 
            return orderlist;
        }

        public bool BeginDown_Call(string orderuid,string resName,string dayshift)
        {
            SqlCommand cmd = PmConnections.SchCmd();
            cmd.CommandText = "UPDATE User_MesDailyData SET pmresname = '" + resName + "', updateDatetime ='" + DateTime.Now + "', bgPerson = '" + PmUser.UserName + "',dayShift = '" + dayshift + "',mesdailydate = '" + DateTime.Now.Date + "' WHERE  UID = '" + orderuid + "'";
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
