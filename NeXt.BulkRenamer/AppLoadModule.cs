using Autofac;
using NeXt.BulkRenamer.Models;
using NeXt.BulkRenamer.ViewModels;

namespace NeXt.BulkRenamer
{
    internal class AppLoadModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TextPatternReplacementFactory>()
                   .As<ITextReplacementFactory>()
                   .SingleInstance();

            builder.RegisterType<BackgroundTextReplacement>()
                   .As<IBackgroundTextReplacement>()
                   .SingleInstance();

            builder.RegisterType<RenameTargetViewModelFactory>()
                   .As<IRenameTargetViewModelFactory>()
                   .InstancePerDependency();
        }
    }
}
