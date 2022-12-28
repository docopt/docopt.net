// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2021 Atif Aziz, Dinh Doan Van Bien

using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<DocoptNet.Playground.App>("#app");
var baseUrl = new Uri(builder.HostEnvironment.BaseAddress);
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = baseUrl });
await builder.Build().RunAsync();
