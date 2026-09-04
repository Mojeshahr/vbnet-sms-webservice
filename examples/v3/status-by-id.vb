' StatusById - وضعیت پیامک با شناسه‌هایی که متد ارسال برگردانده است.
'
' دسته‌ای بپرسید، نه یکی‌یکی. فاصله استعلام‌ها را هم کمتر از چند دقیقه
' نگذارید، وگرنه به خطای ۲۰ می‌خورید.
'
' جز کتابخانه استاندارد دات‌نت به چیزی وابسته نیست. کپی کنید و در پروژه
' خودتان اجرا کنید.
'
'   PAYAM_RESAN_API_KEY=... dotnet run -p:Example=examples/v3/status-by-id.vb

' docs:start
Option Strict On
Option Infer On

Imports System.Collections.Generic
Imports System.Net.Http
Imports System.Text
Imports System.Text.Json.Nodes
Imports System.Threading.Tasks

Module Program

    Function Main() As Integer
        Return StatusById().GetAwaiter().GetResult()
    End Function

    Async Function StatusById() As Task(Of Integer)
        Dim payload As New JsonObject From {
            {"ApiKey", Environment.GetEnvironmentVariable("PAYAM_RESAN_API_KEY")},
            {"Ids", New JsonArray(JsonValue.Create(9903211L), JsonValue.Create(9903212L))}
        }

        Using http As New HttpClient With {.Timeout = TimeSpan.FromSeconds(30)}
            Dim body As New StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
            Dim answer = Await http.PostAsync("https://api.sms-webservice.com/api/V3/StatusById", body)
            Dim response = JsonNode.Parse(Await answer.Content.ReadAsStringAsync())

            Dim success = response?("Success")
            If success Is Nothing OrElse Not success.GetValue(Of Boolean)() Then
                Console.Error.WriteLine($"ناموفق. کد {response?("ErrorCode")}: {response?("Error")}")
                Return 1
            End If

            ' شرط را روی StatusCode بگذارید، نه روی متن Status. این پنج کد یعنی
            ' هنوز در راه است و باید بعداً دوباره استعلام کنید، نه اینکه دوباره
            ' بفرستید.
            Dim pending As New HashSet(Of Integer) From {0, 1, 2, 3, 10}

            For Each message In response("Result").AsArray()
                Dim code = message("StatusCode").GetValue(Of Integer)()
                Dim again = If(pending.Contains(code), " (بعداً دوباره بپرسید)", "")
                Console.WriteLine($"{message("Id")}: {message("Status")}{again}")
            Next
        End Using

        Return 0
    End Function

End Module
' docs:end
