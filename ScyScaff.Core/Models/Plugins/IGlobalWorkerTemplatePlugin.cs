﻿namespace ScyScaff.Core.Models.Plugins;

public interface IGlobalWorkerTemplatePlugin : ITemplatePlugin
{
    string GlobalWorkerName { get; }
}