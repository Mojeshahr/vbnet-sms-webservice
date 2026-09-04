' AccountInfo - اعتبار باقی‌مانده و خطوط فعال حساب.
'
' سبک‌ترین متد سرویس و بهترین راه آزمودن کلید: چیزی ارسال نمی‌کند، اعتباری
' مصرف نمی‌کند، و حتی با اعتبار صفر هم جواب می‌دهد.
'
' جز کتابخانه استاندارد دات‌نت به چیزی وابسته نیست. کپی کنید و در پروژه
' خودتان اجرا کنید.
'
'   PAYAM_RESAN_API_KEY=... dotnet run -p:Example=examples/v3/account-info.vb

' docs:start
Option Strict On
Option Infer On

Imports System.Net.Http
Imports System.Text
Imports System.Text.Json.Nodes
Imports System.Threading.Tasks

Module Program

    Function Main() As Integer
        Return AccountInfo().GetAwaiter().GetResult()
    End Function

    Async Function AccountInfo() As Task(Of Integer)
        Dim payload As New JsonObject From {
            {"ApiKey", Environment.GetEnvironmentVariable("PAYAM_RESAN_API_KEY")}
        }

        Using http As New HttpClient With {.Timeout = TimeSpan.FromSeconds(30)}
            Dim body As New StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
            Dim answer = Await http.PostAsync("https://api.sms-webservice.com/api/V3/AccountInfo", body)
            Dim response = JsonNode.Parse(Await answer.Content.ReadAsStringAsync())

            Dim success = response?("Success")
            If success Is Nothing OrElse Not success.GetValue(Of Boolean)() Then
                Console.Error.WriteLine($"ناموفق. کد {response?("ErrorCode")}: {response?("Error")}")
                Return 1
            End If

            Console.WriteLine($"اعتبار: {response("Result")("Credit")}")

            For Each line In response("Result")("AvailableSenders").AsArray()
                Console.WriteLine($"خط: {line}")
            Next
        End Using

        Return 0
    End Function

End Module
' docs:end
