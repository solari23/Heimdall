// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

namespace Heimdall.Models.Webhooks;

public class Webhook
{
    public string Id { get; set; }

    public string Name { get; set; }

    public List<IAction> Actions { get; set; }
}
