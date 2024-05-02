// Copyright 2024 Interject Data Systems

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


using System.Collections.Generic;

namespace Interject.Api
{
    public class InterjectRequestContext
    {

        /// <summary>
        /// The Excel version of the report
        /// </summary>
        public string ExcelVersion { get; set; }

        /// <summary>
        /// The Interject version of the report
        /// </summary>
        public string IdsVersion { get; set; }

        /// <summary>
        /// The file name of the report
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The file path of the report
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The tab name of the report
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// The cell range reference of the Interject function for this request
        /// </summary>
        public string CellRange { get; set; }

        /// <summary>
        /// The text of the Interject function for this request
        /// </summary>
        public string SourceFunction { get; set; }

        /// <summary>
        /// The number of hours the local time differs from the UTC time
        /// </summary>
        public string UtcOffset { get; set; }

        /// <summary>
        /// A list of <see cref="ColDefItems"/>
        /// </summary>
        public List<InterjectColDefItem> ColDefItems { get; set; }

        /// <summary>
        /// A list of <see cref="RowDefItems"/>
        /// </summary>
        public List<InterjectRowDefItem> RowDefItems { get; set; }

        /// <summary>
        /// Contains information pertaining to the user
        /// </summary>
        public IdsUserContext UserContext { get; set; }

        /// <summary>
        /// An encrypted version of the UserContext
        /// </summary>
        public string UserContextEcrypted { get; set; }

        /// <summary>
        /// A table of the data sent from the report to be saved
        /// </summary>
        public IdsTable XmlDataToSave { get; set; }
    }
}