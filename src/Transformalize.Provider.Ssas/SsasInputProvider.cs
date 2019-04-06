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

using System;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Ssas {
   public class SsasInputProvider : IInputProvider {
      private readonly InputContext _input;

      public SsasInputProvider(InputContext input) {
         _input = input;
      }
      public object GetMaxVersion() {
         throw new NotImplementedException();
      }

      public Schema GetSchema(Entity entity = null) {
         throw new NotImplementedException();
      }

      public IEnumerable<IRow> Read() {
         return new SsasReader(_input).Read();
      }
   }
}
