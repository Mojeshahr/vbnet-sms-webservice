<div align="center">

<a href="https://payam-resan.com">
  <img src=".github/assets/logo.svg" width="64" height="64" alt="Payam Resan">
</a>

<h1>VB.NET examples for the Payam Resan SMS web service</h1>

Talk to the <a href="https://payam-resan.com"><b>Payam Resan SMS panel</b></a> from VB.NET<br>
One runnable file per API method, with no dependencies

[![API](https://img.shields.io/badge/API-V3-0a7cbd)](https://payam-resan.com)
[![.NET](https://img.shields.io/badge/.NET-10-512bd4)](https://dotnet.microsoft.com)
[![Dependencies](https://img.shields.io/badge/dependencies-none-2ea44f)](#quick-start)
[![License](https://img.shields.io/badge/license-MIT-6e7781)](LICENSE)

<a href="README.md">فارسی</a> · <b>English</b>

</div>

<sub>Looking for another language? The same examples exist for the others at
[github.com/Mojeshahr](https://github.com/Mojeshahr).</sub>

---

## Quick start

```bash
git clone https://github.com/Mojeshahr/vbnet-sms-webservice.git
cd vbnet-sms-webservice

export PAYAM_RESAN_API_KEY='123456-XXXXXXXXXXXXXXX'
export PAYAM_RESAN_SENDER='30004040'

dotnet run -p:Example=examples/v3/account-info.vb
```

No NuGet package is needed. Each example uses only `HttpClient` and
`System.Text.Json`, both part of the framework.

Start with `account-info.vb`. It sends nothing, spends no credit, and if it
answers then both the key and the connection are fine.

## Why there is a project file here

VB has no file-based apps. A `.cs` file can be run directly, but the VB compiler
wants a project for every line it builds. So `run.vbproj` sits in the root, and
its only job is to compile and run whichever single example you name.

The examples do not depend on it. Each one is a complete `Module` and works
unchanged inside any other console or WinForms project.

## Before sending anything real

There is a sandbox server that answers exactly like production but sends no
message and spends no credit. Swap `V3` for `V3SandBox` in the URL. The one
exception is `TokenList`, which the sandbox does not implement.

## The methods

| Example | Method | What it does |
|---|---|---|
| [account-info.vb](examples/v3/account-info.vb) | `AccountInfo` | Credit and active lines |
| [send.vb](examples/v3/send.vb) | `Send` | Simple send over `GET` |
| [send-bulk.vb](examples/v3/send-bulk.vb) | `SendBulk` | One text to many recipients, with tracking ids |
| [send-multiple.vb](examples/v3/send-multiple.vb) | `SendMultiple` | A separate text per recipient |
| [token-list.vb](examples/v3/token-list.vb) | `TokenList` | The account's templates |
| [send-token-single.vb](examples/v3/send-token-single.vb) | `SendTokenSingle` | Send a template to one number |
| [send-token-single-get.vb](examples/v3/send-token-single-get.vb) | `SendTokenSingle` | The same, over `GET` |
| [send-token-multi.vb](examples/v3/send-token-multi.vb) | `SendTokenMulti` | One template, many recipients |
| [status-by-id.vb](examples/v3/status-by-id.vb) | `StatusById` | Status by the service's id |
| [status-by-user-trace-id.vb](examples/v3/status-by-user-trace-id.vb) | `StatusByUserTraceId` | Status by your own id |
| [get-inbox.vb](examples/v3/get-inbox.vb) | `GetInbox` | Messages people sent to your lines |

## On .NET Framework

These are written and tested on .NET 10. The code compiles unchanged on .NET
Framework 4.8 with one difference: `System.Text.Json` is not part of that
framework, so you need its NuGet package there.

## Using this in your own project

Every example is deliberately free of any dependency on this repository, so
copying the body into your own service is enough. In a WinForms app, drop the
body of the async function straight into an `Async Sub Button_Click` and leave
the `Await` calls alone.

If you would rather work with typed classes than with `JsonNode`, change only
the response-reading part; the request shape is the same:

```vb
Public Class SendResult
    Public Property Id As Long
    Public Property UserTraceId As Long?
End Class

Public Class SendResponse
    Public Property Success As Boolean
    Public Property ErrorCode As Integer?
    Public Property [Error] As String
    Public Property Result As SendResult()
End Class
```

The brackets around `[Error]` are required, because `Error` is a VB keyword.

An installable NuGet package is planned and will be published in its own
repository.

## Things that will save you time

**`EnsureSuccessStatusCode` proves nothing here.** The service answers `200` to
everything, including a wrong key. Decide on the `Success` field.

**Write the `Success` check in two steps, not as one comparison.** In VB, a
comparison against a null is itself null, and `If` sends that down the `Else`
branch. So `If response?("Success")?.GetValue(Of Boolean)() <> True` silently
lets through a response that has no `Success` field at all. Test that the field
is there, then test its value, the way every example here does.

**A VB entry point cannot be `Async`.** That is why each example has a
synchronous `Main` calling one async function, with
`GetAwaiter().GetResult()` in that single place. It is harmless in a console
app; the same call inside WinForms or ASP.NET deadlocks, so do not move it into
the body.

**Keep the two `Option` lines at the top of the file.** Many VB projects,
especially ones carried over from .NET Framework, have `Option Infer` turned
off, and without those lines every `Dim x = ...` in these files fails to
compile.

**Recipient numbers carry no leading zero.** Use `9121112222`, or
`989121112222` with the country code. A number that does not start with `9` or
`989` returns error code `13`.

**Do not encode the text twice.** In `send.vb`, `FormUrlEncodedContent` already
does it once. Call `UrlEncode` beforehand and the message arrives full of
`%D8`.

**Send a unique `UserTraceId` per recipient.** After a timeout it is the only
way to learn whether the message was registered.

## Key safety

The key is a secret. It does not belong in a code repository, in browser
JavaScript, or in a mobile app bundle. It belongs in an environment variable,
which is where every example here reads it from.

If a key leaks, issue a new one from the panel. A deleted key never comes back.

## Layout

| Path | What it holds |
|---|---|
| `examples/v3/` | One self-contained example per service operation |
| `run.vbproj` | The runner, and the only file here that is not an example |
| `.env.example` | The environment variables the examples read |

The `v3` in the path is deliberate. A new service version means a new
`examples/v<n>/`, with the existing folder left alone.

## Documentation and support

The full guide is at [docs.payam-resan.com](https://docs.payam-resan.com). The
machine-readable OpenAPI description is in
[sms-webservice-spec](https://github.com/Mojeshahr/sms-webservice-spec).

## License

MIT. Full text in [`LICENSE`](LICENSE).
