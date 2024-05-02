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


namespace Interject.Api
{
    public class ReturnedData
    {
        /// <summary>
        /// Serialized instance of <see cref="IdsTable"/>
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Hard coded for reverse compatibility (DataFormat.JsonTableWithSchema)
        /// </summary>
        /// <value>2</value>
        public int DataFormat { get; set; } = 2;

        /// <summary>
        /// Hard code for reverse compatibility (SchemaFormat.Interject_Object)
        /// </summary>
        /// <value>1</value>
        public int SchemaFormat { get; set; } = 1;

        /// <summary>
        /// The schema design for the returned data
        /// </summary>
        public object Schema { get; set; }

        public ReturnedData() { }

        public ReturnedData(object table)
        {
            this.Data = table;
        }
    }
}