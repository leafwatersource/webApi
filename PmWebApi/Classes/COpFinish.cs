
namespace PmWebApi.Classes
{
    public class COpFinish
    {
        public string PlanStartTime { get; set; }
        public string PlanEndTime { get; set; }
        public double PlannedHours { get; set; }
        public double PlannedSetupHours { get; set; }
        public string PlanSetupStartTime { get; set; }
        public string PlanSetupEndTime { get; set; }
        public double PlannedQty { get; set; }
        public double FinishedQty { get; set; }
        public double FailedQty { get; set; }
        public string MesStartTime { get; set; }
        public string MesEndTime { get; set; }
        public double MesHours { get; set; }
        public string MesSetupStartTime { get; set; }
        public string MesSetupEndTime { get; set; }
        public double MesSetupHours { get; set; }
    }

    public class CFinishHistory
    {
        public string EventMessage { get; set; }
        public string EventTime { get; set; }
        public double FinishedQty { get; set; }
        public double FailedQty { get; set; }
        public double PlannedQty { get; set; }
        public double JobQty { get; set; }
        public string MesOperator { get; set; }
    }
}
