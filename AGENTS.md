# Agent guide

Runnable VB.NET examples for the Payam Resan SMS web service. One file per API
method, and every file has to work on its own.

## Rule one: no dependencies, but one project file

An example runs on a stock .NET SDK and nothing else. `HttpClient` and
`System.Text.Json` are both in the base class library, so there is no NuGet
package and nothing to restore beyond the framework itself.

VB is still the one language here that needs a project file. It has no
equivalent of C# file-based apps: `dotnet run some.vb` is not a thing, and the
compiler wants a project for every line it builds. So `run.vbproj` sits in the
root and does exactly one job, compiling the file you name:

```bash
dotnet run -p:Example=examples/v3/account-info.vb
```

Two properties in it are load-bearing. `EnableDefaultCompileItems` is off,
because with it on MSBuild picks up all eleven examples at once and eleven
modules named `Program` collide with BC30179. `RootNamespace` is empty, because
VB prefixes the project name onto every type, which would make the entry point
`run.Program` and leave `StartupObject` pointing at nothing.

The no-dependency rule also rules out helpers from this repository. There is no
shared client class here and there should not be one: a method defined in
`Utils/` means nothing to somebody reading the file on the documentation site.
The examples do not depend on `run.vbproj` either. Each one is a whole module
and drops into any console project unchanged.

## Rule two: Main stays synchronous, and the body stays awaitable

VB has no async entry point. `Main` cannot be `Async`, and `Await` is only legal
inside an `Async` method, so every example is a synchronous `Main` that calls one
async function named after the service method:

```vb
Function Main() As Integer
    Return AccountInfo().GetAwaiter().GetResult()
End Function
```

That shape is deliberate and worth defending. The alternative drops the second
function and blocks at each call site instead, which puts
`GetAwaiter().GetResult()` twice into every POST example. Most VB.NET code in
the world is WinForms or ASP.NET, and in both of those that call deadlocks on
the captured synchronization context. A reader pasting the body into an
`Async Sub Button_Click` should get working code, not a hang.

So `GetAwaiter().GetResult()` appears exactly once per file, at the console
entry point, which is the one place it is safe. Everything below it uses
`Await`.

## Rule three: both Option statements go inside the marked region

Every example opens with these two lines, above the imports:

```vb
Option Strict On
Option Infer On
```

`Option Strict On` is the obvious one. `Option Infer On` is there because plenty
of VB projects, especially ones carried forward from .NET Framework, have infer
turned off at the project level, and under `Option Strict On` that turns every
`Dim x = ...` in these files into a compile error. A file-level `Option`
statement overrides the project setting, so the two lines together make a
snippet that compiles wherever it is pasted.

Both lines sit below `docs:start` for the same reason. The reader on the
documentation site should get a whole compilable file, not a fragment.

## Rule four: the examples are the documentation

Each file carries `' docs:start` and `' docs:end`. The region between them is
lifted verbatim into the method's page on docs.payam-resan.com, so it is read by
people who have never seen this repository.

Two consequences:

- **Full-line comments are stripped** when the region is lifted, and for this
  language the marker is the apostrophe. Anything the reader must see has to be
  code. The `Success` check is an `If`, not a note.
- The file name matches the reference page slug exactly: `send-bulk.vb`,
  `status-by-user-trace-id.vb`. A path with two variants gets two files, the
  plain name for `POST` and a `-get` suffix for `GET`.

File names are flat on purpose. The generator looks for `examples/v3/<slug>.vb`
and cannot express a folder in between, so no example gets its own directory, no
matter how convenient that would be for a project file per example.

The full contract lives in the `handbook` repository, section `docs-site`, file
`code-samples.md`.

## Rule five: check Success, and check it is there at all

The service answers `200` to everything, including a wrong key and an empty
account, so `EnsureSuccessStatusCode` proves nothing and an example that calls
it teaches the wrong thing.

The two-step check is not defensive clutter, and it is the one place these
examples deliberately diverge from the C# ones. Translating `!= true` straight
across is wrong, because comparing against a null in VB yields a null, and `If`
sends a null down the else branch:

```vb
' Wrong: a body with no Success field sails straight through this.
If response?("Success")?.GetValue(Of Boolean)() <> True Then

' Right.
Dim success = response?("Success")
If success Is Nothing OrElse Not success.GetValue(Of Boolean)() Then
    Console.Error.WriteLine($"...")
    Return 1
End If
```

This is not hypothetical. A URL that does not exist answers with a body carrying
only `Message`, which is exactly what the sandbox server returns for
`TokenList`.

## Rule six: build the body with a collection initializer

VB cannot use the C# indexer initializer, but `JsonObject` takes a `From`
initializer and the implicit conversions to `JsonNode` come along for the ride:

```vb
Dim payload As New JsonObject From {
    {"ApiKey", Environment.GetEnvironmentVariable("PAYAM_RESAN_API_KEY")},
    {"Recipients", New JsonArray(
        New JsonObject From {{"Destination", 9121112222L}, {"UserTraceId", 1001L}})}
}
```

An array of primitives is the exception: wrap each value in `JsonValue.Create`,
the way `Ids` and `Parameters` do.

## Rule seven: a version is a folder

A new service version means a new `examples/v<n>/`. No file inside an existing
version folder is moved or renamed; older versions still have users.

## Secrets

The key comes from `PAYAM_RESAN_API_KEY` in the environment. No key, no real
phone number and no customer name goes into a file here, not even a dead one.
Example numbers are `9121112222` upward and the example key is
`123456-XXXXXXXXXXXXXXX`.

## Layout

| Path | What it holds |
|---|---|
| `examples/v3/` | one self-contained file per service operation |
| `run.vbproj` | the runner, and the only file here that is not an example |
| `.env.example` | the environment variables the examples read |

## Before every commit

Run each file. There is no compile-only check worth trusting, because the
failures that matter are the runtime ones.

The workstation has no .NET SDK, so this runs in a container. Save it as a
script rather than pasting it at a prompt:

```bash
#!/bin/sh
# Run every example against the sandbox server.
set -u
export PAYAM_RESAN_API_KEY=123456-XXXXXXXXXXXXXXX
export PAYAM_RESAN_SENDER=30004040
export DOTNET_CLI_HOME=/tmp/home
mkdir -p /tmp/home /tmp/sandbox

for file in /src/examples/v3/*.vb; do
    name=$(basename "$file")
    sed 's#/api/V3/#/api/V3SandBox/#' "$file" > "/tmp/sandbox/$name"
    printf '\n===== %s =====\n' "$name"
    dotnet run --project /src/run.vbproj -p:Example="/tmp/sandbox/$name" -v quiet \
        || echo "FAILED $name"
done
```

```bash
docker run --rm -u "$(id -u):$(id -g)" -e HOME=/tmp/home \
    -v "$PWD":/src -v "$PWD/../scripts":/sp:ro \
    mcr.microsoft.com/dotnet/sdk:10.0 sh /sp/run-examples.sh
```

Two flags there are not decoration. `-u` keeps `bin/` and `obj/` owned by you
rather than by root, and pointing `HOME` outside the mount stops the SDK from
dropping `.dotnet/`, `.nuget/` and `.local/` into the repository.

The `sed` sends everything at `api/V3SandBox/`, so no real message goes out and
no credit is spent. That server answers with fabricated data and ignores the
key, so the example key above is enough to run the whole set. `TokenList` is the
one method it does not implement: it answers `404` there and has to be checked
against the live server instead.

## Git

Semantic messages, `type(scope): subject`, with no explanatory body and no
attribution trailer. Commits here are authored as Payam Resan.
