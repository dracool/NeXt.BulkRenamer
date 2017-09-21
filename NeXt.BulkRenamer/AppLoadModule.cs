using Autofac;
using NeXt.BulkRenamer.Models;
using NeXt.BulkRenamer.Models.Background;
using NeXt.BulkRenamer.Models.Parsing;
using NeXt.BulkRenamer.ViewModels;

namespace NeXt.BulkRenamer
{
    internal class AppLoadModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BackgroundEngine>()
                   .As<IBackgroundEngine>()
                   .SingleInstance();

            builder.RegisterType<GrammarReplacementFactory>()
                   .As<IReplacementFactory>()
                   .SingleInstance();

            builder.RegisterType<RenameTargetViewModelFactory>()
                   .As<IRenameTargetViewModelFactory>()
                   .InstancePerDependency();
        }
    }
}
