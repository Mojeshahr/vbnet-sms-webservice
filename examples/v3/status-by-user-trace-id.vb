' StatusByUserTraceId - وضعیت پیامک با شناسه‌هایی که خودتان داده‌اید.
'
' اگر UserTraceId را کلید رکورد پایگاه داده خودتان بگذارید، دیگر لازم نیست Id
' سامانه را ذخیره کنید. این متد راه امن تشخیص ارسال تکراری هم هست: بعد از قطع
' ارتباط، اول اینجا بپرسید ثبت شده یا نه.
'
' جز کتابخانه استاندارد دات‌نت به چیزی وابسته نیست. کپی کنید و در پروژه
' خودتان اجرا کنید.
'
'   PAYAM_RESAN_API_KEY=... dotnet run -p:Example=examples/v3/status-by-user-trace-id.vb

' docs:start
Option Strict On
Option Infer On

Imports System.Net.Http
Imports System.Text
Imports System.Text.Json.Nodes
Imports System.Threading.Tasks

Module Program

    Function Main() As Integer
        Return StatusByUserTraceId().GetAwaiter().GetResult()
    End Function

    Async Function StatusByUserTraceId() As Task(Of Integer)
        Dim payload As New JsonObject From {
            {"ApiKey", Environment.GetEnvironmentVariable("PAYAM_RESAN_API_KEY")},
            {"UserTraceIds", New JsonArray(JsonValue.Create(1001L), JsonValue.Create(1002L))}
        }

        Using http As New HttpClient With {.Timeout = TimeSpan.FromSeconds(30)}
            Dim body As New StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
            Dim answer = Await http.PostAsync("https://api.sms-webservice.com/api/V3/StatusByUserTraceId", body)
            Dim response = JsonNode.Parse(Await answer.Content.ReadAsStringAsync())

            Dim success = response?("Success")
            If success Is Nothing OrElse Not success.GetValue(Of Boolean)() Then
                Console.Error.WriteLine($"ناموفق. کد {response?("ErrorCode")}: {response?("Error")}")
                Return 1
            End If

            For Each message In response("Result").AsArray()
                ' کد ۸ یعنی این شناسه در حساب شما نیست. بعد از یک timeout، همین
                ' یعنی ارسال ثبت نشده و می‌توانید با خیال راحت دوباره بفرستید.
                If message("StatusCode").GetValue(Of Integer)() = 8 Then
                    Console.WriteLine($"{message("UserTraceId")}: ثبت نشده")
                    Continue For
                End If

                Console.WriteLine($"{message("UserTraceId")}: {message("Status")}")
            Next
        End Using

        Return 0
    End Function

End Module
' docs:end
