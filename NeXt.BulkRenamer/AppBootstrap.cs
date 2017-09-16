using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using NeXt.BulkRenamer.ViewModels;
using IContainer = Autofac.IContainer;

namespace NeXt.BulkRenamer
{
    internal  class AppBootstrap : BootstrapperBase
    {
        public AppBootstrap()
        {
            Initialize();
        }

        protected IContainer Container;

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }

        protected override void Configure()
        {
            var builder = new ContainerBuilder();

            //  register view models
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                   .Where(type => type.Name.EndsWith("ViewModel") || type.Name.EndsWith("VM"))
                   .Where(type => !string.IsNullOrWhiteSpace(type.Namespace) && type.Namespace.EndsWith("ViewModels"))
                   .Where(type => type.IsAssignableTo<INotifyPropertyChanged>())
                   .AsSelf()
                   .InstancePerDependency();

            //  register views
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                   .Where(type => type.Name.EndsWith("View"))
                   .Where(type => !string.IsNullOrWhiteSpace(type.Namespace) && type.Namespace.EndsWith("Views"))
                   .AsSelf()
                   .InstancePerDependency();
            
            RegisterWindowManager(builder);
            RegisterEventAggregator(builder);

            builder.RegisterModule<AppLoadModule>();

            Container = builder.Build();
        }

        protected virtual void RegisterWindowManager(ContainerBuilder builder)
        {
            builder.Register<IWindowManager>(c => new AppWindowManager())
                   .InstancePerLifetimeScope();
        }

        protected virtual void RegisterEventAggregator(ContainerBuilder builder)
        {
            builder.Register<IEventAggregator>(c => new EventAggregator())
                   .InstancePerLifetimeScope();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (Container.TryResolve(service, out object instance))
                    return instance;
            }
            else
            {
                if (Container.TryResolveNamed(key, service, out object instance))
                    return instance;
            }

            return base.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }
    }
}
