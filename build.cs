// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license version 2.0 http://www.apache.org/licenses/LICENSE-2.0

#:project ./src/WeihanLi.Common/WeihanLi.Common.csproj
#:property PublishAot=false

using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

var solutionPath = "./WeihanLi.Common.slnx";
string[] srcProjects = [ 
    "./src/WeihanLi.Common/WeihanLi.Common.csproj",
    "./src/WeihanLi.Common.Logging.Serilog/WeihanLi.Common.Logging.Serilog.csproj",
    "./src/WeihanLi.Extensions.Hosting/WeihanLi.Extensions.Hosting.csproj",
];
string[] testProjects = [ "./test/WeihanLi.Common.Test/WeihanLi.Common.Test.csproj" ];

await DotNetPackageBuildProcess
    .Create(options => 
    {
        options.SolutionPath = solutionPath;
        options.SrcProjects = srcProjects;
        options.TestProjects = testProjects;
    })
    .ExecuteAsync(args);

