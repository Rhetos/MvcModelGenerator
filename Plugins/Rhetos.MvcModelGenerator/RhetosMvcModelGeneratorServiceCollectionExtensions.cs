﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace Rhetos.MvcModelGenerator
{
    public static class RhetosMvcModelGeneratorServiceCollectionExtensions
    {
        public static RhetosServiceCollectionBuilder AddMvcModelGenerator(
            this RhetosServiceCollectionBuilder builder,
            Action<MvcModelGeneratorOptions> configureOptions = null)
        {
            builder.Services.AddOptions();
            if (configureOptions != null)
                builder.Services.Configure(configureOptions);

            // HACK: MvcModelGenerator components are not registered here to DI container,
            // instead they are registered in MvcModelGeneratorModuleConfiguration class.
            // This enables backward compatibility: the generated files are generated by default,
            // even if this method (AddMvcModelGenerator) is not called.
            // Developer can call this method to configure and disable the generators
            // (MvcModelGeneratorOptions.GenerateMvcModel, MvcModelGeneratorOptions.GenerateCaptionsResource).

            return builder;
        }
    }
}