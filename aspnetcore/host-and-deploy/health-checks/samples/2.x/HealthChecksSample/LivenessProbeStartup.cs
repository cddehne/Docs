﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SampleApp
{
    // Use the `--scenario liveness` switch to run this version of the sample.
    //
    // Register Health Check Middleware twice at URLs:
    // - /health/ready for 'readiness'
    // - /health/live for 'liveness'
    //
    // The readiness check might do a set of more expensive or time-consuming checks to determine if all other resources are responding. The liveness check performs a quick set of checks to determine if the process is functioning correctly.
    //
    // Using a separate readiness and liveness checks is useful in an environment such as Kubernetes when an app is required to perform significant work before accepting requests. Using separate checks allows the orchestrator to distinguish whether the app is functioning but not yet ready or if the app has failed to start.
    //
    // See https://kubernetes.io/docs/tasks/configure-pod-container/configure-liveness-readiness-probes/ for more details about readiness and liveness probes in Kubernetes.
    //
    // The readiness check runs all registered checks, including a check with a long initialization time (15 seconds). The liveness check uses an 'identity' check that always returns healthy.

    public class LivenessProbeStartup
    {
        #region snippet_ConfigureServices
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck(new SlowDependencyHealthCheck());
        }
        #endregion

        #region snippet_Configure
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHealthChecks("/health/ready");

            app.UseHealthChecks("/health/live", new HealthCheckOptions()
            {
                // Exclude all checks and return a 200-Ok.
                Predicate = (check) => false
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(
                    "Navigate to /health/ready to see the readiness status.");
                await context.Response.WriteAsync(Environment.NewLine);
                await context.Response.WriteAsync(
                    "Navigate to /health/live to see the liveness status.");
            });
        }
        #endregion
    }
}
