﻿using System;
using System.Diagnostics;
using System.Linq;
using Reloaded.Mod.Launcher.Lib.Utility.Interfaces;
using Reloaded.Mod.Loader.IO.Services;

namespace Reloaded.Mod.Launcher.Lib.Utility;

/// <summary>
/// Class that provides automatic injection to applications with the feature enabled.
/// </summary>
public class AutoInjector
{
    private readonly ApplicationConfigService _configService;
    private readonly IProcessWatcher _processWatcher;

    /* Construction */

    /// <summary/>
    public AutoInjector(ApplicationConfigService configService)
    {
        _configService  = configService;
        _processWatcher = IoC.Get<IProcessWatcher>();
        _processWatcher.OnNewProcess += ProcessWatcherOnOnNewProcess;
    }

    /* Implementation */
    private void ProcessWatcherOnOnNewProcess(Process newProcess)
    {
        try
        {
            string fullPath = newProcess.GetExecutablePath();
            var config = _configService.Items.FirstOrDefault(x => string.Equals(x.Config.AppLocation, fullPath, StringComparison.OrdinalIgnoreCase));
            if (config != null && config.Config.AutoInject)
            {
                var appInjector = new ApplicationInjector(newProcess);
                appInjector.Inject();
            }
        }
        catch (Exception)
        {
            // Ignored
        }
    }
}