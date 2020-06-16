using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public decimal FinishedQty { get; set; }
        public int TaskFinishState { get; set; }
        public string ItemAttr1 { get; set; }
        public string ItemAttr2 { get; set; }
        public string ItemAttr3 { get; set; }
        public string ItemAttr4 { get; set; }
        public string ProductID { get; set; }
        public string PmResName { get; set; }
        public decimal JobQty { get; set; }
        public decimal Plannedqty { get; set; }
        public decimal AllFinishedQty { get; set; }
        public decimal FailedQty { get; set; }
        public decimal ScrappedQty { get; set; }
        public int DayShift { get; set; }
        public decimal WorkHours { get; set; }
        public string ItemDesp { get; set; }
        public decimal SetupTime { get; set; }
        public int OrderUID { get; set; }
        public bool CanReport { get; set; }
        public decimal CanReportQty { get; set; }
        public decimal BomComused { get; set; }
        public DateTime  ReportTime { get; set; }
        public string ChangeResName { get; set; }
    }
}
