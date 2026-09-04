' TokenList - قالب‌های حساب، با کلید و متن و وضعیت تأییدشان.
'
' برای پیدا کردن TemplateKey که متدهای ارسال قالب لازم دارند. این متد هم مثل
' AccountInfo از بررسی اعتبار معاف است.
'
' روی سرور آزمایشی پیاده نشده و ۴۰۴ می‌دهد؛ همین متد را از سرور عملیاتی صدا
' بزنید، چیزی نمی‌فرستد و اعتباری مصرف نمی‌کند.
'
' جز کتابخانه استاندارد دات‌نت به چیزی وابسته نیست. کپی کنید و در پروژه
' خودتان اجرا کنید.
'
'   PAYAM_RESAN_API_KEY=... dotnet run -p:Example=examples/v3/token-list.vb

' docs:start
Option Strict On
Option Infer On

Imports System.Net.Http
Imports System.Text
Imports System.Text.Json.Nodes
Imports System.Threading.Tasks

Module Program

    Function Main() As Integer
        Return TokenList().GetAwaiter().GetResult()
    End Function

    Async Function TokenList() As Task(Of Integer)
        Dim payload As New JsonObject From {
            {"ApiKey", Environment.GetEnvironmentVariable("PAYAM_RESAN_API_KEY")}
        }

        Using http As New HttpClient With {.Timeout = TimeSpan.FromSeconds(30)}
            Dim body As New StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
            Dim answer = Await http.PostAsync("https://api.sms-webservice.com/api/V3/TokenList", body)
            Dim response = JsonNode.Parse(Await answer.Content.ReadAsStringAsync())

            Dim success = response?("Success")
            If success Is Nothing OrElse Not success.GetValue(Of Boolean)() Then
                Console.Error.WriteLine($"ناموفق. کد {response?("ErrorCode")}: {response?("Error")}")
                Return 1
            End If

            For Each template In response("Result").AsArray()
                Dim sendable = If(template("Status").GetValue(Of Integer)() = 2, "قابل ارسال", "قابل ارسال نیست")
                Console.WriteLine($"{template("Key")} ({sendable}): {template("TextTemplate")}")
            Next
        End Using

        Return 0
    End Function

End Module
' docs:end
