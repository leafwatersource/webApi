using System;

namespace PmWebApi.Classes
{
    public class COrderList
    {
        public string MesResName { get; set; }
        public string MesOpName { get; set; }
        public string MesOperator { get; set; }
        public string PlanStartTime { get; set; }
        public string Planendtime { get; set; }
        public string WorkID { get; set; }
        public string PmOpName { get; set; }
        public double FinishedQty { get; set; }
        public int TaskFinishState { get; set; }
        public string ItemAttr1 { get; set; }
        public string ItemAttr2 { get; set; }
        public string ItemAttr3 { get; set; }
        public string ItemAttr4 { get; set; }
        public string ProductID { get; set; }
        public string PmResName { get; set; }
        public double JobQty { get; set; }
        public double Plannedqty { get; set; }
        public double AllFinishedQty { get; set; }
        public double FailedQty { get; set; }
        public double ScrappedQty { get; set; }
        public int DayShift { get; set; }
        public double WorkHours { get; set; }
        public string ItemDesp { get; set; }
        public double SetupTime { get; set; }
        public int OrderUID { get; set; }
        public bool CanReport { get; set; }
        public double CanReportQty { get; set; }
        public double BomComused { get; set; }
        public DateTime  ReportTime { get; set; }
        public string ChangeResName { get; set; }
        public DateTime JobDemandDay { get; set; }
        public bool Ajustment { get; set; }
        public double UnitPrice { get; set; }
        public DateTime MesStartTime { get; set; }
        public DateTime MesEndTime { get; set; }
        public DateTime MesSetupStartTime { get; set; }
        public DateTime MesSetupEndTime { get; set; }
        public string UserComment { get; set; }
    }
}
