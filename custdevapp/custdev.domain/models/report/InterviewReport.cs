

namespace custdev.domain.models.report
{
    public class InterviewReport
    {
        public string ReportId { get; set; }
        public string InterviewDate { get; set; }

        public Respondent Respondent { get; set; }
        public CompanyInfo CompanyInfo { get; set; }

        public List<PainPoint> PainPoints { get; set; }
        public List<CurrentChallenge> CurrentChallenges { get; set; }
        public List<NeedOrWish> NeedsAndWishes { get; set; }

        public SupplierCriteria SupplierCriteria { get; set; }

        public List<MarketOpportunity> MarketOpportunities { get; set; }

        public Conclusions Conclusions { get; set; }
    }

    public class Respondent
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Responsibilities { get; set; }
    }

    public class CompanyInfo
    {
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public List<string> MainProcurementItems { get; set; }
    }

    public class PainPoint
    {
        public string Category { get; set; }
        public string Description { get; set; }
    }

    public class CurrentChallenge
    {
        public string Challenge { get; set; }
        public string Impact { get; set; }
        public string ResolutionActions { get; set; }
    }

    public class NeedOrWish
    {
        public string Need { get; set; }
        public string Priority { get; set; }
        public string Comment { get; set; }
    }

    public class SupplierCriteria
    {
        public string PriceImportance { get; set; }
        public string QualityImportance { get; set; }
        public string EcologyImportance { get; set; }
        public string InnovationImportance { get; set; }
        public string OtherCriteria { get; set; }
    }

    public class MarketOpportunity
    {
        public string Opportunity { get; set; }
        public string Comment { get; set; }
    }

    public class Conclusions
    {
        public List<string> KeyInsights { get; set; }
        public List<string> Recommendations { get; set; }
        public string ClosingRemarks { get; set; }
    }
}
