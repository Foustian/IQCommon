using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQCommon.Model
{
    [Serializable]
    public class IQ_MediaTypeModel
    {
        public string MediaType { get; set; }
        public string SubMediaType { get; set; }
        public string DisplayName { get; set; }
        public short TypeLevel { get; set; }
        public bool HasSubMediaTypes { get; set; }
        public string DataModelType { get; set; }
        public string AnalyticsDataType { get; set; }
        public string DiscChartSearchMethod { get; set; }
        public string DiscResultsSearchMethod { get; set; }
        public string DiscRptGenSearchMethod { get; set; }
        public string DiscExportSearchMethod { get; set; }
        public string FeedsResultView { get; set; }
        public string FeedsChildResultView { get; set; }
        public string DiscoveryResultView { get; set; }
        public string MediaIconPath { get; set; }
        public string EmailMediaIconPath { get; set; }
        public bool UseAudience { get; set; }
        public bool UseMediaValue { get; set; }
        public bool RequireNielsenAccess { get; set; }
        public bool RequireCompeteAccess { get; set; }
        public bool UseFollowers { get; set; }
        public short SortOrder { get; set; }
        public bool IsActiveDiscovery { get; set; }
        public bool IsArchiveOnly { get; set; }
        public bool UseHighlightingText { get; set; }
        public bool HasAccess { get; set; }
        public DashboardData DashboardData { get; set; }
        public string ExternalPlayerUrlSvc { get; set; }

        public AgentType? AgentType { get; set; }
        public string AgentNodeName { get; set; }
        public List<string> SourceTypes { get; set; }
        public string SolrMediaType { get; set; }
       
    }

    [Serializable]
    public class DashboardData
    {
        public List<DataChart> ChartTypes { get; set; }
        public List<DataList> DataLists { get; set; }
        public bool UseCanadaMap { get; set; }
        public bool CheckCanadaSettings { get; set; }
        public string ListSPName { get; set; }
        public string ArchiveListSPName { get; set; }
        public List<DataList> ArchiveDataLists { get; set; }        
    }

    public enum DataChartTypes
    {
        Hits,
        Airing,
        Ad,
        Audience,
        USADMAMap,
        CANADAProvinceMap,
        Sentiment
    }

    [Serializable]
    public class DataList
    {
        public TemplateTypes TemplateType { get; set; }
        public ListTypes ListType { get; set; }
        public string Title { get; set; }
        public string TitleColumn { get; set; }
        public string DataType { get; set; }
    }

    [Serializable]
    public class DataChart
    {
        public DataChartTypes DataChartType { get; set; }
        public string Title { get; set; }
    }

    public enum TemplateTypes
    {
        TVDMA,
        TVStation,
        TVCountry,
        NMDMA,
        Common
    }

    public enum ListTypes
    {
        Country,
        DMA,
        Station
    }

    public enum AgentType
    {
        TV = 1,
        News = 2,
        SocialMedia = 3,
        Twitter = 4,
        Facebook = 5,
        Instagram = 6,
        IQRadio = 7
    }
}
