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
    /// <summary>
    /// The type of command from the <see cref="InterjectRequest"/>.
    /// </summary>
    public enum CommandType
    {
        None = 0,
        StoredProcedure = 1,
        TableDirect = 2,
        Text = 3
    }

    /// <summary>
    /// The sql data type of the parameters from the InterjectRequest
    /// </summary>
    public enum ParameterDataType
    {
        none = 0,
        varchar = 1,
        varcharmax = 2,
        nvarchar = 3,
        nvarcharmax = 4,
        text = 5,
        ntext = 6,
        @char = 7,
        nchar = 8,
        bigint = 9,
        @int = 10,
        smallint = 11,
        tinyint = 12,
        boolean = 13,
        bit = 14,
        money = 15,
        smallmoney = 16,
        @float = 17,
        real = 18,
        single = 19,
        @double = 20,
        @decimal = 21,
        datetime = 22,
        smalldatetime = 23,
        date = 24
    }
}