using System.Collections.Generic;

namespace Interject.Api
{
    public class InterjectRequestContext
    {
        public string ExcelVersion { get; set; }
        public string IdsVersion { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string TabName { get; set; }
        public string CellRange { get; set; }
        public string SourceFunction { get; set; }
        public string UtcOffset { get; set; }
        public List<InterjectColDefItem> ColDefItems { get; set; }
        public List<InterjectRowDefItem> RowDefItems { get; set; }
        public IdsUserContext UserContext { get; set; }
        public string UserContextEcrypted { get; set; }
        public IdsTable XmlDataToSave { get; set; }
    }
}