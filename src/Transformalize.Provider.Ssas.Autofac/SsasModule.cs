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

using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;
using Transformalize.Providers.Ado;

namespace Transformalize.Providers.Ssas.Autofac {
    public class SsasModule : Module {
        protected override void Load(ContainerBuilder builder) {
            if (!builder.Properties.ContainsKey("Process")) {
                return;
            }

            var process = (Process)builder.Properties["Process"];

            // connections
            foreach (var connection in process.Connections.Where(c => c.Provider == "ssas")) {
                builder.Register<ISchemaReader>(ctx => new NullSchemaReader()).Named<ISchemaReader>(connection.Key);
            }

            // Entity input
            foreach (var entity in process.Entities.Where(e => process.Connections.First(c => c.Name == e.Connection).Provider == "ssas")) {

                // input version detector
                builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

                // input reader
                builder.Register<IRead>(ctx => new NullReader(ctx.ResolveNamed<InputContext>(entity.Key), false)).Named<IRead>(entity.Key);
            }

            if (process.Output().Provider == "ssas") {
                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                foreach (var entity in process.Entities) {
                    builder.Register<IOutputController>(ctx => {
                        var input = ctx.ResolveNamed<InputContext>(entity.Key);
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var factory = ctx.ResolveNamed<IConnectionFactory>(input.Connection.Key);
                        var initializer = process.Mode == "init" ? (IAction)new SsasInitializer(input, output, factory) : new NullInitializer();
                        return new SsasOutputController(
                            output,
                            initializer,
                            ctx.ResolveNamed<IInputProvider>(entity.Key),
                            new SsasOutputProvider(input, output)
                        );
                    }
                    ).Named<IOutputController>(entity.Key);

                    // ENTITY WRITER
                    builder.Register<IWrite>(ctx => new SsasWriter(
                        ctx.ResolveNamed<InputContext>(entity.Key), 
                        ctx.ResolveNamed<OutputContext>(entity.Key)
                    )).Named<IWrite>(entity.Key);
                }
            }

        }
    }
}