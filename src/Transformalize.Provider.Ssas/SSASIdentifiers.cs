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

using Transformalize.Context;

namespace Transformalize.Providers.Ssas {

   public class SsasIdentifiers {

      public SsasIdentifiers(InputContext input) {

         Server = input.Connection.Server;
         DatabaseId = input.Connection.Database;
         CubeId = input.Connection.Cube;

         DataSourceId = string.Empty;
         DataSourceViewId = string.Empty;

         VersionId = "Version";
         NormalMeasureGroupId = "Normal";
         DistinctMeasureGroupId = "Distinct";
      }

      public SsasIdentifiers(InputContext input, OutputContext output) {

         Server = output.Connection.Server;
         DatabaseId = output.Connection.Database;
         CubeId = output.Connection.Cube;

         DataSourceId = input.Connection.Name;
         DataSourceViewId = input.Connection.Name;

         VersionId = "Version";
         NormalMeasureGroupId = "Normal";
         DistinctMeasureGroupId = "Distinct";
      }
      public string Server { get; set; }
      public string DatabaseId { get; }
      public string DataSourceId { get; }
      public string DataSourceViewId { get; }
      public string CubeId { get; }
      public string VersionId { get; set; }
      public string NormalMeasureGroupId { get; set; }
      public string DistinctMeasureGroupId { get; set; }
   }
}
