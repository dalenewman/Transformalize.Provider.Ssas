#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using Microsoft.AnalysisServices.AdomdClient;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;

namespace Transformalize.Providers.Ssas {
   public class SsasReader : IRead {
      readonly InputContext _input;
      readonly IRowFactory _rowFactory;
      public SsasReader(InputContext input) {
         _rowFactory = new RowFactory(input.RowCapacity, input.Entity.IsMaster, false);
         _input = input;
      }
      public IEnumerable<IRow> Read() {
         var ids = new SsasIdentifiers(_input);
         using (var cn = new AdomdConnection($"Data Source={_input.Connection.Server};Catalog={ids.DatabaseId}")) {
            cn.Open();
            using (var cmd = new AdomdCommand(_input.Entity.Query, cn)) {
               using (var reader = cmd.ExecuteReader()) {
                  while (reader.Read()) {
                     var row = _rowFactory.Create();
                     foreach (var field in _input.InputFields) {
                        row[field] = field.Convert(reader[field.Name]);
                     }
                     yield return row;
                  }
               }
            }
         }
      }
   }
}
